using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Observe
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public string m_sExecPath;
        public string m_sEnvPath;
        public string m_sMdbPath;
        public string m_sSelfIP;
        public int m_nPort;
        public string m_sRemoteIP;
        private Boolean m_bCheckMdb;
        private Thread m_rcvUDPThread;
        private UdpClient m_rcvUDPClient;
        private Boolean m_bUDPFlag;
        public LibCommon m_libCmn;
        public LibCanvas m_libCnvs;
        public LibOdbc m_libOdbc;
        DispatcherTimer m_dsptCheckTime;
        public BlockWin m_blockWin;
        public UnderWin m_underWin;
        private DispatcherTimer m_dsptWaitTime;
        public string m_sMapPath;
        private int m_nMapBase;
        private int m_nMapTblIdx;
        //                          10   11    12    13    14     15     16     17      18
        private int[] m_tblTopX = { 900, 1810, 3624, 7253, 14511, 29026, 58057, 116119, 232243 };
        private int[] m_tblTopY = { 395,  800, 1605, 3215,  6435, 12874, 25748,  51496, 102992 };
        private int[] m_tblEndX = { 915, 1823, 3643, 7283, 14563, 29123, 58247, 116495, 232991 };
        private int[] m_tblEndY = { 410,  811, 1619, 3234,  6465, 12927, 25855,  51711, 103423 };
        //                               10         11         12         13         14
        //                               15         16         17         18
        private double[] m_tblTopLat = { 35.906629, 35.906629, 35.906629, 35.906629, 35.906629 
                                       , 35.90685, 35.906629, 35.906629, 35.906629, };
        private double[] m_tblTopLnd = { 138.93274, 138.93274, 138.93274, 138.93274, 138.93274
                                       , 138.9331, 138.93274, 138.93274, 138.93274, };
        private double[] m_tblLatBlock = { -0.0089050, -0.0089050, -0.0089050, -0.0089050, -0.0089050
                                         , -0.0089050, -0.0089050, -0.0089050, -0.0089050, };
        private double[] m_tblLndBlock = {  0.0109850,  0.0109850,  0.0109850,  0.0109850,  0.0109850
                                         ,  0.0109850,  0.0109850,  0.0109850,  0.0109850, };
        private int m_nCanvasWidth, m_nCanvasHeight;
        private int m_nWidthDiv, m_nHeightDiv;
        private int m_nCenterXBlock, m_nCenterYBlock;
        private int m_nCenterXAdd, m_nCenterYAdd;
        private double m_dLatDotStep, m_dLndDotStep;
        private int m_nLastX, m_nLastY;
        private string[] m_aryPlaceNameLine;
        private Boolean m_bRetouMode;
        private Image m_imgCamera;
        private TextBlock m_tbCrt;
        public ClsCard m_clsCardCrt;
        private ClsCard m_clsCardBack;
        public ClsObserve m_clsObserve;

        public MainWindow()
        {
            InitializeComponent();
            m_sExecPath = initExePath();
            m_libCmn = new LibCommon();
            m_libCnvs = new LibCanvas();
            m_libOdbc = new LibOdbc();
            m_libOdbc.setLibCommonClass(m_libCmn);
            m_libOdbc.setExecPath(m_sExecPath);

            m_sEnvPath = initEnvPath();
            m_libOdbc.setEnvPath(m_sEnvPath);
            loadEnv();

            odbcLoadEnv();

            m_sMapPath = m_sEnvPath + "\\東京都";
            m_nMapBase = 15;
            setTableElement();
            m_bRetouMode = false;
            m_clsCardCrt = null;
            m_tbCrt = null;

            m_clsObserve = new ClsObserve();
        }
        private void setTableElement()
        {
            m_nMapTblIdx = m_nMapBase - 10;
            m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
            m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;
            m_dLatDotStep = m_tblLatBlock[m_nMapTblIdx] / Constants.MAPDOTSIZE;
            m_dLndDotStep = m_tblLndBlock[m_nMapTblIdx] / Constants.MAPDOTSIZE;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Type type;
            string sXmlFile;

            startCheckThread();
            //this.MaximizeBox
            sXmlFile = m_sEnvPath + "\\Observe.xml";
            type = typeof(ClsObserve);
            m_clsObserve = (ClsObserve)m_libCmn.DataXmlLoad(sXmlFile, type);
            if (m_clsObserve == null)
            {
                m_clsObserve = new ClsObserve();
            }
            if (m_clsObserve.m_lstClsCard.Count == 0)
            {
                m_clsCardCrt = null;
            }
            else
            {
                m_clsCardCrt = m_clsObserve.m_lstClsCard[0];
            }
            m_cnvsMove = new Canvas();
            initMapElement();
            initMouseEvent();
            initCmbGroup();
            initCmbPlaceName();
            m_dZoomTime = 1.0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
            initBlockWin();
            initUnderWin();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Type type;
            string sXmlFile;

            if (m_clsObserve == null)
            {
                return;
            }
            sXmlFile = m_sEnvPath + "\\Observe.xml";
            type = typeof(ClsObserve);
            m_libCmn.DataXmlSave(sXmlFile, type, m_clsObserve);
        }
        public void startCheckThread()
        {
            setCheckMdbFlag(true);
            m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 10);
            m_dsptCheckTime.Tick += new EventHandler(checkThread);
            m_dsptCheckTime.Start();
        }
        public void setCheckMdbFlag(Boolean flag)
        {
            if (flag == false)
            {
                m_bUDPFlag = false;
            }
            m_bCheckMdb = flag;
        }
        private void checkThread(object sender, EventArgs e)
        {
            if (m_bCheckMdb == true)
            {
                checkIO6Event();
            }
            else
            {
                if (m_bUDPFlag == true)
                {
                    setCheckMdbFlag(true);
                    m_rcvUDPClient.Close();
                    m_rcvUDPThread.Abort();
                    return;
                }
                m_rcvUDPThread = new System.Threading.Thread(new System.Threading.ThreadStart(checkUdpEvent));
                m_rcvUDPThread.Start();
            }
        }
        private void dispMameoryCrt()
        {
            long currentMem = Environment.WorkingSet;
            long currentCGMem = Environment.WorkingSet;
            //long currentCGMem = GC.GetTotalMemory(true);
            currentMem = currentMem / 1024;
            currentMem = currentMem / 1024;
            currentCGMem = currentCGMem / 1024;
            currentCGMem = currentCGMem / 1024;
        }
        private string initExePath()
        {
            string path;

            Assembly myAssembly = Assembly.GetEntryAssembly();
            path = myAssembly.Location;
            return (path.Substring(0, path.LastIndexOf("\\")));
        }
        private string initEnvPath()
        {
            string sEnvPath;
            string sOptFIleName;
            string[] aryLine;

            sEnvPath = "c:\\Observe";
            sOptFIleName = m_sExecPath + "\\observeopt.txt";
            aryLine = m_libCmn.LoadFileLineSJIS(sOptFIleName);
            if (aryLine[0] != "")
            {
                sEnvPath = aryLine[1];
            }
            m_libCmn.CreatePath(sEnvPath);
            return (sEnvPath);

        }
        private void loadEnv()
        {
            string sFileName;
            string[] aryLine;

            sFileName = m_sEnvPath + "\\Observe.env";
            aryLine = m_libCmn.LoadFileLineSJIS(sFileName);
            if (4 <= aryLine.Length)
            {
                m_sMdbPath = aryLine[1];
                m_sSelfIP = aryLine[2];
                m_nPort = m_libCmn.StrToInt(aryLine[3]);
                m_sRemoteIP = aryLine[4];
            }
        }
        public Boolean checkRemoteIP(IPAddress checkip)
        {
            if (m_sRemoteIP == "")
            {
                return (false);
            }
            IPAddress ipadrs = IPAddress.Parse(m_sRemoteIP);
            if (checkip == ipadrs)
            {
                return (true);
            }
            return (false);
        }
        public void ResetEnv()
        {
            if (m_cnvsMove == null)
            {
                return;
            }
            m_dZoomTime = 1.0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_cnvsMove == null)
            {
                return;
            }
            m_dZoomTime = 1.0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
        }
        private void initCmbGroup()
        {
            cmbGroup.Items.Add("23区");
            cmbGroup.Items.Add("市町");
            cmbGroup.Items.Add("離島");
            cmbGroup.SelectedIndex = 0;
            m_bRetouMode = false;
        }
        private void cmbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGroup != null && cmbPlaceName != null)
            {
                initCmbPlaceName();
            }
        }
        private void initCmbPlaceName()
        {
            string sFileName;
            int idx, max;
            string[] ary;

            cmbPlaceName.Items.Clear();
            if (cmbGroup.SelectedIndex == 0)
            {
                m_bRetouMode = false;
                sFileName = m_sExecPath + "\\ku.txt";
            }
            else if (cmbGroup.SelectedIndex == 1)
            {
                m_bRetouMode = false;
                sFileName = m_sExecPath + "\\si.txt";
            }
            else
            {
                m_bRetouMode = false;
                sFileName = m_sExecPath + "\\tou.txt";
            }
            m_aryPlaceNameLine = m_libCmn.LoadFileLineSJIS(sFileName);
            max = m_aryPlaceNameLine.Length;
            for (idx = 1; idx < max; idx++)
            {
                if (m_aryPlaceNameLine[idx] == "" || m_aryPlaceNameLine[idx] == " ")
                {
                    break;
                }
                ary = m_aryPlaceNameLine[idx].Split(',');
                cmbPlaceName.Items.Add(ary[0]);
            }
        }
        private void initBlockWin()
        {
            m_blockWin = new BlockWin();
            m_blockWin.SetMainWindow(this);
            m_blockWin.Owner = this;
            m_blockWin.Show();
        }
        private void initUnderWin()
        {
            m_underWin = new UnderWin();
            m_underWin.SetMainWindow(this);
            m_underWin.Owner = this;
            m_underWin.Show();
        }
        private void initMapElement()
        {
            int width;
            int height;
            int xidx, yidx, xmax, ymax;
            int xpos, ypos, size;

            width = (int)gridDrawArea.ActualWidth;
            height = (int)gridDrawArea.ActualHeight;
            m_nCanvasWidth = width;
            m_nCanvasHeight = height;
            m_nCenterXBlock = (m_nCanvasWidth / 2) / Constants.MAPDOTSIZE;
            m_nCenterYBlock = (m_nCanvasHeight / 2) / Constants.MAPDOTSIZE;
            m_nCenterXAdd = m_nCanvasWidth - m_nCenterXBlock * Constants.MAPDOTSIZE;
            m_nCenterYAdd = m_nCanvasHeight - m_nCenterXBlock * Constants.MAPDOTSIZE;

            m_nWidthDiv = m_nCanvasWidth / Constants.MAPDOTSIZE;
            if (m_nCanvasWidth % Constants.MAPDOTSIZE != 0)
            {
                m_nWidthDiv++;
            }
            m_nHeightDiv = m_nCanvasHeight / Constants.MAPDOTSIZE;
            if (m_nCanvasHeight % Constants.MAPDOTSIZE != 0)
            {
                m_nHeightDiv++;
            }

            m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
            m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;

            m_cnvsMove.Width = m_nWidthDiv * 3 * Constants.MAPDOTSIZE;
            m_cnvsMove.Height = m_nHeightDiv * 3 * Constants.MAPDOTSIZE;
        }
        private void timerWait()
        {
            m_dsptWaitTime = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptWaitTime.Interval = TimeSpan.FromMilliseconds(50);
            m_dsptWaitTime.Tick += new EventHandler(TickWaitTimer);
            m_dsptWaitTime.Start();
        }
        private void TickWaitTimer(object sender, EventArgs e)
        {
            m_dsptWaitTime.Stop();
        }
        private void moveLatLnd(double dLat, double dLnd)
        {
            double dSubLat, dSubLnd;
            int setxblock, setyblock;
            int setxdot, setydot;

            m_nMapBase = 15;
            m_nMapTblIdx = m_nMapBase - 10;
            m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
            m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;
            dSubLat = (dLat - m_tblTopLat[m_nMapTblIdx]);
            dSubLnd = (dLnd - m_tblTopLnd[m_nMapTblIdx]);
            setyblock = (int)(dSubLat / m_tblLatBlock[m_nMapTblIdx]);
            setxblock = (int)(dSubLnd / m_tblLndBlock[m_nMapTblIdx]);
            setydot = (int)((dSubLat - setyblock * m_tblLatBlock[m_nMapTblIdx]) / m_dLatDotStep);
            setxdot = (int)((dSubLnd - setxblock * m_tblLndBlock[m_nMapTblIdx]) / m_dLndDotStep);

            m_nCrtX = setxblock - m_nCenterXBlock;
            m_nCrtY = setyblock - m_nCenterYBlock;
            m_nAddX = m_nCanvasWidth / 2 - (m_nCenterXBlock * Constants.MAPDOTSIZE + setxdot);
            m_nAddY = m_nCanvasWidth / 2 - (m_nCenterYBlock * Constants.MAPDOTSIZE + setydot);
            m_nAddX = m_nAddX - m_nWidthDiv * Constants.MAPDOTSIZE;
            m_nAddY = m_nAddY - m_nHeightDiv * Constants.MAPDOTSIZE;
            m_dZoomTime = 1.0;
            m_nZoomAddX = 0;
            m_nZoomAddY = 0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
        }
        private void addCardMark()
        {
            int max, idx;
            double dLat, dLnd;
            string sStr;

            if (m_clsCardCrt == null)
            {
                return;
            }
            cnvsMarkArea.Children.Clear();
            if (m_nMapBase == 15)
            {
                addCnvsMoveCardMark(m_clsCardCrt.m_dLat, m_clsCardCrt.m_dLnd, m_clsCardCrt.m_sSyoNo);
                max = m_clsCardCrt.m_lstGpsPos.Count;
                for (idx = 0; idx < max; idx++)
                {
                    dLat = m_clsCardCrt.m_lstGpsPos[idx].m_dLat;
                    dLnd = m_clsCardCrt.m_lstGpsPos[idx].m_dLnd;
                    sStr = m_clsCardCrt.m_lstGpsPos[idx].m_sDate;
                    addCnvsMoveGPSMark(dLat, dLnd, sStr);
                }
            }
        }
        private void addCnvsMoveCardMark(double dLat, double dLnd, string sSetNo)
        {
            double subx, suby;
            int sx, sy;

            suby = (dLat - (m_tblTopLat[m_nMapTblIdx] + m_nCrtY * m_tblLatBlock[m_nMapTblIdx]));
            subx = (dLnd - (m_tblTopLnd[m_nMapTblIdx] + m_nCrtX * m_tblLndBlock[m_nMapTblIdx]));
            sy = (int)(suby / m_dLatDotStep * m_dZoomTime);
            sx = (int)(subx / m_dLndDotStep * m_dZoomTime);
            sx = sx + (int)(m_nAddX * m_dZoomTime + m_nMoveX + m_nZoomAddX);
            sy = sy + (int)(m_nAddY * m_dZoomTime + m_nMoveY + m_nZoomAddY);
            if (sx < 0 || sy < 0 || cnvsMarkArea.ActualWidth < sx || cnvsMarkArea.ActualHeight < sy)
            {
                return;
            }
            m_libCnvs.setFontSize(24);
            m_tbCrt = m_libCnvs.CreateTextBlock(0, 0, sSetNo);
            subx = 24 * sSetNo.Length / 4 - 12;
            Canvas.SetLeft(m_tbCrt, sx-subx);
            Canvas.SetTop(m_tbCrt, sy-24);
            cnvsMarkArea.Children.Add(m_tbCrt);
            m_imgCamera = new Image();
            Canvas.SetLeft(m_imgCamera, sx);
            Canvas.SetTop(m_imgCamera, sy);
            m_imgCamera.Width = 64;
            m_imgCamera.Stretch = Stretch.Fill;
            m_imgCamera.Source = new BitmapImage(new Uri("pic/camera.png", UriKind.Relative));
            cnvsMarkArea.Children.Add(m_imgCamera);
        }
        private void addCnvsMoveGPSMark(double dLat, double dLnd, string sStr)
        {
            double subx, suby;
            int sx, sy;
            TextBlock tb;

            suby = (dLat - (m_tblTopLat[m_nMapTblIdx] + m_nCrtY * m_tblLatBlock[m_nMapTblIdx]));
            subx = (dLnd - (m_tblTopLnd[m_nMapTblIdx] + m_nCrtX * m_tblLndBlock[m_nMapTblIdx]));
            sy = (int)(suby / m_dLatDotStep * m_dZoomTime);
            sx = (int)(subx / m_dLndDotStep * m_dZoomTime);
            sx = sx + (int)(m_nAddX * m_dZoomTime + m_nMoveX + m_nZoomAddX);
            sy = sy + (int)(m_nAddY * m_dZoomTime + m_nMoveY + m_nZoomAddY);
            if (sx < 0 || sy < 0 || cnvsMarkArea.ActualWidth < sx || cnvsMarkArea.ActualHeight < sy)
            {
                return;
            }
            m_libCnvs.setFontSize(24);
            tb = m_libCnvs.CreateTextBlock(0, 0, sStr);
            tb.Foreground = Brushes.Red;
            Canvas.SetLeft(tb, sx);
            Canvas.SetTop(tb, sy);
            cnvsMarkArea.Children.Add(tb);
        }
        private void setMarkPosition(int nxmap, int nymap, int nxdsp, int nydsp)
        {
            ClsLatLnd clsLatLnd;
            int nNo;
            CardWin cardWin;

            //m_clsCardCrt = new ClsCard();
            //m_clsObserve.m_lstClsCard.Add(m_clsCardCrt);
            m_clsCardBack = m_clsCardCrt;
            if (m_clsCardCrt == null)
            {
                m_clsCardBack = null;
                m_clsCardCrt = new ClsCard();
                m_clsObserve.m_lstClsCard.Add(m_clsCardCrt);
            }
            else
            {
                m_clsCardBack = new ClsCard();
                m_clsCardBack.m_dLat = m_clsCardCrt.m_dLat;
                m_clsCardBack.m_dLnd = m_clsCardCrt.m_dLnd;
                m_clsCardBack.m_sSetNo = m_clsCardCrt.m_sSetNo;
                m_clsCardBack.m_sIP = m_clsCardCrt.m_sIP;
                m_clsCardBack.m_sSyoNo = m_clsCardCrt.m_sSyoNo;
                m_clsCardBack.m_sAddress1 = m_clsCardCrt.m_sAddress1;
                m_clsCardBack.m_sAddress2 = m_clsCardCrt.m_sAddress2;
                m_clsCardBack.m_sTel1 = m_clsCardCrt.m_sTel1;
                m_clsCardBack.m_sTel2 = m_clsCardCrt.m_sTel2;
                m_clsCardBack.m_sName = m_clsCardCrt.m_sName;
                m_clsCardBack.m_sBikou = m_clsCardCrt.m_sBikou;
                m_clsCardBack.m_lstGpsPos = m_clsCardCrt.m_lstGpsPos;
            }
            //nNo = m_clsObserve.m_lstClsCard.Count+1;
            //m_clsCardCrt.m_sSetNo = nNo.ToString("0000000");
            m_clsCardCrt.m_sSyoNo = "";
            clsLatLnd = getMousePosToLatLnd(nxmap, nymap);
            m_clsCardCrt.m_dLat = clsLatLnd.m_dLat;
            m_clsCardCrt.m_dLnd = clsLatLnd.m_dLnd;
            addCnvsMoveCardMark(m_clsCardCrt.m_dLat, m_clsCardCrt.m_dLnd, m_clsCardCrt.m_sSyoNo);

            cardWin = new CardWin();
            cardWin.SetMainWindow(this);
            cardWin.SetClsCard(m_clsCardCrt);
            cardWin.Left = 400;
            cardWin.Top = 500;
            cardWin.Owner = this;
            cardWin.ShowDialog();
            if (m_clsCardCrt == null)
            {
                return;
            }
        }
        public void SetClsCardElement(ClsCard clsCard)
        {
            m_clsCardCrt.m_sSetNo = clsCard.m_sSetNo;
            m_clsCardCrt.m_sIP = clsCard.m_sIP;
            m_clsCardCrt.m_sSyoNo = clsCard.m_sSyoNo;
            m_clsCardCrt.m_sName = clsCard.m_sName;
            m_clsCardCrt.m_sAddress1 = clsCard.m_sAddress1;
            m_clsCardCrt.m_sAddress2 = clsCard.m_sAddress2;
            m_clsCardCrt.m_sTel1 = clsCard.m_sTel1;
            m_clsCardCrt.m_sTel2 = clsCard.m_sTel2;
            m_clsCardCrt.m_sBikou = clsCard.m_sBikou;
            m_clsCardCrt.m_lstGpsPos.Clear();

            addCardMark();
        }
        public void ResetClsCardElement()
        {
            if (m_clsCardBack == null)
            {
                m_clsObserve.m_lstClsCard.Clear();
                m_clsCardCrt = null;
            }
            else
            {
                m_clsCardCrt.m_dLat = m_clsCardBack.m_dLat;
                m_clsCardCrt.m_dLnd = m_clsCardBack.m_dLnd;
                m_clsCardCrt.m_sSetNo = m_clsCardBack.m_sSetNo;
                m_clsCardCrt.m_sIP = m_clsCardBack.m_sIP;
                m_clsCardCrt.m_sSyoNo = m_clsCardBack.m_sSyoNo;
                m_clsCardCrt.m_sAddress1 = m_clsCardBack.m_sAddress1;
                m_clsCardCrt.m_sAddress2 = m_clsCardBack.m_sAddress2;
                m_clsCardCrt.m_sTel1 = m_clsCardBack.m_sTel1;
                m_clsCardCrt.m_sTel2 = m_clsCardBack.m_sTel2;
                m_clsCardCrt.m_sName = m_clsCardBack.m_sName;
                m_clsCardCrt.m_sBikou = m_clsCardBack.m_sBikou;
                addCnvsMoveCardMark(m_clsCardCrt.m_dLat, m_clsCardCrt.m_dLnd, m_clsCardCrt.m_sSyoNo);
            }

            addCardMark();
        }
        private ClsLatLnd getMousePosToLatLnd(int nx, int ny)
        {
            double dPosX, dPosY;
            ClsLatLnd clsLatLnd;
            double dCanvasOffsetX, dCanvasOffsetY;

            clsLatLnd = new ClsLatLnd();
            dPosX = (double)(nx);
            dPosY = (double)(ny);
            dCanvasOffsetX = dPosX;
            dCanvasOffsetY = dPosY;
            clsLatLnd.m_dLat = m_tblTopLat[m_nMapTblIdx] + m_nCrtY * m_tblLatBlock[m_nMapTblIdx] + dCanvasOffsetY * m_dLatDotStep;
            clsLatLnd.m_dLnd = m_tblTopLnd[m_nMapTblIdx] + m_nCrtX * m_tblLndBlock[m_nMapTblIdx] + dCanvasOffsetX * m_dLndDotStep;
            return (clsLatLnd);
        }
        private void cmbPlaceName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sPlace;
            string[] ary;
            int nSXBlock;
            int nSYBlock;
            double dLat, dLnd;

            int idx = cmbPlaceName.SelectedIndex;
            if (idx == -1)
            {
                return;
            }
            sPlace = m_aryPlaceNameLine[idx + 1];
            ary = sPlace.Split(',');
            if (cmbGroup.SelectedIndex == 2)
            {
                m_nMapBase = 15;
                m_nMapTblIdx = m_nMapBase - 10;
                m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
                m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;
                m_bRetouMode = true;
                nSXBlock = m_libCmn.StrToInt(ary[3]) - m_tblTopX[m_nMapTblIdx];
                nSYBlock = m_libCmn.StrToInt(ary[4]) - m_tblTopY[m_nMapTblIdx];
                m_nCrtX = nSXBlock - m_nWidthDiv;
                m_nCrtY = nSYBlock - m_nHeightDiv;
                m_nAddX = -m_nWidthDiv * Constants.MAPDOTSIZE;
                m_nAddY = -m_nHeightDiv * Constants.MAPDOTSIZE;
                m_dZoomTime = 1.0;
                m_cnvsMove.RenderTransform = null;
                initMapArea();
            }
            else
            {
                m_bRetouMode = false;
                dLat = m_libCmn.StrToDouble(ary[1]);
                dLnd = m_libCmn.StrToDouble(ary[2]);
                moveLatLnd(dLat, dLnd);
            }
        }
        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            string sAddress;
            OdbcConnection con;
            int max, len;
            string sBeforeSql;
            string sSql;
            OdbcCommand com;
            OdbcDataReader reader;
            string sSubStr;
            double dLat, dLnd;

            sAddress = txtAddress.Text;
            sAddress = m_libCmn.StrNumToKan(sAddress);
            con = m_libOdbc.openMdb();
            if (con != null)
            {
                sBeforeSql = "";
                max = sAddress.Length;
                for(len = 2; len < max; len++){
                    sSubStr = sAddress.Substring(0, len);
                    sSql = "SELECT * FROM adrslatlnd";
                    sSql = sSql + " WHERE address LIKE '%"+sSubStr+"%';";
                    com = new OdbcCommand(sSql, con);
                    try
                    {
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            sBeforeSql = sSql;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                com = new OdbcCommand(sBeforeSql, con);
                try
                {
                    reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        dLat = reader.GetDouble(2);
                        dLnd = reader.GetDouble(3);
                        moveLatLnd(dLat, dLnd);
                        break;
                    }
                }
                catch (Exception ex)
                {
                }
                m_libOdbc.closeMdb(con);
            }
        }
    }
}

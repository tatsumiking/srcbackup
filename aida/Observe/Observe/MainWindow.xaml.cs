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
        public Boolean m_bUDPInFlag;
        public LibCommon m_libCmn;
        public LibCanvas m_libCnvs;
        public LibOdbc m_libOdbc;
        public BlockWin m_blockWin;
        public UnderWin m_underWin;
        public CardWin m_cardWin;
        private DispatcherTimer m_dsptCheckTime;
        private DispatcherTimer m_dsptWaitTime;
        public string m_sMapPath;
        //実際の地図は+-4データあり 10   11    12    13    14     15     16     17      18
        private int[] m_tblTopX = { 907, 1814, 3628, 7257, 14515, 29030, 58061, 116123, 232247 };
        private int[] m_tblTopY = { 402,  804, 1609, 3219,  6439, 12878, 25757,  51513, 103031 };
        private int[] m_tblEndX = { 909, 1819, 3639, 7279, 14559, 29119, 58239, 116479, 232959 };
        private int[] m_tblEndY = { 403,  807, 1615, 3230,  6461, 12923, 25849,  51693, 103387 };
        //private int[] m_tblTopX = { 903, 1810, 3624, 7253, 14511, 29026, 58057, 116119, 232243 };
        //private int[] m_tblTopY = { 398, 800, 1605, 3215, 6435, 12874, 25748, 51496, 102992 };
        //private int[] m_tblEndX = { 915, 1823, 3643, 7283, 14563, 29123, 58247, 116495, 232991 };
        //private int[] m_tblEndY = { 410, 811, 1619, 3234, 6465, 12927, 25855, 51711, 103423 };
        //                               10         11         12          13          14
        //                               15         16         17          18
        private double[] m_tblTopLat = {  36.03150,  36.03130,  35.9602,  35.92465,  35.90685
                                       ,  36.03125,  35.90240,  35.900175,  35.899065,   0};
        private double[] m_tblTopLnd = { 138.8672, 138.8672, 138.8671, 138.91115, 138.9331
                                       , 138.93310, 138.93860, 138.941345, 138.942720,   0};

        private double[] m_tblLatBlock = { -0.284444, -0.142222, -0.071111, -0.035555, -0.017777
                                         , -0.008888, -0.004444, -0.002222, -0.001111,  0};
        private double[] m_tblLndBlock = { 0.351562496,0.175781248,0.087890624,0.043945312,0.021972656
                                          ,0.010986328,0.005493164,0.002746582,0.001373291,0};
        private int[] m_tblBaseY = { 402, 804, 1608, 3216, 6432, 12864, 25728, 51456, 102912, 0 };
        private double m_dBaseLat;
        private int m_n18BlockLatLast;
        private double m_d18BlockLatAdd; 
        private double m_dStepLatSub;
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
        public int m_nClsCardCrtIdx;
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
            m_dBaseLat = 36.03150;
            m_d18BlockLatAdd = -0.0011105;
            m_dStepLatSub = -0.0000001579;
            m_n18BlockLatLast = m_tblEndY[8] - m_tblBaseY[8];
            m_bRetouMode = false;
            m_tbCrt = null;
            m_clsObserve = null;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Type type;
            string sXmlFile;

            //this.MaximizeBox
            sXmlFile = m_sEnvPath + "\\Observe.xml";
            type = typeof(ClsObserve);
            m_clsObserve = (ClsObserve)m_libCmn.DataXmlLoad(sXmlFile, type);
            if (m_clsObserve == null)
            {
                m_clsObserve = new ClsObserve();
            }
            setCrtCardIdx(-1);

            m_cnvsMove = new Canvas();

            initMouseEvent();
            initMapElement();
            initCmbGroup();
            initCmbPlaceName();
            m_dZoomTime = 1.0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
            initBlockWin();
            initUnderWin();
            startCheckThread();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Type type;
            string sXmlFile;

            saveCrtTizuPos();
            if (m_clsObserve == null)
            {
                return;
            }
            sXmlFile = m_sEnvPath + "\\Observe.xml";
            type = typeof(ClsObserve);
            m_libCmn.DataXmlSave(sXmlFile, type, m_clsObserve);
        }
        public void setCrtCardIdx(int idx)
        {
            m_nClsCardCrtIdx = idx;
        }
        public int getCrtCardIdx()
        {
            return (m_nClsCardCrtIdx);
        }
        public void startCheckThread()
        {
            m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 10);
            m_dsptCheckTime.Tick += new EventHandler(checkThread);
            m_dsptCheckTime.Start();
        }
        public void resetUDPFlag()
        {
            int max, idx;

            max = m_clsObserve.m_lstClsCard.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (m_clsObserve.m_lstClsCard[idx].m_sStat == "2")
                {
                    m_clsObserve.m_lstClsCard[idx].m_sStat = "";
                    setUnderMsg("");
                    setBlockWin();
                }
            }
            m_bUDPFlag = false;
            m_bUDPInFlag = false;
        }
        public void setCheckStartUdp()
        {
            m_bUDPFlag = true;
        }
        private void checkThread(object sender, EventArgs e)
        {
            checkMdbElement();
            if (m_bUDPFlag == true)
            {
                if(m_bUDPInFlag == false)
                {
                    m_rcvUDPThread = new System.Threading.Thread(new System.Threading.ThreadStart(checkUdpThread));
                    m_rcvUDPThread.Start();
                }
            }
        }
        public void checkUdpThread()
        {
            m_bUDPInFlag = true;
            udpReceiveGPSData();
            m_bUDPInFlag = false;
            addCardMark();
        }
        private int searchCardIdx(String sId)
        {
            int max, idx;

            max = m_clsObserve.m_lstClsCard.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (m_clsObserve.m_lstClsCard[idx].m_sSetNo == sId)
                {
                    return (idx);
                }
            }
            return (-1);
        }
        private void setUnderMsg(String sMsg)
        {
            m_underWin.dispMsg(sMsg);
        }
        private void setBlockWin()
        {
            m_blockWin.SetListElement();
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

            // m_nPort = 5547;
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

            m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv;
            m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv;

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
        public void moveLatLnd(ClsLatLnd clsLatLnd)
        {
            ClsPagePosXY clsPagePosXY;
            int setxPage, setyPage;
            int setxdot, setydot;

            //m_nMapBase = 15;
            //setTableElement(); // m_nMapTblIdx, m_nLastX, m_nLastY, m_dLatDotStep, m_dLndDotStep 設定

            // ページ数＋ページ位置算出
            clsPagePosXY = convLatLndToRltvPagePosXY(clsLatLnd);
            // ページ数セット
            setxPage = (int)(clsPagePosXY.m_dPagePosX);
            setyPage = (int)(clsPagePosXY.m_dPagePosY);
            // ドット数
            setxdot = (int)((clsPagePosXY.m_dPagePosX - setxPage) * Constants.MAPDOTSIZE);
            setydot = (int)((clsPagePosXY.m_dPagePosY - setyPage) * Constants.MAPDOTSIZE);

            m_nCrtX = setxPage - m_nCenterXBlock - m_nWidthDiv;
            m_nCrtY = setyPage - m_nCenterYBlock - m_nHeightDiv;
            int nPageX = m_nCrtX + m_tblTopX[m_nMapTblIdx];
            int nPageY = m_nCrtY + m_tblTopY[m_nMapTblIdx];
            m_nAddX = m_nCanvasWidth / 2 - (m_nCenterXBlock * Constants.MAPDOTSIZE + setxdot);
            m_nAddY = m_nCanvasHeight / 2 - (m_nCenterYBlock * Constants.MAPDOTSIZE + setydot);
            m_nAddX = m_nAddX - m_nWidthDiv * Constants.MAPDOTSIZE;
            m_nAddY = m_nAddY - m_nHeightDiv * Constants.MAPDOTSIZE;
            m_dZoomTime = 1.0;
            m_nZoomAddX = 0;
            m_nZoomAddY = 0;
            m_cnvsMove.RenderTransform = null;
            initMapArea();
        }
        private ClsPagePosXY convLatLndToRltvPagePosXY(ClsLatLnd clsLatLnd)
        {
            ClsPagePosXY clsPagePosXY;
            double dSubLat, dSubLnd;

            clsPagePosXY = new ClsPagePosXY();
            dSubLnd = (clsLatLnd.m_dLnd - m_tblTopLnd[m_nMapTblIdx]);
            clsPagePosXY.m_dPagePosX = dSubLnd / m_tblLndBlock[m_nMapTblIdx];
            m_dLndDotStep = m_tblLndBlock[m_nMapTblIdx] / Constants.MAPDOTSIZE;
            if (clsPagePosXY.m_dPagePosX < 0)
            {
                clsPagePosXY.m_dPagePosX = 0;
            }
            if ((double)m_nLastX < clsPagePosXY.m_dPagePosX)
            {
                clsPagePosXY.m_dPagePosX = (double)m_nLastX;
            }

            dSubLat = clsLatLnd.m_dLat - m_dBaseLat;
            clsPagePosXY.m_dPagePosY = getLatBlockPagePos(dSubLat);
            return (clsPagePosXY);
        }
        public double getLatBlockPagePos(double dSubLat)
        {
            double  dCrtSubLat;
            double  d18AddStep;
            double  dPagePos;
            int     subPage;
            int     n18LastY;
            int     idx;

            dCrtSubLat = 0;
            d18AddStep = m_d18BlockLatAdd;
            dPagePos = dSubLat / d18AddStep;
            dPagePos = 0.0;
            n18LastY = m_tblEndY[8] - m_tblBaseY[8];
            // 18z時のページポジションを求める
            for (subPage = 10; subPage < n18LastY; subPage += 10)
            {
                if((dCrtSubLat + d18AddStep * 10) < dSubLat){
                    dPagePos = subPage - 10;
                    dPagePos = dPagePos + ((dSubLat - dCrtSubLat) / (d18AddStep));
                    break;
                }
                dCrtSubLat = dCrtSubLat + d18AddStep * 10;
                d18AddStep = d18AddStep + m_dStepLatSub;
            }
            m_dLatDotStep = d18AddStep / Constants.MAPDOTSIZE;
            for(idx = 8; idx >= 0; idx--){
                if(m_nMapTblIdx == idx){
                    dPagePos = dPagePos + m_tblBaseY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx];
                    if(dPagePos < 0){
                        dPagePos = 0;
                    }
                    if((double)m_nLastY < dPagePos){
                        dPagePos = (double)m_nLastY;
                    }
                    return(dPagePos);
                }
                m_dLatDotStep = m_dLatDotStep * 2;
                dPagePos = dPagePos / 2.0;
            }
            return (0.0);
        }
        public void setCrtCardWinDisp()
        {
            int nCrtIdx;
            ClsCard clsCard;

            nCrtIdx = getCrtCardIdx();
            if (nCrtIdx == -1)
            {
                m_clsCardBack = null;
                clsCard = new ClsCard();
                nCrtIdx = m_clsObserve.m_lstClsCard.Count;
                m_clsObserve.m_lstClsCard.Add(clsCard);
                setCrtCardIdx(nCrtIdx);
            }
            else
            {
                m_clsCardBack = new ClsCard();
                m_clsCardBack.copySetElement(m_clsObserve.m_lstClsCard[nCrtIdx]);
                clsCard = m_clsCardBack;
            }
            m_cardWin = new CardWin();
            m_cardWin.SetMainWindow(this);
            m_cardWin.SetClsCard(clsCard);
            m_cardWin.Owner = this;
            m_cardWin.Show();
        }
        public void SetCrtCardElement(ClsCard clsCard)
        {
            int nCrtIdx;

            nCrtIdx = getCrtCardIdx();
            if (nCrtIdx == -1)
            {
                return;
            }
            m_clsObserve.m_lstClsCard[nCrtIdx].copySetElement(clsCard);

            m_blockWin.SetListElement();
            addCardMark();
        }
        public void ResetClsCardElement()
        {
            int nCrtIdx;

            nCrtIdx = getCrtCardIdx();
            if(nCrtIdx == -1)
            {
                return;
            }
            if (m_clsCardBack == null)
            {
                // m_clsObserve.m_lstClsCard.Clear();
                setCrtCardIdx(-1);
                return;
            }
            m_clsObserve.m_lstClsCard[nCrtIdx].copySetElement(m_clsCardBack);
            addCardMark();
        }
        public void SetCrtCardLatLnd(double dLat, double dLnd)
        {
            int nCrtIdx;

            nCrtIdx = getCrtCardIdx();
            if (nCrtIdx == -1)
            {
                return;
            }
            m_clsObserve.m_lstClsCard[nCrtIdx].m_dLat = dLat;
            m_clsObserve.m_lstClsCard[nCrtIdx].m_dLnd = dLnd;

            addCardMark();
        }
        private void cmbPlaceName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sPlace;
            string[] ary;
            int nSXBlock;
            int nSYBlock;
            ClsLatLnd clsLatLnd;

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
                clsLatLnd = new ClsLatLnd();
                clsLatLnd.m_dLat = m_libCmn.StrToDouble(ary[1]);
                clsLatLnd.m_dLnd = m_libCmn.StrToDouble(ary[2]);
                moveLatLnd(clsLatLnd);
            }
        }
        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            string sAddress;
            ClsLatLnd clsLatLnd;

            sAddress = txtAddress.Text;
            clsLatLnd = getAddressToLatLnd(sAddress);
            moveLatLnd(clsLatLnd);
        }
        public ClsLatLnd getAddressToLatLnd(String sAddress)
        {
            ClsLatLnd latlnd;
            OdbcConnection con;
            int max, len;
            string sBeforeSql;
            string sSql;
            OdbcCommand com;
            OdbcDataReader reader;
            string sSubStr;

            latlnd = new ClsLatLnd();
            latlnd.m_dLat = 0;
            latlnd.m_dLnd = 0;
            sAddress = m_libCmn.StrNumToKan(sAddress);
            if (sAddress.Substring(0, 3) == "東京都")
            {
                sAddress = sAddress.Substring(3);
            }
            con = m_libOdbc.openMdb();
            if (con != null)
            {
                sBeforeSql = "";
                max = sAddress.Length;
                for(len = 2; len < max; len++){
                    sSubStr = sAddress.Substring(0, len);
                    sSql = "SELECT * FROM adrslatlnd";
                    sSql = sSql + " WHERE adrs LIKE '%"+sSubStr+"%';";
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
                        latlnd.m_dLat = reader.GetDouble(2);
                        latlnd.m_dLnd = reader.GetDouble(3);
                        break;
                    }
                }
                catch (Exception ex)
                {
                }
                m_libOdbc.closeMdb(con);
            }
            return (latlnd);
        }
    }
}

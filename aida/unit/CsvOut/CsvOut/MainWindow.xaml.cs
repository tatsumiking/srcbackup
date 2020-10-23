using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace CsvOut
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string m_sExecPath;
        private string m_sEnvPath;
        private LibCommon m_libCmn;
        DispatcherTimer m_dsptCheckTime;
        private List<string> m_lstCsvStr;
        private string m_sSavePath;
        private int m_nCheckTime;
        private string m_sBaseDate;
        private string m_sBaseTime;
        private Boolean m_bCheckFlag;
        private string m_sLastDate;
        private string m_sLastTime;

        public MainWindow()
        {
            InitializeComponent();
            m_libCmn = new LibCommon();
            m_sExecPath = InitExePath();
            m_sEnvPath = InitEnvPath();

            EnvFileLoad();
            InitCmbHour();
            InitCmbMinute();
#if DEBUG
            m_sBaseDate = "20161027";
            m_sBaseTime = "050000";
#endif
            MainWindowODBCInit();
            m_bCheckFlag = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int nHM, nHH, nMM;

            txtPath.Text = m_sSavePath;
            nHM = m_nCheckTime / 100;
            nHH = nHM / 100;
            nMM = nHM % 100;
            cmbHour.Text = nHH.ToString("D2");
            cmbMinute.Text = nMM.ToString("D2");
            m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 60);
            m_dsptCheckTime.Tick += new EventHandler(TickCheckTimeLoop);
            m_dsptCheckTime.Start();
        }
        private void TickCheckTimeLoop(object sender, EventArgs e)
        {
            string sCrtTime;
            int nCrtTime;

            DateTime dt = DateTime.Now;
            sCrtTime = dt.ToString("HHmmss");
            nCrtTime = m_libCmn.StrToInt(sCrtTime);
            if (m_nCheckTime <= nCrtTime)
            {
                if (m_bCheckFlag == true)
                {
                    lblMsg.Content = "データ取得中";
                    if (ODBCOpenUnisDB() == true)
                    {
                        ODBCSelecttEnter();
                        ODBCCloseUnisDB();
                        SaveCsvFile();
                        m_sBaseDate = m_sLastDate;
                        m_sBaseTime = m_sLastTime;
                        m_bCheckFlag = false;
                    } 
                }
            }
            else
            {
                if (m_bCheckFlag == false)
                {
                    m_bCheckFlag = true;
                }
            }
        }
        private void SaveCsvFile()
        {
            string sSaveFileName;
            string sData;
            int idx, max;
            string sCrtDate;

            lblMsg.Content = "保存中";
            sData = "社員番号,yyyy/mm/dd,hh:mm,出退勤フラグ,固定値,\n";
            max = m_lstCsvStr.Count;
            if (max == 0)
            {
                return;
            }
            for (idx = 0; idx < max; idx++)
            {
                sData = sData + m_lstCsvStr[idx];
            }
            DateTime dt = DateTime.Now;
            sCrtDate = dt.ToString("yyyyMMdd");
            sSaveFileName = m_sSavePath + "\\" + sCrtDate + ".csv";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
            lblMsg.Content = "待機中";
        }
        private void EnvFileLoad()
        {
            string sLoadFileName;
            string sData;
            string[] aryLine;


            DateTime dt = DateTime.Now;
            m_sSavePath = "c:\\csvout";
            m_nCheckTime = 230000;
            m_sBaseDate = dt.ToString("yyyyMMdd");
            m_sBaseTime = dt.ToString("HHmmss");
            sLoadFileName = m_sEnvPath + "\\csvout.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_sSavePath = aryLine[1];
                m_nCheckTime = m_libCmn.StrToInt(aryLine[2]);
                m_sBaseDate = aryLine[3];
                m_sBaseTime = aryLine[4];
            }
            m_libCmn.CreatePath(m_sSavePath);
        }
        private void EnvFileSave()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\csvout.env";
            sData = "// csvout env\n";
            sData = sData + m_sSavePath + "\n";
            sData = sData + m_nCheckTime + "\n";
            sData = sData + m_sBaseDate + "\n";
            sData = sData + m_sBaseTime + "\n";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private string InitExePath()
        {
            string path;

            Assembly myAssembly = Assembly.GetEntryAssembly();
            path = myAssembly.Location;
            return (path.Substring(0, path.LastIndexOf("\\")));
        }
        private string InitEnvPath()
        {
            string sEnvPath;

            sEnvPath = "c:\\ProgramData";
            m_libCmn.CreatePath(sEnvPath);
            sEnvPath = sEnvPath + "\\uniscsv";
            m_libCmn.CreatePath(sEnvPath);
            return (sEnvPath);
        }
        private void InitCmbHour()
        {
            string[] lst = new string[]
            { "00", "01", "02", "03", "04"
            , "05", "06", "07", "08", "09"
            , "10", "11", "12", "13", "14"
            , "15", "16", "17", "18", "19"
            , "20", "21", "22", "23"
            };

            cmbHour.ItemsSource = lst;
            cmbHour.IsEditable = false;
        }
        private void InitCmbMinute()
        {
            string[] lst = new string[]
            { "00", "10", "20", "30", "40", "50"};

            cmbMinute.ItemsSource = lst;
            cmbMinute.IsEditable = false;
        }
        private void btnSlct_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = txtPath.Text;
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = fbd.SelectedPath;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            string sHH, sMM;
            string sCheckTime;

            m_sSavePath = txtPath.Text;
            sHH = cmbHour.Text;
            sMM = cmbMinute.Text;
            sCheckTime = sHH + sMM + "00";
            m_nCheckTime = m_libCmn.StrToInt(sCheckTime);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            EnvFileSave();
            this.Close();
        }
    }
}

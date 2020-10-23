using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private string m_sPostFileName;
        private string m_sDelimiter;
        DispatcherTimer m_dsptCheckTime;
        private List<string> m_lstCsvStr;
        private string[] m_aryCsvTitleTbl;
        private string[] m_aryFildKeyTbl;
        private string[] m_aryFucStrTbl;
        private string m_sSavePath;
        private string m_sCheckDDHHmm;
        private string m_sBaseDate;
        private string m_sBaseTime;
        private Boolean m_bCheckFlag;
        private string m_sLastDate;
        private string m_sLastTime;
        private string m_sCheckTime;

        public MainWindow()
        {
            InitializeComponent();
            m_dsptCheckTime = null;
            m_libCmn = new LibCommon();
            m_sExecPath = InitExePath();
            m_sEnvPath = InitEnvPath();

            EnvFileLoad();
            FildKeyFileLoad();
            InitcmbDelimiter();
            InitCmbDay();
            InitCmbHour();
            InitCmbMinute();
            MainWindowODBCInit();
            m_bCheckFlag = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string sMsg;
            if (m_sBaseDate == "0")
            {
                sMsg = "締め日時分等を設定し「適応」ボタンを押してください";
                MessageBox.Show(sMsg);
            }
            else
            {
                m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
                m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 30);
                m_dsptCheckTime.Tick += new EventHandler(TickCheckTimeLoop);
                m_dsptCheckTime.Start();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int nSelect;
            string sDay, sHH, sMM;

            txtPath.Text = m_sSavePath;
            txtPostFileName.Text = m_sPostFileName;

            nSelect = 0;
            if (m_sDelimiter == ",")
            {
                nSelect = 0;
            }
            else if (m_sDelimiter == "\t")
            {
                nSelect = 1;
            }
            else if (m_sDelimiter == " ")
            {
                nSelect = 2;
            }
            cmbDelimiter.SelectedIndex = nSelect;

            sDay = m_sCheckDDHHmm.Substring(0, 2);
            sHH = m_sCheckDDHHmm.Substring(2, 2);
            sMM = m_sCheckDDHHmm.Substring(4, 2);
            if (sDay.CompareTo("31") < 0)
            {
                cmbDay.Text = sDay;
            }
            else
            {
                cmbDay.Text = "末";
            }
            cmbHour.Text = sHH;
            cmbMinute.Text = sMM;
        }
        private void TickCheckTimeLoop(object sender, EventArgs e)
        {
            string sCrtTime;

            DateTime dt = DateTime.Now;
            sCrtTime = dt.ToString("yyyyMMddHHmmss");
            if (m_sCheckTime.CompareTo(sCrtTime) < 0)
            {
                if (m_bCheckFlag == true)
                {
                    lblMsg.Content = "データ取得中";
                    GetODBCDataToFile();
                    m_bCheckFlag = false;
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
        private void GetODBCDataToFile()
        {
            string sMsg;
            string sCrtDate;
            string sSaveFileName;

            sCrtDate = m_sCheckTime.Substring(0, 8);
            sSaveFileName = m_sSavePath + "\\" + sCrtDate + m_sPostFileName + ".csv";
            if (File.Exists(sSaveFileName) == true){
                sMsg = sSaveFileName + "が存在します上書きしますか";
                if (MessageBox.Show(sMsg, "Information", MessageBoxButton.YesNo,
                     MessageBoxImage.Information) == MessageBoxResult.No)
                {
                    m_sBaseDate = m_sCheckTime.Substring(0, 8);
                    m_sBaseTime = m_sCheckTime.Substring(8, 6);
                    incrementCheckTime(m_sBaseDate, m_sCheckDDHHmm);
                    return;
                }
            }

            if (ODBCOpenUnisDB() == true)
            {
                ODBCSelecttEnter();
                ODBCCloseUnisDB();
                SaveCsvFile(sSaveFileName);
                m_sBaseDate = m_sCheckTime.Substring(0, 8);
                m_sBaseTime = m_sCheckTime.Substring(8, 6);
                incrementCheckTime(m_sBaseDate, m_sCheckDDHHmm);
            }
        }

        private void SaveCsvFile(string sSaveFileName)
        {
            int fldmax, fldidx;
            string sData;
            int idx, max;

            lblMsg.Content = "保存中";
            fldmax = m_aryCsvTitleTbl.Length;
            sData = "";
            for(fldidx = 0 ; fldidx < fldmax; fldidx++){
                sData = sData + m_aryCsvTitleTbl[fldidx]+ m_sDelimiter;
            }
            sData = sData + "\n";
            max = m_lstCsvStr.Count;
            for (idx = 0; idx < max; idx++)
            {
                sData = sData + m_lstCsvStr[idx];
            }

            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
            lblMsg.Content = "待機中";
        }
        private void EnvFileLoad()
        {
            string sYY, sMM;
            string sLoadFileName;
            string sData;
            string[] aryLine;
            string sMsg;

            m_sSavePath = "c:\\MajorFlow";
            m_sPostFileName = "month";
            m_sDelimiter = ",";
            m_sCheckDDHHmm = "322359";
            m_sBaseDate = "0";
            m_sBaseTime = "2359";
            sLoadFileName = m_sEnvPath + "\\csvout.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_sSavePath = aryLine[1];
                m_sPostFileName = aryLine[2];
                m_sDelimiter = aryLine[3];
                m_sCheckDDHHmm = aryLine[4];
                m_sBaseDate = aryLine[5];
                m_sBaseTime = aryLine[6];
            }
            DateTime dt = DateTime.Now;
            sYY = dt.ToString("yyyy");
            sMM = dt.ToString("MM");
            UpDateCheckTime(sYY, sMM, m_sCheckDDHHmm);
            m_libCmn.CreatePath(m_sSavePath);
        }
        private void incrementCheckTime(string sBaseDate, string sDDHHmm)
        {
            string sYY, sMM;
            int nYY, nMM;

            sYY = sBaseDate.Substring(0, 4);
            sMM = sBaseDate.Substring(4, 2);
            nYY = m_libCmn.StrToInt(sYY);
            nMM = m_libCmn.StrToInt(sMM);
            if (nMM == 12)
            {
                nMM = 1;
                nYY++;
            }
            else
            {
                nMM++;
            }
            sYY = nYY.ToString("D4");
            sMM = nMM.ToString("D2");
            UpDateCheckTime(sYY, sMM, sDDHHmm);

        }
        private void UpDateCheckTime(string sYY, string sMM, string sDDHHmm)
        {
            string sDD, sLast;
            int nYY, nMM, nDD;

            sDD = sDDHHmm.Substring(0, 2);
            nYY = m_libCmn.StrToInt(sYY);
            nMM = m_libCmn.StrToInt(sMM);
            nDD = DateTime.DaysInMonth(nYY, nMM);
            sLast = nDD.ToString("D2");
            if (sLast.CompareTo(sDD) < 0)
            {
                sDD = sLast;
            }
            m_sCheckTime = sYY + sMM + sDD + m_sCheckDDHHmm.Substring(2, 4) + "00";
        }
        private void EnvFileSave()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\csvout.env";
            sData = "// csvout env\n";
            sData = sData + m_sSavePath + "\n";
            sData = sData + m_sPostFileName + "\n";
            sData = sData + m_sDelimiter + "\n";
            sData = sData + m_sCheckDDHHmm + "\n";
            sData = sData + m_sBaseDate + "\n";
            sData = sData + m_sBaseTime + "\n";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void FildKeyFileLoad()
        {
            string sTitles;
            string sFilfkeys;
            string sFncStrs;
            string sLoadFileName;
            string sData;
            string[] aryLine;

            sTitles = "社員番号,yyyy/mm/dd,hh:mm,出退勤フラグ,固定値";
            m_aryCsvTitleTbl = sTitles.Split(',');
            sFilfkeys = "%L_UID%,%C_Date%,%C_Time%,%L_Mode%,0";
            m_aryFildKeyTbl = sFilfkeys.Split(',');
            sFncStrs = "出勤,退勤,出勤,退勤";
            m_aryFucStrTbl = sFncStrs.Split(',');
            sLoadFileName = m_sEnvPath + "\\csvfild.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_aryCsvTitleTbl = aryLine[1].Split(',');
                m_aryFildKeyTbl = aryLine[2].Split(',');
                m_aryFucStrTbl = aryLine[3].Split(',');
            }
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
        private void InitcmbDelimiter()
        {
            string[] lst = new string[]
            { ",(カンマ)", "\\t(タブ)", " (スペース)"};

            cmbDelimiter.ItemsSource = lst;
            cmbDelimiter.IsEditable = false;
        }
        private void InitCmbDay()
        {
            string[] lst = new string[]
            { "00", "01", "02", "03", "04"
            , "05", "06", "07", "08", "09"
            , "10", "11", "12", "13", "14"
            , "15", "16", "17", "18", "19"
            , "20", "21", "22", "23", "24"
            , "25", "26", "27", "28", "29"
            , "30", "31", "末"
           };

            cmbDay.ItemsSource = lst;
            cmbDay.IsEditable = false;
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
            string[] lst = new string[] { "00", "10", "20", "30", "40", "50", "59" };

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
            int nSelect;
            string sYY, sMM;
            string sDay, sHH, sMin;

            m_sSavePath = txtPath.Text;
            m_sPostFileName = txtPostFileName.Text;
            nSelect = cmbDelimiter.SelectedIndex;
            if (nSelect == 0)
            {
                m_sDelimiter = ",";
            }
            else if (nSelect == 1)
            {
                m_sDelimiter = "\t";
            }
            else if (nSelect == 2)
            {
                m_sDelimiter = " ";
            }
            sDay = cmbDay.Text;
            if (sDay == "末")
            {
                sDay = "32";
            }
            sHH = cmbHour.Text;
            sMin = cmbMinute.Text;
            m_sCheckDDHHmm = sDay + sHH + sMin;
            DateTime dt = DateTime.Now;
            sYY = dt.ToString("yyyy");
            sMM = dt.ToString("MM");
            UpDateCheckTime(sYY, sMM, m_sCheckDDHHmm);
            if (m_sBaseDate == "0")
            {
                InitBaseDate();
            }
        }
        private void InitBaseDate()
        {
            string sChkDD;
            string sYY, sMM, sDD;
            int nYY, nMM, nDD;
            int nLast;

            sChkDD = m_sCheckTime.Substring(6, 2);
            DateTime dt = DateTime.Now;
            sYY = dt.ToString("yyyy");
            sMM = dt.ToString("MM");
            sDD = dt.ToString("dd");

            nYY = m_libCmn.StrToInt(sYY);
            nMM = m_libCmn.StrToInt(sMM);
            if (sChkDD.CompareTo(sDD) > 0)
            {
                // 現在が20で締め日が25日のとき２ヶ月前の開始日時を設定
                if (nMM == 1)
                {
                    nMM = 11;
                    nYY = nYY - 1;
                }
                else if (nMM == 2)
                {
                    nMM = 12;
                    nYY = nYY - 1;
                }
                else
                {
                    nMM = nMM - 2;
                }
            }
            else // 現在が20で締め日が15日のとき前の月の開始日時を設定
            {
                if (nMM == 1)
                {
                    nMM = 12;
                    nYY = nYY - 1;
                }
                else
                {
                    nMM = nMM - 1;
                }
            }
            nLast = DateTime.DaysInMonth(nYY, nMM);
            sDD = m_sCheckDDHHmm.Substring(0, 2);
            nDD = m_libCmn.StrToInt(sDD);
            if (nLast < nDD)
            {
                nDD = nLast;
            }
            sYY = nYY.ToString("D4");
            sMM = nMM.ToString("D2");
            sDD = nDD.ToString("D2");
            m_sBaseDate = sYY+sMM+sDD;
            // 前月分の集計
            GetODBCDataToFile();
            if (m_dsptCheckTime == null)
            {
                m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
                m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 30);
                m_dsptCheckTime.Tick += new EventHandler(TickCheckTimeLoop);
                m_dsptCheckTime.Start();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            EnvFileSave();
            this.Close();
        }
    }
}

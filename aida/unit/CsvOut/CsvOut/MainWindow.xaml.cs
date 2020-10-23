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
        DispatcherTimer m_dsptCheckTime;
        private List<string> m_lstCsvStr;
        private string[] m_aryCsvTitleTbl;
        private string[] m_aryFildKeyTbl;
        private string[] m_aryFucStrTbl;

        private string m_sSavePath;
        private string m_sUnisDBPath;
        private string m_sPostFileName;
        private string m_sDelimiter;
        private string m_sFncStrs;
        private int m_nCheckKind;
        private string m_sIntervalList;
        private string[] m_aryInterval;
        private string m_sCheckTimeList;
        private List<string> m_lstCheckTime;
        private string m_sInterval;
        private string m_sBaseDate;
        private string m_sBaseTime;
        private string m_sLastDate;
        private string m_sLastTime;
        private string m_sCheckTime;
        private Boolean m_bCheckOutIn;

        public MainWindow()
        {
            InitializeComponent();
            m_dsptCheckTime = null;
            m_bCheckOutIn = false;
            m_libCmn = new LibCommon();
            m_sExecPath = InitExePath();
            m_sEnvPath = InitEnvPath();
            string sFileName = m_sEnvPath + "\\csvoutlog.txt";
            if (File.Exists(sFileName))
            {
                App.m_sArgv = "log";
            }
            EnvFileLoad();
            FildKeyFileLoad();
            InitCmbDelimiter();
            InitCmbInterval();
            InitCmbCheckTimes();
            MainWindowODBCInit();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EnvFileSave();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string sMsg;
            if (m_sBaseDate == "0")
            {
                sMsg = "間隔指定か時刻指定などを設定し「適応」ボタンを押してください";
                MessageBox.Show(sMsg);
            }
            else
            {
                CheckLoopExec();
            }
        }
        private void CheckLoopExec()
        {
            SetNextCheckTime(m_sBaseDate, m_sBaseTime);
            m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 30);
            m_dsptCheckTime.Tick += new EventHandler(TickCheckTimeLoop);
            m_dsptCheckTime.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int nSelect;
            int max, idx;
            string sChk;

            txtPath.Text = m_sSavePath;
            txtMDBPath.Text = m_sUnisDBPath;
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
            if (m_nCheckKind == 0)
            {
                rdoInterval.IsChecked = true;
                rdoCheckTime.IsChecked = false;
            }
            else
            {
                rdoInterval.IsChecked = false;
                rdoCheckTime.IsChecked = true;
            }

            nSelect = 0;
            max = m_aryInterval.Length;
            for (idx = 0; idx < max; idx++)
            {
                sChk = GetIntervalString(m_aryInterval[idx]);
                if (m_sInterval == sChk)
                {
                    nSelect = idx;
                }
            }
            cmbInterval.SelectedIndex = nSelect;
            SetCmbCheckTimes(m_sCheckTimeList);

            txtF1.Text = m_aryFucStrTbl[0];
            txtF2.Text = m_aryFucStrTbl[1];
            txtF3.Text = m_aryFucStrTbl[2];
            txtF4.Text = m_aryFucStrTbl[3];
        }
        private void TickCheckTimeLoop(object sender, EventArgs e)
        {
            string sCrtTime;

            if (m_bCheckOutIn == true)
            {
                return;
            }
            DateTime dt = DateTime.Now;
            sCrtTime = dt.ToString("yyyyMMddHHmmss");
            if (m_sCheckTime.CompareTo(sCrtTime) < 0)
            {
                m_bCheckOutIn = true;
                lblMsg.Content = "データ取得中";
                GetODBCDataToFile();
                m_bCheckOutIn = false;
            }
        }
        private void GetODBCDataToFile()
        {
            string sMsg;
            string sCrtDate;
            string sSaveFileName;

            DateTime dt = DateTime.Now;
            App.LogOut(dt.ToString("yyyy/MM/dd HH:mm:ss"));

            sCrtDate = m_sCheckTime.Substring(0, 12);
            App.LogOut("上書き確認中。");
            sSaveFileName = m_sSavePath + "\\" + sCrtDate + m_sPostFileName + ".csv";
            if (File.Exists(sSaveFileName) == true){
                sMsg = sSaveFileName + "が存在します上書きしますか";
                if (MessageBox.Show(sMsg, "Information", MessageBoxButton.YesNo,
                     MessageBoxImage.Information) == MessageBoxResult.No)
                {
                    m_sBaseDate = m_sCheckTime.Substring(0, 8);
                    m_sBaseTime = m_sCheckTime.Substring(8, 6);
                    SetNextCheckTime(m_sBaseDate, m_sBaseTime);
                    return;
                }
            }
            App.LogOut("MDBファイルオープン。");
            if (ODBCOpenUnisDB() == true)
            {
                App.LogOut("SQL実行。");
                ODBCSelecttEnter();
                App.LogOut("MDBファイルクローズ。");
                ODBCCloseUnisDB();
                App.LogOut("ファイル保存中。");
                SaveCsvFile(sSaveFileName);
                App.LogOut("ベース時間更新。");
                m_sBaseDate = m_sCheckTime.Substring(0, 8);
                m_sBaseTime = m_sCheckTime.Substring(8, 6);
                App.LogOut("チェック日時更新。");
                SetNextCheckTime(m_sBaseDate, m_sBaseTime);
            }
        }
        private void SaveCsvFile(string sSaveFileName)
        {
            string sData;
            int idx, max;

            lblMsg.Content = "保存中";
            sData = "";
            /*
            int fldmax, fldidx;
            fldmax = m_aryCsvTitleTbl.Length;
            for(fldidx = 0 ; fldidx < fldmax; fldidx++){
                sData = sData + m_aryCsvTitleTbl[fldidx]+ m_sDelimiter;
            }
            sData = sData + "\n";
            */
            max = m_lstCsvStr.Count;
            if (max == 0)
            {
                lblMsg.Content = "待機中";
                return;
            }
            for (idx = 0; idx < max; idx++)
            {
                sData = sData + m_lstCsvStr[idx];
            }

            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
            lblMsg.Content = "待機中";
        }
        // 次にチェックする時分
        private void SetNextCheckTime(string sBaseDate, string sBaseTime)
        {
            DateTime dt;
            string sYY, sMM, sDD, sHH, sMin, sSS;
            int nYY, nMM, nDD, nHH, nMin, nSS;
            string sAddHH, sAddMin, sHHMin, sChkHH, sChkMin;
            int nAddHH, nAddMin, nChkHH, nChkMin;
            int max, idx, setflag;

            dt = DateTime.Now;
            int nYYMMDD = m_libCmn.StrToInt(sBaseDate);
            int nHHMINSS = m_libCmn.StrToInt(sBaseTime);
            if (nYYMMDD == 0 && nHHMINSS == 0)
            {
                // 最初にこれまでのデータを収集
                dt = DateTime.Now;
                sYY = dt.ToString("yyyy");
                sMM = dt.ToString("MM");
                sDD = dt.ToString("dd");
                sHH = dt.ToString("HH");
                sMin = dt.ToString("mm");
                sSS = "00";
                sBaseDate = sYY + sMM + sDD;
                sBaseTime = sHH + sMin + sSS;
            }

            sYY = sBaseDate.Substring(0, 4);
            sMM = sBaseDate.Substring(4, 2);
            sDD = sBaseDate.Substring(6, 2);
            sHH = sBaseTime.Substring(0, 2);
            sMin = sBaseTime.Substring(2, 2);
            sSS = sBaseTime.Substring(4, 2);

            nYY = m_libCmn.StrToInt(sYY);
            nMM = m_libCmn.StrToInt(sMM);
            nDD = m_libCmn.StrToInt(sDD);
            nHH = m_libCmn.StrToInt(sHH);
            nMin = m_libCmn.StrToInt(sMin);
            nSS = m_libCmn.StrToInt(sSS);
            if (m_nCheckKind == 0)
            {
                sAddHH = m_sInterval.Substring(0,2);
                sAddMin = m_sInterval.Substring(2, 2);
                nAddHH = m_libCmn.StrToInt(sAddHH);
                nAddMin = m_libCmn.StrToInt(sAddMin);
                nHH = nHH + nAddHH;
                nMin = nMin + nAddMin;
                if (60 <= nMin)
                {
                    nMin = nMin - 60;
                    nHH++;
                }
                if (24 <= nHH)
                {
                    nHH = nHH - 24;
                    nDD++;
                }
                dt = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
            }else{
                setflag = 0;
                max = m_lstCheckTime.Count;
                for (idx = 0; idx < max; idx++)
                {
                    sHHMin = m_lstCheckTime[idx];
                    sChkHH = sHHMin.Substring(0,2);
                    sChkMin = sHHMin.Substring(2, 2);
                    nChkHH = m_libCmn.StrToInt(sChkHH);
                    nChkMin = m_libCmn.StrToInt(sChkMin);
                    if(nHH == nChkHH && nMin < nChkMin){
                        setflag = 1;
                        nHH = nChkHH;
                        nMin = nChkMin;
                        dt = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                        break;
                    }
                    else if(nHH < nChkHH){
                        setflag = 1;
                        nHH = nChkHH;
                        nMin = nChkMin;
                        dt = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                        break;
                    }
                }
                if(setflag == 0){
                    nDD++;
                    sHHMin = m_lstCheckTime[0];
                    sChkHH = sHHMin.Substring(0,2);
                    sChkMin = sHHMin.Substring(2, 2);
                    nChkHH = m_libCmn.StrToInt(sChkHH);
                    nChkMin = m_libCmn.StrToInt(sChkMin);
                    nHH = nChkHH;
                    nMin = nChkMin;
                    dt = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                }
            }
            m_sCheckTime = dt.ToString("yyyyMMddHHmmss");
        }
        private void EnvFileLoad()
        {
            string sLoadFileName;
            string sData;
            string[] aryLine;

            m_sSavePath = "c:\\MajorFlow";
            m_sUnisDBPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            m_sUnisDBPath = m_sUnisDBPath + "\\UNIS";
            m_sPostFileName = "major";
            m_sDelimiter = ",";
            m_sFncStrs = "出勤,退勤,外出,戻り";
            m_nCheckKind = 0;
            m_sIntervalList = "05分,01時間,03時間,06時間,12時間";
            m_sCheckTimeList = "0800,0900,1300,1700,1800,2300";
            m_sInterval = "0005";
            m_sBaseDate = "0";
            m_sBaseTime = "0";
            sLoadFileName = m_sEnvPath + "\\csvout.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_sSavePath = aryLine[1];
                m_sUnisDBPath = aryLine[2];
                m_sPostFileName = aryLine[3];
                m_sDelimiter = aryLine[4];
                m_sFncStrs = aryLine[5];
                m_nCheckKind = m_libCmn.StrToInt(aryLine[6]);
                m_sIntervalList = aryLine[7];
                m_sCheckTimeList = aryLine[8];
                m_sInterval = aryLine[9];
                m_sBaseDate = aryLine[10];
                m_sBaseTime = aryLine[11];
            }
            m_aryFucStrTbl = m_sFncStrs.Split(',');
            m_aryInterval = m_sIntervalList.Split(',');
            SortSetLstCheckTime(m_sCheckTimeList);
            m_libCmn.CreatePath(m_sSavePath);
        }
        private void EnvFileSave()
        {
            string sSaveFileName;
            string sData;
            int max, idx;

            max = m_aryInterval.Length;
            m_sIntervalList = m_aryInterval[0];
            for (idx = 1; idx < max; idx++)
            {
                m_sIntervalList = m_sIntervalList + "," + m_aryInterval[idx];
            }
            max = m_aryFucStrTbl.Length;
            m_sFncStrs = m_aryFucStrTbl[0];
            for (idx = 1; idx < max; idx++)
            {
                m_sFncStrs = m_sFncStrs + "," + m_aryFucStrTbl[idx];
            }
            sSaveFileName = m_sEnvPath + "\\csvout.env";
            sData = "// csvout env\n";
            sData = sData + m_sSavePath + "\n";
            sData = sData + m_sUnisDBPath + "\n";
            sData = sData + m_sPostFileName + "\n";
            sData = sData + m_sDelimiter + "\n";
            sData = sData + m_sFncStrs + "\n";
            sData = sData + m_nCheckKind + "\n"; ;
            sData = sData + m_sIntervalList + "\n"; ;
            sData = sData + m_sCheckTimeList + "\n"; ;
            sData = sData + m_sInterval + "\n"; ;
            sData = sData + m_sBaseDate + "\n";
            sData = sData + m_sBaseTime + "\n";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
            sSaveFileName = "c:\\csvout.env";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void FildKeyFileLoad()
        {
            string sTitles;
            string sFilfkeys;
            string sLoadFileName;
            string sData;
            string[] aryLine;

            sTitles = "社員番号,西暦/月/日,時:分,出退勤フラグ,固定値";
            m_aryCsvTitleTbl = sTitles.Split(',');
            sFilfkeys = "%C_OCODE%,%C_Date%,%C_Time%,%L_Mode%,0";
            m_aryFildKeyTbl = sFilfkeys.Split(',');
            sLoadFileName = m_sEnvPath + "\\csvfield.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_aryCsvTitleTbl = aryLine[1].Split(',');
                m_aryFildKeyTbl = aryLine[2].Split(',');
            }
        }
        private void FildKeyFileSave()
        {
            int max, idx;
            string sData;
            string sSaveFileName;

            sData = "// csvfield \n";
            max = m_aryCsvTitleTbl.Length;
            sData = sData + m_aryCsvTitleTbl[0];
            for (idx = 1; idx < max; idx++)
            {
                sData = sData + "," + m_aryCsvTitleTbl[idx];
            }
            sData = sData + "\n";
            max = m_aryFildKeyTbl.Length;
            sData = sData + m_aryFildKeyTbl[0];
            for (idx = 1; idx < max; idx++)
            {
                sData = sData + "," + m_aryFildKeyTbl[idx];
            }
            sData = sData + "\n";

            sSaveFileName = m_sEnvPath + "\\csvfield.env";
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
            sEnvPath = sEnvPath + "\\csvout";
            m_libCmn.CreatePath(sEnvPath);
            return (sEnvPath);
        }
        private void InitCmbDelimiter()
        {
            string[] lst = new string[]
            { ",(カンマ)", "\\t(タブ)", " (スペース)"};

            cmbDelimiter.ItemsSource = lst;
            cmbDelimiter.IsEditable = false;
        }
        private void InitCmbInterval()
        {
            cmbInterval.ItemsSource = m_aryInterval;
            cmbInterval.IsEditable = false;
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
        private void btnMDBSlct_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = txtMDBPath.Text;
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtMDBPath.Text = fbd.SelectedPath;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            int nSelect;

            m_sSavePath = txtPath.Text;
            m_sUnisDBPath = txtMDBPath.Text;
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

            if(rdoCheckTime.IsChecked == true)
            {
                m_nCheckKind = 1;
            }
            else
            {
                m_nCheckKind = 0;
            }

            nSelect = cmbInterval.SelectedIndex;
            m_sInterval = GetIntervalString(m_aryInterval[nSelect]);

            m_sCheckTimeList = GetcmbCheckTimes();
            SortSetLstCheckTime(m_sCheckTimeList);

            m_aryFucStrTbl[0] = txtF1.Text;
            m_aryFucStrTbl[1] = txtF2.Text;
            m_aryFucStrTbl[2] = txtF3.Text;
            m_aryFucStrTbl[3] = txtF4.Text;
            if (m_sBaseDate == "0")
            {
                CheckLoopExec();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private string GetIntervalString(string hhmin)
        {
            string[] ary;
            int hh, min;

            hhmin = hhmin.Replace("時間", ":");
            hhmin = hhmin.Replace("分", "");
            hhmin = hhmin
                          .Replace("０", "0")
                          .Replace("１", "1")
                          .Replace("２", "2")
                          .Replace("３", "3")
                          .Replace("４", "4")
                          .Replace("５", "5")
                          .Replace("６", "6")
                          .Replace("７", "7")
                          .Replace("８", "8")
                          .Replace("９", "9");
            ary = hhmin.Split(':');
            if (ary.Length == 1)
            {
                hh = 0;
                min = m_libCmn.StrToInt(ary[0]);
            }
            else if (ary.Length == 2)
            {
                hh = m_libCmn.StrToInt(ary[0]);
                min = m_libCmn.StrToInt(ary[1]);
            }
            else
            {
                hh = 0;
                min = 0;
            }
            hhmin = hh.ToString("D2") + min.ToString("D2");
            return (hhmin);
        }
        private void InitCmbCheckTimes()
        {
            List<string> lstHH = new List<string>();
            List<string> lstMM = new List<string>();
            int idx;

            lstHH.Add("");
            for (idx = 0; idx < 24; idx++)
            {
                lstHH.Add(idx.ToString("D2"));
            }
            lstMM.Add("");
            for (idx = 0; idx < 60; idx++)
            {
                lstMM.Add(idx.ToString("D2"));
            }
            cmbCheckHH1.ItemsSource = lstHH;
            cmbCheckHH2.ItemsSource = lstHH;
            cmbCheckHH3.ItemsSource = lstHH;
            cmbCheckHH4.ItemsSource = lstHH;
            cmbCheckHH5.ItemsSource = lstHH;
            cmbCheckHH6.ItemsSource = lstHH;
            cmbCheckMM1.ItemsSource = lstMM;
            cmbCheckMM2.ItemsSource = lstMM;
            cmbCheckMM3.ItemsSource = lstMM;
            cmbCheckMM4.ItemsSource = lstMM;
            cmbCheckMM5.ItemsSource = lstMM;
            cmbCheckMM6.ItemsSource = lstMM;

        }
        private void SetCmbCheckTimes(string sList)
        {
            string[] ary;
            ary = sList.Split(',');
            if (ary[0] == "" || ary[0].Length != 4)
            {
                cmbCheckHH1.SelectedValue = "";
                cmbCheckMM1.SelectedValue = "";
            }
            else
            {
                cmbCheckHH1.SelectedValue = ary[0].Substring(0, 2);
                cmbCheckMM1.SelectedValue = ary[0].Substring(2, 2);
            }
            if (ary[1] == "" || ary[1].Length != 4)
            {
                cmbCheckHH2.SelectedValue = "";
                cmbCheckMM2.SelectedValue = "";
            }
            else
            {
                cmbCheckHH2.SelectedValue = ary[1].Substring(0, 2);
                cmbCheckMM2.SelectedValue = ary[1].Substring(2, 2);
            }
            if (ary[2] == "" || ary[2].Length != 4)
            {
                cmbCheckHH3.SelectedValue = "";
                cmbCheckMM3.SelectedValue = "";
            }
            else
            {
                cmbCheckHH3.SelectedValue = ary[2].Substring(0, 2);
                cmbCheckMM3.SelectedValue = ary[2].Substring(2, 2);
            }
            if (ary[3] == "" || ary[3].Length != 4)
            {
                cmbCheckHH4.SelectedValue = "";
                cmbCheckMM4.SelectedValue = "";
            }
            else
            {
                cmbCheckHH4.SelectedValue = ary[3].Substring(0, 2);
                cmbCheckMM4.SelectedValue = ary[3].Substring(2, 2);
            }
            if (ary[4] == "" || ary[4].Length != 4)
            {
                cmbCheckHH5.SelectedValue = "";
                cmbCheckMM5.SelectedValue = "";
            }
            else
            {
                cmbCheckHH5.SelectedValue = ary[4].Substring(0, 2);
                cmbCheckMM5.SelectedValue = ary[4].Substring(2, 2);
            }
            if (ary[5] == "" || ary[5].Length != 4)
            {
                cmbCheckHH6.SelectedValue = "";
                cmbCheckMM6.SelectedValue = "";
            }
            else
            {
                cmbCheckHH6.SelectedValue = ary[5].Substring(0, 2);
                cmbCheckMM6.SelectedValue = ary[5].Substring(2, 2);
            }
        }
        private string GetcmbCheckTimes()
        {
            string sHH, sMM;
            string sList;
            sHH = cmbCheckHH1.SelectedValue.ToString();
            sMM = cmbCheckMM1.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = "";
            }
            else
            {
                sList = sHH + sMM;
            }
            sHH = cmbCheckHH2.SelectedValue.ToString();
            sMM = cmbCheckMM2.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = sList+ ",";
            }
            else
            {
                sList = sList + "," + sHH + sMM;
            }
            sHH = cmbCheckHH3.SelectedValue.ToString();
            sMM = cmbCheckMM3.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = sList + ",";
            }
            else
            {
                sList = sList + "," + sHH + sMM;
            }
            sHH = cmbCheckHH4.SelectedValue.ToString();
            sMM = cmbCheckMM4.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = sList + ",";
            }
            else
            {
                sList = sList + "," + sHH + sMM;
            }
            sHH = cmbCheckHH5.SelectedValue.ToString();
            sMM = cmbCheckMM5.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = sList + ",";
            }
            else
            {
                sList = sList + "," + sHH + sMM;
            }
            sHH = cmbCheckHH6.SelectedValue.ToString();
            sMM = cmbCheckMM6.SelectedValue.ToString();
            if (sHH == "" || sMM == "")
            {
                sList = sList + ",";
            }
            else
            {
                sList = sList + "," + sHH + sMM;
            }
            return (sList);
        }
        private void SortSetLstCheckTime(string sList)
        {
            int max, idx, setidx;
            string sHHMM;
            m_lstCheckTime = new List<string>();
            string[] ary = sList.Split(',');
            max = ary.Length;
            while(true){
                sHHMM = "";
                setidx = -1;
                for(idx = 0; idx < max; idx++){
                    if(ary[idx] != ""){
                        if(sHHMM == ""){
                            sHHMM = ary[idx];
                            setidx = idx;
                        }else if(sHHMM.CompareTo(ary[idx]) > 0){
                            sHHMM = ary[idx];
                            setidx = idx;
                        }
                    }
                }
                if(sHHMM == ""){
                    break;
                }
                ary[setidx] = "";
                m_lstCheckTime.Add(sHHMM);
            }
        }

        private void cmbCheckHH1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbCheckHH1.SelectedIndex == 0){
                cmbCheckMM1.SelectedValue = "";
            }
            else
            {
                cmbCheckMM1.SelectedValue = "00";
            }
        }

        private void cmbCheckHH2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCheckHH2.SelectedIndex == 0)
            {
                cmbCheckMM2.SelectedValue = "";
            }
            else
            {
                cmbCheckMM2.SelectedValue = "00";
            }
        }

        private void cmbCheckHH3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCheckHH3.SelectedIndex == 0)
            {
                cmbCheckMM3.SelectedValue = "";
            }
            else
            {
                cmbCheckMM3.SelectedValue = "00";
            }
        }

        private void cmbCheckHH4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCheckHH4.SelectedIndex == 0)
            {
                cmbCheckMM4.SelectedValue = "";
            }
            else
            {
                cmbCheckMM4.SelectedValue = "00";
            }
        }

        private void cmbCheckHH5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCheckHH5.SelectedIndex == 0)
            {
                cmbCheckMM5.SelectedValue = "";
            }
            else
            {
                cmbCheckMM5.SelectedValue = "00";
            }
        }

        private void cmbCheckHH6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCheckHH6.SelectedIndex == 0)
            {
                cmbCheckMM6.SelectedValue = "";
            }
            else
            {
                cmbCheckMM6.SelectedValue = "00";
            }
        }
    }
}

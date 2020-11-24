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
        private string m_sProgName;
        private string m_sDefSavePath;
        private string m_sDefPreFileName;
        private string m_sDefDateFileName;
        private string m_sDefPostFileName;
        private string m_sEnvPath;
        private LibCommon m_libCmn;
        DispatcherTimer m_dsptCheckTime;
        private List<string> m_lstCsvStr;
        private List<string> m_lstErrorStr;
        private string[] m_aryCsvTitleTbl;
        private string[] m_aryFildKeyTbl;
        private string[] m_aryFucStrTbl;

        private string m_sBackupUnisDBPath;
        private string m_sSavePath;
        private string m_sUnisDBPath;
        private string m_sPreFileName;
        private string m_sDateFileName;
        private string m_sPostFileName;
        private string m_sDelimiter;
        private int m_nOutputId;
        private int m_nOutputType;
        private string m_sFncStrs;
        private int m_nCheckKind;
        private string m_sIntervalList;
        private string[] m_aryInterval;
        private string m_sCheckTimeList;
        private List<string> m_lstCheckTime;
        private string m_sInterval;
        private string m_sTempDate;
        private string m_sBaseDate;
        private string m_sBaseTime;
        private string m_sLastDate;
        private string m_sLastTime;
        private string m_sCheckTime;
        private DateTime m_dtCheckTime;
        private Boolean m_bCheckOutIn;
        private string m_sSaveFileName;
        private int m_nIncNo;

        public MainWindow()
        {
            string[] aryLine;
            string sFileName;
            string sData;

            InitializeComponent();
            try
            {
                Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                aryLine = asm.FullName.Split(',');
                if (aryLine[0] == "CsvEx")
                {
                    m_sProgName = "csvex";
                    this.Title = "CsvEx";
                }
                else
                {
                    m_sProgName = "csvout";
                    this.Title = "CsvOut";
                }
                App.m_sProgName = m_sProgName;
                m_nIncNo = 1;
                m_dsptCheckTime = null;
                m_bCheckOutIn = false;
                m_libCmn = new LibCommon();
                m_sBackupUnisDBPath = "";
                m_sExecPath = InitExePath();
                m_sEnvPath = InitEnvPath();
                sFileName = m_sExecPath + "\\" + m_sProgName + "def.txt";
                if (File.Exists(sFileName))
                {
                    sData = m_libCmn.LoadFileSJIS(sFileName);
                    aryLine = sData.Split('\n');
                    m_sDefSavePath = aryLine[1];
                    m_sDefPreFileName = aryLine[2];
                    m_sDefDateFileName = aryLine[3];
                    m_sDefPostFileName = aryLine[4];
                }
                else
                {
                    m_sDefSavePath = m_sEnvPath;
                    if(m_sProgName == "csvout"){
                        m_sDefPreFileName = "";
                        m_sDefDateFileName = "yyyyMMddHHmm";
                        m_sDefPostFileName = "major";
                    }else{
                        m_sDefPreFileName = "KINTAIDAKOKU_";
                        m_sDefDateFileName = "yyyyMMdd";
                        m_sDefPostFileName = "00";
                    }
                }
                sFileName = m_sEnvPath + "\\" + m_sProgName + "log.txt";
                if (File.Exists(sFileName))
                {
                    App.m_sArgv = "log";
                }

                EnvFileLoad();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("MainWindow");
                App.LogOut(ex.ToString());
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int nSelect;
            int max, idx;
            string sChk;

            try
            {
                InitCmbDelimiter();
                InitCmbOutputID();
                InitCmbOutputType();
                InitCmbInterval();
                InitCmbCheckTimes();
                FildKeyFileLoad();
                MainWindowODBCInit();

                txtPath.Text = m_sSavePath;
                txtMDBPath.Text = m_sUnisDBPath;

                txtPreFileName.Text = m_sPreFileName;
                txtDateFileName.Text = m_sDateFileName;
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
                else
                {
                    nSelect = 3;
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("Window_Loaded");
                App.LogOut(ex.ToString());
            }
            /*
            m_sBaseDate = "20170101";
            m_sBaseTime = "010101";
            DateTime dt = DateTime.Now;
            m_sCheckTime = dt.ToString("yyyyMMddHHmmss");
            GetODBCDataToFile();
            */
            //testDataLoad();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                EnvFileSave();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("Window_Closed Exec");
                App.LogOut(ex.ToString());
                return;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string sYY, sMM, sDD, sHH, sMin, sSS;
            string sBaseDate, sBaseTime;
            string sMsg;

            try
            {
                if (m_sBaseDate == "0")
                {
                    sMsg = "間隔指定か時刻指定などを設定し「適用」ボタンを押してください";
                    MessageBox.Show(sMsg);
                }
                else
                {
                    DateTime dt = DateTime.Now;
                    sYY = dt.ToString("yyyy");
                    sMM = dt.ToString("MM");
                    sDD = dt.ToString("dd");
                    sHH = dt.ToString("HH");
                    sMin = dt.ToString("mm");
                    sSS = "00";
                    sBaseDate = sYY + sMM + sDD;
                    sBaseTime = sHH + sMin + sSS;
                    SetNextCheckTime(sBaseDate, sBaseTime);
                    CheckLoopExec();
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("Window_ContentRendered");
                App.LogOut(ex.ToString());
            }
        }
        private void CheckLoopExec()
        {
            try
            {
                m_dsptCheckTime = new DispatcherTimer(DispatcherPriority.Normal);
                m_dsptCheckTime.Interval = TimeSpan.FromMilliseconds(1000 * 30);
                m_dsptCheckTime.Tick += new EventHandler(TickCheckTimeLoop);
                m_dsptCheckTime.Start();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("CheckLoopExec");
                App.LogOut(ex.ToString());
            }
        }
        private void testDataLoad()
        {
            
            try
            {
                if (ODBCOpenUnisDB() == true)
                {
                    App.LogOut("SQL実行。");
                    ODBCSelecttEnterAll();
                    App.LogOut("MDBファイルクローズ。");
                    ODBCCloseUnisDB();
                }
            }
            catch (Exception ex)
            {
                 App.ErrorLogAppend("testDataLoad");
                 App.LogOut(ex.ToString());
            }
        }
        private void TickCheckTimeLoop(object sender, EventArgs e)
        {
            string sCrtTime;

            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("TickCheckTimeLoop");
                App.LogOut(ex.ToString());
            }
        }
        private void GetODBCDataToFile()
        {
            string sMsg;
            string sSaveFileName;
            string sSafix;
            Boolean ret;

            try
            {
                DateTime dt = DateTime.Now;
                App.LogOut(dt.ToString("yyyy/MM/dd HH:mm:ss"));
                if (m_sTempDate == "")
                {
                    m_sTempDate = m_sBaseDate;
                }
                if (m_sTempDate != m_sBaseDate)
                {
                    m_sTempDate = m_sBaseDate;
                    m_nIncNo = 1;
                }
                sSafix = ".csv";
                if (m_sTempDate == "")
                {
                    sSafix = ".txt";
                }
                else
                {
                    sSafix = ".csv";
                }
                if (m_sPostFileName == "00")
                {
                    sSaveFileName = m_sSavePath + "\\" + m_sPreFileName+ dt.ToString(m_sDateFileName) + m_nIncNo.ToString("00") + sSafix;
                }
                else
                {
                    sSaveFileName = m_sSavePath + "\\" + m_sPreFileName + dt.ToString(m_sDateFileName) + m_sPostFileName + sSafix;
                }
                m_nIncNo++;
                if (m_nOutputType == 0)
                {
                    App.LogOut("上書き確認中。");
                    if (File.Exists(sSaveFileName) == true)
                    {
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
                }
                App.LogOut("MDBファイルオープン。");
                if (ODBCOpenUnisDB() == true)
                {
                    App.LogOut("SQL実行。");
                    ODBCSelecttEnter();
                    App.LogOut("MDBファイルクローズ。");
                    ODBCCloseUnisDB();
                    App.LogOut("ファイル保存中。");
                    if (m_nOutputType == 0)
                    {
                        ret = SaveCsvFile(sSaveFileName);
                    }
                    else
                    {
                        ret = SaveCsvFile(m_sSaveFileName);
                    }
                    if (ret == true)
                    {
                        App.LogOut("ベース時間更新。");
                        m_sBaseDate = m_sCheckTime.Substring(0, 8);
                        m_sBaseTime = m_sCheckTime.Substring(8, 6);
                        App.LogOut("チェック日時更新。");
                        SetNextCheckTime(m_sBaseDate, m_sBaseTime);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("GetODBCDataToFile");
                App.LogOut(ex.ToString());
            }
        }
        private Boolean SaveCsvFile(string sSaveFileName)
        {
            Boolean ret;
            string sMsg;
            int idx, max;

            try
            {
                lblMsg.Content = "保存中";
                if (m_sTempDate == "")
                {
                    ret = SaveFixsFildFile(sSaveFileName);
                }
                else
                {
                    ret = SaveCsvFildFile(sSaveFileName);
                }
                if (ret == true)
                {
                    max = m_lstCsvStr.Count;
                    sMsg = m_sCheckTime.Substring(0, 4) + "/" + m_sCheckTime.Substring(4, 2);
                    sMsg = sMsg + "/" + m_sCheckTime.Substring(6, 2) + " " + m_sCheckTime.Substring(8, 2);
                    sMsg = sMsg + ":" + m_sCheckTime.Substring(10, 2) + ":" + m_sCheckTime.Substring(12, 2) + " " + max + "件";
                    App.OutLogAppend(sMsg);
                }
                max = m_lstErrorStr.Count;
                if (max != 0)
                {
                    for (idx = 0; idx < max; idx++)
                    {
                        App.ErrorDataAppend(m_lstErrorStr[idx]);
                    }
                }

                lblMsg.Content = "待機中";
                return (ret);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("SaveCsvFile");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        private Boolean SaveCsvFildFile(string sSaveFileName)
        {
            string sData;
            int idx, max;
            int fldmax, fldidx;
            Boolean ret;

            sData = "";
            ret = true;
            try
            {
                max = m_lstCsvStr.Count;
                if (max != 0)
                {
                    for (idx = 0; idx < max; idx++)
                    {
                        m_aryCsvTitleTbl = m_lstCsvStr[idx].Split(',');
                        fldmax = m_aryCsvTitleTbl.Length;
                        for (fldidx = 0; fldidx < fldmax; fldidx++)
                        {
                            sData = sData + m_aryCsvTitleTbl[fldidx] + m_sDelimiter;
                        }
                        sData = sData + "\r\n";
                    }
                }

                if (m_nOutputType == 0)
                {
                    ret = m_libCmn.SaveFileSJIS(sSaveFileName, sData);
                }
                else
                {
                    ret = m_libCmn.AppendSaveFileSJIS(sSaveFileName, sData);
                }
                return (ret);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("SaveCsvFildFile");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        private Boolean SaveFixsFildFile(string sSaveFileName)
        {
            string sData;
            int idx, max;
            Boolean ret;
            string[] ary;
            string sOne;

            sData = "";
            ret = true;
            try
            {
                max = m_lstCsvStr.Count;
                if (max != 0)
                {
                    for (idx = 0; idx < max; idx++)
                    {
                        ary = m_lstCsvStr[idx].Split(',');
                        sOne = "  "+ary[1]+ary[2].Substring(0,4)+"  "+ary[4]+String.Format("{0,-10}", ary[5]);
                        sOne = sOne + "  " + ary[7] + "\r\n";
                        sData = sData + sOne;
                    }
                    if (m_nOutputType == 0)
                    {
                        ret = m_libCmn.SaveFileSJIS(sSaveFileName, sData);
                    }
                    else
                    {
                        ret = m_libCmn.AppendSaveFileSJIS(sSaveFileName, sData);
                    }
                }
                return (ret);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("SaveFixsFildFile");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        // 次にチェックする時分
        private void SetNextCheckTime(string sBaseDate, string sBaseTime)
        {
            int nCrtYYMMDD, nCrtHHMINSS;
            int nYYMMDD, nHHMINSS;
            string sYY, sMM, sDD, sHH, sMin, sSS;
            int nYY, nMM, nDD, nHH, nMin, nSS;
            string sAddHH, sAddMin, sHHMin, sChkHH, sChkMin;
            int nAddHH, nAddMin, nChkHH, nChkMin;
            int max, idx, setflag;

            try
            {
                m_dtCheckTime = DateTime.Now;
                nYYMMDD = m_libCmn.StrToInt(sBaseDate);
                nHHMINSS = m_libCmn.StrToInt(sBaseTime);
                if (nYYMMDD == 0 && nHHMINSS == 0)
                {
                    // 最初にこれまでのデータを収集
                    m_dtCheckTime = DateTime.Now;
                    sYY = m_dtCheckTime.ToString("yyyy");
                    sMM = m_dtCheckTime.ToString("MM");
                    sDD = m_dtCheckTime.ToString("dd");
                    sHH = m_dtCheckTime.ToString("HH");
                    sMin = m_dtCheckTime.ToString("mm");
                    sSS = "00";
                    sBaseDate = sYY + sMM + sDD;
                    sBaseTime = sHH + sMin + sSS;
                }
                else
                {
                    nCrtYYMMDD = m_libCmn.StrToInt(m_dtCheckTime.ToString("yyyyMMdd"));
                    nCrtHHMINSS = m_libCmn.StrToInt(m_dtCheckTime.ToString("HHmmss"));
                    if (nYYMMDD < nCrtYYMMDD)
                    {
                        sBaseDate = m_dtCheckTime.ToString("yyyyMMdd");
                        sBaseTime = m_dtCheckTime.ToString("HHmmss");
                    }
                    else if (nYYMMDD == nCrtYYMMDD && nHHMINSS < nCrtHHMINSS)
                    {
                        sBaseDate = m_dtCheckTime.ToString("yyyyMMdd");
                        sBaseTime = m_dtCheckTime.ToString("HHmmss");
                    }
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
                        try
                        {
                            m_dtCheckTime = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                            m_dtCheckTime = m_dtCheckTime.AddDays(1);
                        }
                        catch (Exception ex1)
                        {
                            m_dtCheckTime = DateTime.Now;
                            App.ErrorLogAppend("time day add" + nYY + "/" + nMM + "/" + nDD);
                            App.LogOut(ex1.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            m_dtCheckTime = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                        }
                        catch (Exception ex2)
                        {
                            m_dtCheckTime = DateTime.Now;
                            App.ErrorLogAppend("time day add" + nYY + "/" + nMM + "/" + nDD);
                            App.LogOut(ex2.ToString());
                        }
                    }
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
                            try
                            {
                                m_dtCheckTime = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                            }
                            catch (Exception ex3)
                            {
                                m_dtCheckTime = DateTime.Now;
                                App.ErrorLogAppend("time day add" + nYY + "/" + nMM + "/" + nDD);
                                App.LogOut(ex3.ToString());
                            }
                            break;
                        }
                        else if(nHH < nChkHH){
                            setflag = 1;
                            nHH = nChkHH;
                            nMin = nChkMin;
                            try
                            {
                                m_dtCheckTime = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                            }
                            catch (Exception ex4)
                            {
                                m_dtCheckTime = DateTime.Now;
                                App.ErrorLogAppend("time day add" + nYY + "/" + nMM + "/" + nDD);
                                App.LogOut(ex4.ToString());
                            }
                            break;
                        }
                    }
                    if(setflag == 0){
                        // nDD++;
                        sHHMin = m_lstCheckTime[0];
                        sChkHH = sHHMin.Substring(0,2);
                        sChkMin = sHHMin.Substring(2, 2);
                        nChkHH = m_libCmn.StrToInt(sChkHH);
                        nChkMin = m_libCmn.StrToInt(sChkMin);
                        nHH = nChkHH;
                        nMin = nChkMin;
                        try
                        {
                            m_dtCheckTime = new DateTime(nYY, nMM, nDD, nHH, nMin, 0);
                            m_dtCheckTime = m_dtCheckTime.AddDays(1);
                        }
                        catch (Exception ex5)
                        {
                            m_dtCheckTime = DateTime.Now;
                            App.ErrorLogAppend("time day add" + nYY + "/" + nMM + "/" + nDD);
                            App.LogOut(ex5.ToString());
                        }
                    }
                }
                m_sCheckTime = m_dtCheckTime.ToString("yyyyMMddHHmmss");
                EnvFileSave();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("SetNextCheckTime");
                App.LogOut(ex.ToString());
            }
        }
        private void EnvFileLoad()
        {
            string sLoadFileName;
            string sData;
            string[] aryLine;

            try
            {
                m_sSavePath = m_sDefSavePath;
                m_sUnisDBPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                m_sUnisDBPath = m_sUnisDBPath + "\\UNIS";
                m_sPostFileName = m_sDefPostFileName;
                m_sDelimiter = ",";
                m_nOutputId = 0;
                m_nOutputType = 0;
                if (m_sPostFileName == "csvex")
                {
                    m_sFncStrs = "01,02,00,00";
                }
                else
                {
                    m_sFncStrs = "出勤,退勤,外出,戻り";
                }
                m_nCheckKind = 0;
                m_sIntervalList = "05分,01時間,03時間,06時間,12時間";
                m_sCheckTimeList = "0800,0900,1300,1700,1800,2300";
                m_sInterval = "0005";
                m_sBaseDate = "0";
                m_sBaseTime = "0";
                m_nOutputId = 1;
                m_nOutputType = 0;
                m_sPreFileName = m_sDefPreFileName;
                m_sDateFileName = m_sDefDateFileName;
                sLoadFileName = m_sEnvPath + "\\" + m_sProgName + ".env";
                sData = m_libCmn.LoadFileSJIS(sLoadFileName);
                if (sData != "")
                {
                    sData = sData.Replace("\r\n", "\n");
                    aryLine = sData.Split('\n');
                    if (12 <= aryLine.Length)
                    {
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
                        if(14 <= aryLine.Length){
                            m_nOutputId = m_libCmn.StrToInt(aryLine[12]);
                            m_nOutputType = m_libCmn.StrToInt(aryLine[13]);

                            m_sSaveFileName = null;
                            if (16 <= aryLine.Length)
                            {// 新しいフォーマット
                                m_sPreFileName = aryLine[14];
                                m_sDateFileName = aryLine[15];
                            }
                            else
                            {
                                if (m_sPostFileName == "csvex")
                                {
                                    m_sPostFileName = "00";
                                    m_sDelimiter = "";
                                    m_sSaveFileName = m_sSavePath + "\\KINTAIDAKOKU.txt";
                                }
                            }
                        }
                    }
                }
                m_aryFucStrTbl = m_sFncStrs.Split(',');
                m_aryInterval = m_sIntervalList.Split(',');
                SortSetLstCheckTime(m_sCheckTimeList);
                m_libCmn.CreatePath(m_sSavePath);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("EnvFileLoad");
                App.LogOut(ex.ToString());
            }
        }
        private void EnvFileSave()
        {
            string sSaveFileName;
            string sData;
            int max, idx;

            try
            {
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
                sSaveFileName = m_sEnvPath + "\\" + m_sProgName + ".env";
                sData = "//,1000,"+m_sProgName+" env\r\n";
                sData = sData + m_sSavePath + "\r\n";
                sData = sData + m_sUnisDBPath + "\r\n";
                sData = sData + m_sPostFileName + "\r\n";
                sData = sData + m_sDelimiter + "\r\n";
                sData = sData + m_sFncStrs + "\r\n";
                sData = sData + m_nCheckKind + "\r\n"; ;
                sData = sData + m_sIntervalList + "\r\n"; ;
                sData = sData + m_sCheckTimeList + "\r\n"; ;
                sData = sData + m_sInterval + "\r\n"; ;
                sData = sData + m_sBaseDate + "\r\n";
                sData = sData + m_sBaseTime + "\r\n";
                sData = sData + m_nOutputId + "\r\n";
                sData = sData + m_nOutputType + "\r\n";
                sData = sData + m_sPreFileName + "\r\n";
                sData = sData + m_sDateFileName + "\r\n";

                m_libCmn.SaveFileSJIS(sSaveFileName, sData);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("EnvFileSave");
                App.LogOut(ex.ToString());
            }
        }
        private void FildKeyFileLoad()
        {
            string sTitles;
            string sFilfkeys;
            string sFildKeyFileName;
            string sData;
            string[] aryLine;

            try
            {
                if (m_sPostFileName == "csvex")
                {
                    if (m_nOutputId == 0)
                    {
                        sTitles = "データ区分,打刻日,打刻時間,シフトコード,出退勤フラグ,ユーザID,例外コード,ターミナルNO";
                        sFilfkeys = "  ,%C_Date%,%C_Time%,  ,%L_Mode%,%L_UID%,  ,%L_TID%";
                    }
                    else
                    {
                        sTitles = "データ区分,打刻日,打刻時間,シフトコード,出退勤フラグ,社員番号,例外コード,ターミナルNO";
                        sFilfkeys = "  ,%C_Date%,%C_Time%,  ,%L_Mode%,%C_OCODE%,  ,%L_TID%";
                    }
                }
                else
                {
                    sTitles = "社員番号,西暦/月/日,時:分,出退勤フラグ,固定値";
                    sFilfkeys = "%C_OCODE%,%C_Date%,%C_Time%,%L_Mode%,0";
                }

                sFildKeyFileName = m_sEnvPath + "\\csvfield.env";
                sData = m_libCmn.LoadFileSJIS(sFildKeyFileName);
                if (sData != "")
                {
                    sData = sData.Replace("\r\n", "\n");
                    aryLine = sData.Split('\n');
                    sTitles = aryLine[1];
                    sFilfkeys = aryLine[2];
                }
                m_aryCsvTitleTbl = sTitles.Split(',');
                m_aryFildKeyTbl = sFilfkeys.Split(',');
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("FildKeyFileLoad");
                App.LogOut(ex.ToString());
            }
        }
        private void InitCmbDelimiter()
        {
            try
            {
                string[] lst = new string[] { 
                    ",(カンマ)CSV形式", "\\t(タブ)CSV形式", " (スペース)CSV形式", "スペース固定長"
                };
                cmbDelimiter.ItemsSource = lst;
                cmbDelimiter.IsEditable = false;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitCmbDelimiter");
                App.LogOut(ex.ToString());
            }
        }
        private void InitCmbOutputID()
        {
            try
            {
                string[] lst = new string[] 
                { "ユーザーID", "社員ID" };

                cmbOutputID.ItemsSource = lst;
                cmbOutputID.IsEditable = false;
                cmbOutputID.SelectedIndex = m_nOutputId;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitCmbOutputID");
                App.LogOut(ex.ToString());
            }
        }
        private void InitCmbOutputType()
        {
            try
            {
                string[] lst = new string[]
                { "個別ファイル出力", "単一ファイル出力" };

                cmbOutputType.ItemsSource = lst;
                cmbOutputType.IsEditable = false;
                cmbOutputType.SelectedIndex = m_nOutputType;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitCmbOutputType");
                App.LogOut(ex.ToString());
            }
        }
        private void InitCmbInterval()
        {
            try
            {
                cmbInterval.ItemsSource = m_aryInterval;
                cmbInterval.IsEditable = false;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitCmbInterval");
                App.LogOut(ex.ToString());
            }
        }
        private void btnSlctClick()
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.SelectedPath = txtPath.Text;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txtPath.Text = fbd.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("btnSlctClick");
                App.LogOut(ex.ToString());
            }
        }
        private void btnMDBSlctClick()
        {
            string sMsg;
            string sFileName;

            try
            {
                m_sBackupUnisDBPath = txtMDBPath.Text;
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.SelectedPath = txtMDBPath.Text;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    sFileName = fbd.SelectedPath + "\\unis.mdb";
                    if (File.Exists(sFileName) == false)
                    {
                        sMsg = fbd.SelectedPath + "にUNISDBファイルが見つかりません";
                        MessageBox.Show(sMsg);
                        if (m_sBackupUnisDBPath != "")
                        {
                            txtMDBPath.Text = m_sBackupUnisDBPath;
                        }
                        return;
                    }
                    txtMDBPath.Text = fbd.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("btnMDBSlctClick");
                App.LogOut(ex.ToString());
            }
        }

        private void btnSetClick()
        {
            string sMsg;
            string sy, sM, sd, sH, sm, ss;
            int nSelect;

            try
            {
                if (System.IO.Directory.Exists(txtPath.Text) == false)
                {
                    sMsg = "CSV保存場所「"+txtPath.Text+"」が見つかりません";
                    MessageBox.Show(sMsg);
                    return;
                }
                m_sSavePath = txtPath.Text;
                m_sUnisDBPath = txtMDBPath.Text;

                MainWindowODBCInit();
                if (System.IO.File.Exists(m_sUnisDBFile) == false)
                {
                    sMsg = "データベースファイル「" + m_sUnisDBFile + "」が見つかりません";
                    MessageBox.Show(sMsg);
                    return;
                }

                m_sPreFileName = txtPreFileName.Text;
                m_sDateFileName = txtDateFileName.Text;
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
                else
                {
                    m_sDelimiter = "";
                }

                nSelect = cmbOutputID.SelectedIndex;
                if (nSelect == 0)
                {
                    m_nOutputId = 0;
                    m_aryCsvTitleTbl[0] = "ユーザID";
                    m_aryFildKeyTbl[0] = "%L_UID%";
                }
                else if (nSelect == 1)
                {
                    m_nOutputId = 1;
                    m_aryCsvTitleTbl[0] = "社員番号";
                    m_aryFildKeyTbl[0] = "%C_OCODE%";
                }
                nSelect = cmbOutputType.SelectedIndex;
                if (nSelect == 0)
                {
                    m_nOutputType = 0;
                }
                else if (nSelect == 1)
                {
                    m_nOutputType = 1;
                }

                if (rdoCheckTime.IsChecked == true)
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

                SetNextCheckTime(m_sBaseDate, m_sBaseTime);
                sy = m_sCheckTime.Substring(0,4);
                sM = m_sCheckTime.Substring(4,2);
                sd = m_sCheckTime.Substring(6,2);
                sH = m_sCheckTime.Substring(8,2);
                sm = m_sCheckTime.Substring(10,2);
                ss = m_sCheckTime.Substring(12,2);
                sMsg = "「適応」処理を行いました。\n　次回CSVファイル作成日時は\n";
                sMsg = sMsg+"[" + sy +"/"+ sM +"/"+ sd +" "+ sH +":"+ sm +":"+ ss +"]　になります。";
                MessageBox.Show(sMsg);

                CheckLoopExec();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("btnSetClick");
                App.LogOut(ex.ToString());
            }
        }
        private void btnMiniClick()
        {
            try
            {
                App.Current.MainWindow.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("btnMiniClick");
                App.LogOut(ex.ToString());
            }
        }
        private void btnExitClick()
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                App.LogOut(ex.ToString());
            }
        }
        private string GetIntervalString(string hhmin)
        {
            string[] ary;
            int hh, min;

            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("GetIntervalString");
                App.LogOut(ex.ToString());
                return ("0005");
            }
        }
        private void InitCmbCheckTimes()
        {
            List<string> lstHH = new List<string>();
            List<string> lstMM = new List<string>();
            int idx;

            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitCmbCheckTimes");
                App.LogOut(ex.ToString());
            }
        }
        private void SetCmbCheckTimes(string sList)
        {
            string[] ary;
            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("SetCmbCheckTimes");
                App.LogOut(ex.ToString());
            }
        }
        private string GetcmbCheckTimes()
        {
            string sHH, sMM;
            string sList;
            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("GetcmbCheckTimes");
                App.LogOut(ex.ToString());
                return ("0000");
            }
        }
        private void SortSetLstCheckTime(string sList)
        {
            int max, idx, setidx;
            string sHHMM;

            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("GetcmbCheckTimes");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(cmbCheckHH1.SelectedIndex == 0){
                    cmbCheckMM1.SelectedValue = "";
                }
                else
                {
                    cmbCheckMM1.SelectedValue = "00";
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH1_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH2_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH3_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH4_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH5_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }

        private void cmbCheckHH6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("cmbCheckHH6_SelectionChanged");
                App.LogOut(ex.ToString());
            }
        }
    }
}

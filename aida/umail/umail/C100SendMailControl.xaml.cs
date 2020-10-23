using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace umail
{
    /// <summary>
    /// C100SendMailWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class C100SendMailControl : UserControl
    {
        private Boolean m_bCheckUnisSystem;
        // ターミナルのPINGが非同期のため１度でもPINGを成功した場合true; ターミナルチェック前にfalse;
        private Boolean m_bCheckTerminal;
        private Boolean m_bInCheckTerminal; // PING非同期開始時にtrue 終了時false
        private Boolean m_bLocalCheckTerminal; // ターミナルPingチェックループフラグ

        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;
        private LibUsbProtect m_libUsbProtect;
        private string m_sLogFileName;
        public DispatcherTimer m_dsptCheckUnis;
        public DispatcherTimer m_dsptCheckTerminal;
        public DispatcherTimer m_dsptSendMail;
        private String m_sUNISV3ProcessName = "UNIS_Access";
        private String m_sUNISV4ProcessName = "UNIS_RManager";
        public int m_nTimeCount;
        private string m_nCrthhmmss;

        public C100SendMailControl()
        {
            string sMsg;
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            m_libUsbProtect = new LibUsbProtect();
            int ret = m_libUsbProtect.ProtectMail();
            if (ret != Constants.USBPROTECTNO)
            {
                sMsg = "プロテクトが確認できません。";
                MessageBox.Show(sMsg);
                m_Wnd.ExitMainWindow();
                return;
            }
            InitControl();
        }

        private void InitControl()
        {
            double sec;
            string sYear;
            string sMonth;
            string sDay;
            string sPath;

            m_bCheckUnisSystem = false;
            m_bCheckTerminal = false;
            DateTime dt = DateTime.Now;
            sYear = dt.ToString("yyyy");
            sMonth = dt.ToString("MM");
            sDay = dt.ToString("dd");
            txtDate.Text = sYear + "年" + sMonth + "月" + sDay + "日";
            sPath = m_Wnd.GetLogFilePath() + "\\" + sYear;
            if (!System.IO.Directory.Exists(sPath))
            {
                System.IO.Directory.CreateDirectory(sPath);
            }
            m_sLogFileName = sPath + "\\" + sMonth + sDay + ".txt";
            if (m_Wnd.GetLogSave() == true)
            {
                LoadLogFile();
            }

            sec = m_Wnd.GetIntervalSec();

            m_dsptCheckUnis = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckUnis.Interval = TimeSpan.FromMilliseconds(sec * 1000);
            m_dsptCheckUnis.Tick += new EventHandler(TickCheckUnisLoop);
            m_dsptCheckTerminal = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckTerminal.Interval = TimeSpan.FromMilliseconds(sec * 1000);
            m_dsptCheckTerminal.Tick += new EventHandler(TickCheckTerminalLoop);
#if DEBUG
#else
#endif
            m_dsptSendMail = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptSendMail.Interval = TimeSpan.FromMilliseconds(sec * 1000);
            m_dsptSendMail.Tick += new EventHandler(TickSendMailLoop);
#if DEBUG
            m_bCheckUnisSystem = true;
            m_bCheckTerminal = true;
#else
            DoDispatch();
            CheckUnisProcess();

            DoDispatch();
            CheckTerminalPing();

            DoDispatch();
            m_dsptCheckUnis.Start();
            m_dsptCheckTerminal.Start();
#endif
            m_dsptSendMail.Start();
        }
        private void ExitControl()
        {
            m_dsptCheckUnis.Stop();
            m_dsptCheckTerminal.Stop();
            m_dsptSendMail.Stop();
            m_bLocalCheckTerminal = true; // 途中でPING中断
            m_bCheckUnisSystem = false;
            m_bCheckTerminal = false;
            if (m_Wnd.GetLogSave() == true)
            {
                SaveLogFile();
            }
        }
        private void LoadLogFile()
        {
            string[] aryLine;
            int max, idx;

            aryLine = m_libCmn.LoadFileLineSJIS(m_sLogFileName);
            if (aryLine == null && aryLine.Length == 0)
            {
                return;
            }
            max = aryLine.Length;
            for (idx = 0; idx < max; idx++)
            {
                if (aryLine[idx] != "")
                {
                    lstLog.Items.Add(aryLine[idx]);
                }
            }
        }
        private void SaveLogFile()
        {
            string sData;
            int max, idx;

            sData = "";
            max = lstLog.Items.Count;
            for (idx = 0; idx < max; idx++)
            {
                sData = sData + lstLog.Items[idx].ToString() + "\r\n";
            }
            m_libCmn.SaveFileSJIS(m_sLogFileName, sData);
        }
        public void AddLstLog(string sMsg)
        {
            string sAddStr;

            sAddStr = sMsg+"\t\t\t"+ lstLog.Items.Count;
            lstLog.Items.Add(sAddStr);
            Object obj = lstLog.Items[lstLog.Items.Count - 1];
            lstLog.ScrollIntoView(obj);
        }
        private void TickCheckUnisLoop(object sender, EventArgs e)
        {
            CheckUnisProcess();
        }
        private void CheckUnisProcess()
        {
            string sMsg;
            Process[] processesV3;
            Process[] processesV4;

            processesV3 = Process.GetProcessesByName(m_sUNISV3ProcessName);
            processesV4 = Process.GetProcessesByName(m_sUNISV4ProcessName);
            if (processesV3.Length != 0 || processesV4.Length != 0)
            {
                m_bCheckUnisSystem = true;
            }
            else
            {
                sMsg = "プロセス" + m_sUNISV3ProcessName + processesV3 + "," + m_sUNISV4ProcessName+processesV4 + "起動していません。";
                MessageBox.Show(sMsg);
                m_bCheckUnisSystem = false;
                ExitControl();
                m_Wnd.ExitMainWindow();
            }
        }
        private void TickCheckTerminalLoop(object sender, EventArgs e)
        {
            CheckTerminalPing();
        }
        private void CheckTerminalPing()
        {
            List<ObjTerminalElement> list;
            ObjTerminalElement ote = new ObjTerminalElement();
            int max, idx;

            if (m_bCheckUnisSystem == true) 
            {
                m_bLocalCheckTerminal = false;
                m_Wnd.ODBCOpenUnisDB();
                m_Wnd.ODBCSelecttTerminal();
                m_Wnd.ODBCCloseUnisDB();
                list = m_Wnd.GetTerminalElementList();
                max = list.Count;
                for (idx = 0; idx < max; idx++)
                {
                    if (list[idx].m_bCheck == true)
                    {
                        CheckPingLoop(list[idx]);
                        if (m_bLocalCheckTerminal == true)
                        {
                            m_bCheckTerminal = true;
                            return;
                        }
                    }
                }
                m_bCheckTerminal = false;
                if (m_Wnd.GetStatusDisplay() == true)
                {
                    if (m_Wnd.GetStatusDisplay() == true)
                    {
                        txtStat.Text = "ターミナルが動いていません";
                    }
                }
            }
        }
        private void CheckPingLoop(ObjTerminalElement ote)
        {
            Ping ping;
            int idx, max;
            int timeOut = 1000;
            object userToken = null;

            ping = new Ping();
            max = 3;
            for (idx = 0; idx < max; idx++)
            {
                m_bInCheckTerminal = true;
                ping.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                ping.SendAsync(ote.m_sIpAdrs, timeOut, userToken);
                while (true)
                {
                    if (m_bInCheckTerminal == false)
                    {
                        break;
                    }
                    DoDispatch();
                }
                if (m_bLocalCheckTerminal == true)
                {
                    return;
                }
            }
        }
        public void DoDispatch()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            Dispatcher.PushFrame(frame);
        }
        public object ExitFrames(object frames)
        {
            ((DispatcherFrame)frames).Continue = false;
            return null;
        }

        private void PingCompletedCallback(Object sender, PingCompletedEventArgs arg)
        {
            if (arg.Reply.Status == IPStatus.Success)
            {
                m_bLocalCheckTerminal = true;
            }
            m_bInCheckTerminal = false;
        }
        private void TickSendMailLoop(object sender, EventArgs e)
        {
            SendMailLoop();
        }
        private void SendMailLoop()
        {
            ObjSendRecord objSendRecord;
            int ret;
            int cnt;
            string sMsg;
            string sYear;
            string sMonth;
            string sDay;
            string sHour;
            string sMinute;
            string sSecond;
            string sDate;
            string sTime;
            int max, idx;

            if (m_bCheckUnisSystem == true && m_bCheckTerminal == true)
            {
                ret = m_libUsbProtect.ProtectMail();
                if (ret != Constants.USBPROTECTNO)
                {
                    sMsg = "プロテクトが確認できません。";
                    MessageBox.Show(sMsg);
                    ExitControl();
                    m_Wnd.ExitMainWindow();
                    return;

                }
                cnt = m_Wnd.GetMailElementUserCount();
                if (m_Wnd.GetStatusDisplay() == true)
                {
                    txtStat.Text = "UMailシステム動作中 登録者数" + cnt + "人";
                }
                DateTime dt = DateTime.Now;
                sYear = dt.ToString("yyyy");
                sMonth = dt.ToString("MM");
                sDay = dt.ToString("dd");
                sHour = dt.ToString("HH");
                sMinute = dt.ToString("mm");
                sSecond = dt.ToString("ss");

                m_nTimeCount++;
                txtDate.Text = sYear + "年" + sMonth + "月" + sDay + "日";
                m_nCrthhmmss = sHour + "時" + sMinute + "分" + sSecond + "秒";
                sDate = sYear + sMonth + sDay;
                sTime = sHour + sMinute + sSecond;

                m_Wnd.ODBCOpenUnisDB();
                objSendRecord = m_Wnd.ODBCSelecttEnter();
                m_Wnd.ODBCCloseUnisDB();
                max = objSendRecord.m_lstRecord.Count;
                for (idx = 0; idx < max; idx++)
                {
                    m_Wnd.SendMailLoop(objSendRecord.m_lstRecord[idx]);
                }
                if (objSendRecord.m_bRet == true)
                {
                    m_Wnd.SetBaseDate(sDate);
                    m_Wnd.SetBaseTime(sTime);
                }
            }
        }
        private void btnEnv_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC110EnvSetControl();
        }
        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC120MailSetControl();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (m_Wnd.CheckCloseWindows() == true)
            {
                ExitControl();
                m_Wnd.ExitMainWindow();
            }
        }
    }
}

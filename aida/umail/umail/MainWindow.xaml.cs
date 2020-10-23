using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;    // Assembly

namespace umail
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string m_sExecPath;
        private string m_sEnvPath;
        private UserControl m_objCrt;
        private LibUsbProtect m_libUsbProtect;
        private LibCommon m_libCmn;

        public MainWindow()
        {
            string sMsg;

            InitializeComponent();
            InitElement();
            m_libCmn = new LibCommon();
            m_libUsbProtect = new LibUsbProtect();
            m_sExecPath = InitExePath();
            m_sEnvPath = InitEnvPath();
            LoadEnvFile();
            MainWindowODBCInit(); // チェック開始時刻をUMail起動時に設定
#if DEBUG
            SetBaseDate("20161027");
            SetBaseTime("103234");
#endif
            InitC100SendMailControl();
        }
        public void ExitMainWindow()
        {
            ChangeUserControl();
            SaveEnvFile();
            this.Close();
        }
        public LibCommon GetClassLibCommon()
        {
            return (m_libCmn);
        }
        public LibCommon GetClassLibUsbProtect()
        {
            return (m_libCmn);
        }
        public string GetBaseDate()
        {
            return (m_sBaseDate);
        }
        public string GetBaseTime()
        {
            return (m_sBaseTime);
        }
        public void SetBaseDate(string sBaseDate)
        {
            m_sBaseDate = sBaseDate;
        }
        public void SetBaseTime(string sBaseTime)
        {
            m_sBaseTime = sBaseTime;
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
            string sPath;

            sEnvPath = "c:\\ProgramData";
            if (!System.IO.Directory.Exists(sEnvPath))
            {
                System.IO.Directory.CreateDirectory(sEnvPath);
            }
            sEnvPath = sEnvPath + "\\umail";
            if (!System.IO.Directory.Exists(sEnvPath))
            {
                System.IO.Directory.CreateDirectory(sEnvPath);
            }
            sPath = sEnvPath + "\\log";
            if (!System.IO.Directory.Exists(sPath))
            {
                System.IO.Directory.CreateDirectory(sPath);
            }
            return (sEnvPath);
        }
        public string GetLogFilePath()
        {
            string sPath;

            sPath = m_sEnvPath + "\\log";
            return (sPath);
        }
        public void InitC100SendMailControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C100SendMailControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC110EnvSetControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C110EnvSetControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC120MailSetControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C120MailSetControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC111TerminalSelectControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C111TerminalSelectControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC112MailServerSettingControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C112MailServerSettingControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC113AdminMailSettingControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C113AdminMailSettingControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC114ArrivalMailSettingControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C114ArrivalMailSettingControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        public void InitC115RetHomeMailSettingControl()
        {
            Grid gr = this.FindName("MainGrid") as Grid;

            if (ChangeUserControl() == false)
            {
                return;
            }
            m_objCrt = new C115RetHomeMailSettingControl();
            m_objCrt.SetValue(Grid.RowProperty, 1);
            gr.Children.Add(m_objCrt);
        }
        private Boolean ChangeUserControl()
        {
            Boolean ret;
            Grid gr = this.FindName("MainGrid") as Grid;
            ret = true;

            if (1 <= gr.Children.Count)
            {
                gr.Children.RemoveRange(0, gr.Children.Count);
            }
            return (ret);
        }

        private void CheckSave()
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (m_objCrt == null)
            {
                return;
            }
            Type type = m_objCrt.GetType();
            if (type == typeof(C100SendMailControl))
            {
                if (CheckCloseWindows() == false)
                {
                    e.Cancel = true;
                }
                else
                {
                    SaveEnvFile();
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
        public Boolean CheckCloseWindows()
        {
            string sTitle;
            string sMsg;

            sTitle = "終了確認";
            sMsg = "本当に終了しますか。";
            MessageBoxResult ret = MessageBox.Show(sMsg, sTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (MessageBoxResult.Yes == ret)
            {
                m_objCrt = null;
                return (true);
            }
            return (false);
        }

    }
}

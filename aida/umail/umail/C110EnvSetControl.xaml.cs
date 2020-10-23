using System;
using System.Collections.Generic;
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

namespace umail
{
    /// <summary>
    /// C110EnvSetControl.xaml の相互作用ロジック
    /// </summary>
    public partial class C110EnvSetControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;

        public C110EnvSetControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            InitControl();
        }
        private void InitControl()
        {
            Boolean nFlag;

            nFlag = m_Wnd.GetStatusDisplay();
            chkStatusDisplay.IsChecked = nFlag;
            InitCmbInterval();
            cmbInterval.Text = m_Wnd.GetIntervalSec().ToString();
            nFlag = m_Wnd.GetAdminSend();
            chkAdminSend.IsChecked = nFlag;
            nFlag = m_Wnd.GetLogSave();
            chkLogSave.IsChecked = nFlag;

        }
        private void ExitControl()
        {
            Boolean nFlag;
            String sInterval;
            double dInterval;

            nFlag = (Boolean)chkStatusDisplay.IsChecked;
            m_Wnd.SetStatusDisplay(nFlag);
            sInterval = cmbInterval.Text;
            dInterval = m_libCmn.StrToDouble(sInterval);
            m_Wnd.SetIntervalSec(dInterval);
            nFlag = (Boolean)chkAdminSend.IsChecked;
            m_Wnd.SetAdminSend(nFlag);
            nFlag = (Boolean)chkLogSave.IsChecked;
            m_Wnd.SetLogSave(nFlag);
        }
        private void InitCmbInterval()
        {
            int[] lst = new int[] { 30, 40, 50, 60, 70, 80, 90};

            cmbInterval.ItemsSource = lst;
            cmbInterval.IsEditable = true;
            cmbInterval.IsReadOnly = false;

        }
        private void btnTerminalSelect_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC111TerminalSelectControl();
        }

        private void btnMailServerSetting_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC112MailServerSettingControl();
        }

        private void btnAdminMailSetting_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC113AdminMailSettingControl();
        }
        private void btnArrivalMailSetting_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC114ArrivalMailSettingControl();
        }
        private void btnRetHomeMailSetting_Click(object sender, RoutedEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC115RetHomeMailSettingControl();
        }
        private void btnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC100SendMailControl();
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Wnd.InitC100SendMailControl();
        }
    }
}

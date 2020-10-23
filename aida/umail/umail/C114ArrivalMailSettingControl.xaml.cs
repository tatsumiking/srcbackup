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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace umail
{
    /// <summary>
    /// C114ArrivalMailSettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class C114ArrivalMailSettingControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;
        private TextBox m_tbCrtSlct;

        public C114ArrivalMailSettingControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            m_tbCrtSlct = null;
            InitControl();
        }
        private void InitControl()
        {
            txtMailSubject.Text = m_Wnd.GetArrivalSubject();
            txtMailBody.Text = m_Wnd.GetArrivalBody(); 
        }
        private void ExitControl()
        {
            m_Wnd.SetArrivalSubject(txtMailSubject.Text);
            m_Wnd.SetArrivalBody(txtMailBody.Text);
        }
        private void btnTID_Click(object sender, RoutedEventArgs e)
        {
            if (m_tbCrtSlct == null)
            {
                return;
            }
            Clipboard.SetText("[ターミナルID]");
            m_tbCrtSlct.Paste();
        }

        private void btnTName_Click(object sender, RoutedEventArgs e)
        {
            if (m_tbCrtSlct == null)
            {
                return;
            }
            Clipboard.SetText("[ターミナル名]");
            m_tbCrtSlct.Paste();
        }
        private void btnUID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[ユーザID]");
            m_tbCrtSlct.Paste();
        }

        private void btnUName_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[ユーザ名]");
            m_tbCrtSlct.Paste();
        }

        private void btnSName_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[受信者名]");
            m_tbCrtSlct.Paste();
        }

        private void btnSMail_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[受信者メール]");
            m_tbCrtSlct.Paste();
        }

        private void btnSYMD_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[西暦年月日]");
            m_tbCrtSlct.Paste();
        }

        private void btnWYMD_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[和暦年月日]");
            m_tbCrtSlct.Paste();
        }

        private void btnWHHMM_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[○○時○○分]");
            m_tbCrtSlct.Paste();
        }

        private void btnSHHMM_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("[○○：○○]");
            m_tbCrtSlct.Paste();
        }
        private void txtMailSubject_GotFocus(object sender, RoutedEventArgs e)
        {
            m_tbCrtSlct = txtMailSubject;
        }

        private void txtMailBody_GotFocus(object sender, RoutedEventArgs e)
        {
            m_tbCrtSlct = txtMailBody;
        }
        private void btnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC110EnvSetControl();
        }
        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Wnd.InitC110EnvSetControl();
        }
    }
}

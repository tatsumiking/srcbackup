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
    /// C112MailServerSettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class C112MailServerSettingControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;

        public C112MailServerSettingControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            InitControl();
        }
        private void InitControl()
        {
            Boolean bSsl;

            txtHostName.Text = m_Wnd.GetServerHostName();
            txtPortNo.Text = m_Wnd.GetServerPortNo().ToString();
            bSsl = m_Wnd.GetServerSsl();
            if (bSsl == true)
            {
                rdoSslTrue.IsChecked = true;
            }
            else
            {
                rdoSslFalse.IsChecked = true;
            }
            txtMailAddress.Text = m_Wnd.GetServerMail();
            txtPassWord.Password = m_Wnd.GetServerPassWord();
        }
        private void ExitControl()
        {
            /*
            int nPort;

            m_Wnd.SetServerHostName(txtHostName.Text);
            nPort = Int32.Parse(txtPortNo.Text);
            m_Wnd.SetServerPortNo(nPort);
            if (rdoSslTrue.IsChecked == true)
            {
                m_Wnd.SetServerSsl(true);
            }
            else
            {
                m_Wnd.SetServerSsl(false);
            }
            m_Wnd.SetServerMail(txtMailAddress.Text);
            m_Wnd.SetServerPassWord(txtPassWord.Password);
            */
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

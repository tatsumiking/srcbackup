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

namespace Observe
{
    /// <summary>
    /// CardWin.xaml の相互作用ロジック
    /// </summary>
    public partial class CardWin : Window
    {
        MainWindow m_wndMain;
        ClsCard m_clsCard;

        public CardWin()
        {
            InitializeComponent();
        }
        public void SetMainWindow(MainWindow wnd)
        {
            m_wndMain = wnd;
        }
        public void SetClsCard(ClsCard clsCard)
        {
            m_clsCard = clsCard;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSetNo.Text = "";
            //txtIP.Text = "";
            txtSyoNo.Text = "";
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtTel1.Text = "";
            txtTel2.Text = "";
            txtName.Text = "";
            txtBikou.Text = "";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            m_wndMain.ResetClsCardElement();
            this.Close();
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            m_clsCard.m_sSetNo = txtSetNo.Text;
            //m_clsCard.m_sIP = txtIP.Text;
            m_clsCard.m_sSyoNo = txtSyoNo.Text;
            m_clsCard.m_sAddress1 = txtAddress1.Text;
            m_clsCard.m_sAddress2 = txtAddress2.Text;
            m_clsCard.m_sTel1 = txtTel1.Text;
            m_clsCard.m_sTel2 = txtTel2.Text;
            m_clsCard.m_sName = txtName.Text;
            m_clsCard.m_sBikou = txtBikou.Text;
            m_wndMain.SetClsCardElement(m_clsCard);
            this.Close();
        }
    }
}

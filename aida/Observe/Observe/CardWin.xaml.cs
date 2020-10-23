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
        public void setCardLatLnd(double dLat, double dLnd)
        {
            m_clsCard.m_dLat = dLat;
            m_clsCard.m_dLnd = dLnd;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_clsCard == null)
            {
                txtSetNo.Text = "";
                txtIP.Text = "";
                txtSyoNo.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtTel1.Text = "";
                txtTel2.Text = "";
                txtName.Text = "";
                txtBikou.Text = "";
            }
            else
            {
                txtSetNo.Text = m_clsCard.m_sSetNo;
                txtIP.Text = "";
                txtSyoNo.Text = m_clsCard.m_sSyoNo;
                txtAddress1.Text = m_clsCard.m_sAddress1;
                txtAddress2.Text = m_clsCard.m_sAddress2;
                txtTel1.Text = m_clsCard.m_sTel1;
                txtTel2.Text = m_clsCard.m_sTel1;
                txtName.Text = m_clsCard.m_sName;
                txtBikou.Text = m_clsCard.m_sBikou;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // m_wndMain.ResetClsCardElement();
            this.Close();
            m_wndMain.m_cardWin = null;
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            m_clsCard.m_sSetNo = txtSetNo.Text;
            m_clsCard.m_sSyoNo = txtSyoNo.Text;
            m_clsCard.m_sAddress1 = txtAddress1.Text;
            m_clsCard.m_sAddress2 = txtAddress2.Text;
            m_clsCard.m_sTel1 = txtTel1.Text;
            m_clsCard.m_sTel2 = txtTel2.Text;
            m_clsCard.m_sName = txtName.Text;
            m_clsCard.m_sBikou = txtBikou.Text;
            m_wndMain.SetCrtCardElement(m_clsCard);
            this.Close();
            m_wndMain.m_cardWin = null;
        }
        private void btnMovePos_Click(object sender, RoutedEventArgs e)
        {
            String sAddress;
            ClsLatLnd clsLatLnd;

            sAddress = txtAddress1.Text;
            clsLatLnd = m_wndMain.getAddressToLatLnd(sAddress);
            m_clsCard.m_dLat = clsLatLnd.m_dLat;
            m_clsCard.m_dLnd = clsLatLnd.m_dLnd;
            m_wndMain.SetCrtCardLatLnd(clsLatLnd.m_dLat, clsLatLnd.m_dLnd);
            m_wndMain.moveLatLnd(clsLatLnd);
        }
    }
}

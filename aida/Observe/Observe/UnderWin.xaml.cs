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
    /// UnderWin.xaml の相互作用ロジック
    /// </summary>
    public partial class UnderWin : Window
    {
        private MainWindow m_wndMain;
        public LibCommon m_libCmn;
        public LibCanvas m_libCnvs;
        private Button m_btnReset;

        public UnderWin()
        {
            InitializeComponent();
            m_libCnvs = new LibCanvas();
        }
        public void SetMainWindow(MainWindow wnd)
        {
            m_wndMain = wnd;
        }
        private void cnvsMsg_Loaded(object sender, RoutedEventArgs e)
        {
            dispMsg("");
        }
        public void dispMsg(String sMsg)
        {
            double wd, hi;
            double addy;

            cnvsMsg.Children.Clear();
            if (sMsg == "")
            {
                cnvsMsg.Background = Brushes.White;
            }
            else
            {
                wd = cnvsMsg.ActualWidth;
                hi = cnvsMsg.ActualHeight;
                m_libCnvs.setFontSize(30);
                addy = (hi - 30) / 2;

                cnvsMsg.Background = Brushes.Red;
                m_libCnvs.setFillBrush(Brushes.Black);
                m_libCnvs.drawCenterText(cnvsMsg, 0, 0, wd, hi, 0, addy, sMsg);
                if (sMsg == "持ち去り警報")
                {
                    m_btnReset = m_libCnvs.drawButton(cnvsMsg, wd - 120, 30, 120, 30, "解除");
                    m_btnReset.Click += (sender, e) => btnTReset_onClick(sender);
                }

            }
        }
        private void btnTReset_onClick(object sender)
        {
            m_wndMain.resetUDPFlag();
        }
    }
}

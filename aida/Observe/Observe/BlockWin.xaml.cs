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
    /// BlockWin.xaml の相互作用ロジック
    /// </summary>
    public partial class BlockWin : Window
    {
        private MainWindow m_wndMain;
        public LibCanvas m_libCnvs;

        public BlockWin()
        {
            InitializeComponent();
            m_libCnvs = new LibCanvas();
        }
        public void SetMainWindow(MainWindow wnd)
        {
            m_wndMain = wnd;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetCardElement();
        }
        public void SetCardElement()
        {
            ClsCard clsCard;
            double dWidth, dHeight;
            double dLineSize, dClmSize;
            double sx, sy, ex, ey, cx, cy;
            double wd, hi;

            clsCard = m_wndMain.m_clsCardCrt;
            if (clsCard == null)
            {
                return;
            }
            cnvsCard.Children.Clear();

            dWidth = cnvsCard.ActualWidth;
            dHeight = cnvsCard.ActualHeight;
            dLineSize = dHeight / 8.0;
            dClmSize = dWidth / 53;

            m_libCnvs.setFontSize(dClmSize);
            wd = dClmSize * 10; hi = dLineSize;
            sx = 0;
            sy = 0;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "警報装置番号");
            sy = dLineSize;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "警報装置署番号");
            sy = dLineSize * 2;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "設置場所");
            sy = dLineSize * 4;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "設置場所連絡先");
            sy = dLineSize * 6;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "設置場所代表者");
            sy = dLineSize * 7;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "備考");

            wd = dClmSize * 8; hi = dLineSize;
            sx = dClmSize * 10;
            sy = 0;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, clsCard.m_sSetNo);
            sy = dLineSize;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, clsCard.m_sSyoNo);
            sy = dLineSize * 2;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "住所１");
            sy = dLineSize * 3;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "住所２");
            sy = dLineSize * 4;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "電話番号１");
            sy = dLineSize * 5;
            m_libCnvs.drawCenterText(cnvsCard, sx, sy, wd, hi, "電話番号２");


            wd = dClmSize * 35; hi = dLineSize;
            sx = dClmSize * 18;
            sy = dLineSize * 2;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sAddress1);
            sy = dLineSize * 3;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sAddress2);
            sy = dLineSize * 6;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sName);
            sy = dLineSize * 7;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sBikou);

            wd = dClmSize * 12; hi = dLineSize;
            sy = dLineSize * 4;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sTel1);
            sy = dLineSize * 5;
            m_libCnvs.drawLeftText(cnvsCard, sx, sy, wd, hi, clsCard.m_sTel2);

            sx = 0; ex = dWidth;
            cy = 0;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize*2;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 3;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 4;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 5;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 6;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 7;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);
            cy = dLineSize * 8;
            m_libCnvs.drawLine(cnvsCard, sx, cy, ex, cy);

            sy = 0; ey = dHeight;
            cx = 0;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);
            cx = dClmSize * 10;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);
            cx = dClmSize * 18;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);
            cx = dClmSize * 53;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);
            cx = dClmSize * 32;

            sy = 0; ey = dLineSize * 2;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);
            sy = dLineSize * 4; ey = dLineSize * 6;
            m_libCnvs.drawLine(cnvsCard, cx, sy, cx, ey);


            /*

                ;
                clsCard.m_sSyoNo;
                clsCard.m_sAddress1;
                clsCard.m_sAddress2;
                clsCard.m_sTel1;
                clsCard.m_sTel2;
                clsCard.m_sName;
                clsCard.m_sBikou;
            */
        }
    }
}

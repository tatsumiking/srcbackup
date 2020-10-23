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
        public double m_dOneCardHeight;

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
            if (m_wndMain.m_clsObserve != null)
            {
                SetListElement();
            }
        }
        public void SetListElement()
        {
            double dWidth, dHeight;
            double dLineSize;
            double dFontSize;
            double x1, x2;
            double wd, hi;
            double sx, sy, ex, ey;
            double addx, addy;
            int max, idx;
            Grid grid;
            ClsCard card;

            cnvsList.Children.Clear();
            m_libCnvs.setStrokeBrush(Brushes.Black, 1.0);
            dWidth = cnvsList.ActualWidth;
            dHeight = cnvsList.ActualHeight;
            dLineSize = 38.0;
            m_dOneCardHeight = dLineSize;
            dFontSize = 30.0;
            addx = 0;
            addy = 4;
            sx = 0; ex = dWidth;
            max = 11;
            for (idx = 0; idx < max; idx++)
            {
                sy = idx * dLineSize;
                m_libCnvs.drawLine(cnvsList, sx, sy, ex, sy);
                grid = new Grid();
                Canvas.SetLeft(grid, sx);
                Canvas.SetTop(grid, sy);
                grid.Width = dWidth;
                grid.Height = dHeight;
                grid.Name = "grid" + idx;
                //cnvsList.Children.Add(grid);
            }
            sy = 0;
            ey = dHeight;
            sx = 0;
            m_libCnvs.drawLine(cnvsList, sx, sy, sx, ey);
            x1 = dFontSize * 12;
            m_libCnvs.drawLine(cnvsList, x1, sy, x1, ey);
            x2 = dFontSize * 22;
            m_libCnvs.drawLine(cnvsList, x2, sy, x2, ey);
            ex = dWidth;
            m_libCnvs.drawLine(cnvsList, ex, sy, ex, ey);

            m_libCnvs.setFontSize(dFontSize);

            sx = dFontSize * 0.5;
            wd = dFontSize * 5;
            sy = 0;
            hi = dLineSize;
            m_libCnvs.setFillBrush(Brushes.Black);
            m_libCnvs.drawLeftText(cnvsList, sx, sy, wd, hi, addx, addy, "稼動●警報");
            sx = sx + wd;
            wd = dFontSize;
            m_libCnvs.setFillBrush(Brushes.Red);
            m_libCnvs.drawLeftText(cnvsList, sx, sy, wd, hi, addx, addy, "●");
            sx = sx + wd;
            wd = dFontSize*4;
            m_libCnvs.setFillBrush(Brushes.Black);
            m_libCnvs.drawLeftText(cnvsList, sx, sy, wd, hi, addx, addy, "持ち去り");
            sx = sx + wd;
            wd = dFontSize * 4;
            m_libCnvs.setFillBrush(Brushes.Green);
            m_libCnvs.drawLeftText(cnvsList, sx, sy, wd, hi, addx, addy, "●");

            sx = x1;
            wd = x2 - x1;
            m_libCnvs.setFillBrush(Brushes.Black);
            m_libCnvs.drawCenterText(cnvsList, sx, sy, wd, hi, addx, addy, "警報装置番号");

            sx = x2;
            wd = dWidth - x2;
            m_libCnvs.setFillBrush(Brushes.Black);
            m_libCnvs.drawCenterText(cnvsList, sx, sy, wd, hi, addx, addy, "警報装置署番号");

            if (m_wndMain == null || m_wndMain.m_clsObserve == null
             || m_wndMain.m_clsObserve.m_lstClsCard == null)
            {
                return;
            }

            hi = m_dOneCardHeight;
            max = m_wndMain.m_clsObserve.m_lstClsCard.Count;
            for (idx = 0; idx < max; idx++)
            {
                card = m_wndMain.m_clsObserve.m_lstClsCard[idx];

                sx = 0;
                sy = m_dOneCardHeight * (idx + 1);
                wd = x1;
                if (card.m_sStat == "0")
                {
                    m_libCnvs.setFillBrush(Brushes.Black);
                }
                else if (card.m_sStat == "1")
                {
                    m_libCnvs.setFillBrush(Brushes.Red);
                }
                else if (card.m_sStat == "2")
                {
                    m_libCnvs.setFillBrush(Brushes.Green);
                }
                m_libCnvs.drawCenterText(cnvsList, sx, sy, wd, hi, addx, addy, "●");
                sx = x1;
                wd = x2 - x1;
                m_libCnvs.setFillBrush(Brushes.Black);
                m_libCnvs.drawCenterText(cnvsList, sx, sy, wd, hi, addx, addy, card.m_sSetNo);
                sx = x2;
                wd = dWidth - x2;
                m_libCnvs.setFillBrush(Brushes.Black);
                m_libCnvs.drawCenterText(cnvsList, sx, sy, wd, hi, addx, addy, card.m_sSyoNo);
            }
        }
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int idx;
            Point pos = e.GetPosition(gridBlock);
            idx = (int)(pos.Y / m_dOneCardHeight) - 1;
            if (0 > idx)
            {
                return;
            }
            if (m_wndMain.m_clsObserve.m_lstClsCard.Count <= idx)
            {
                m_wndMain.m_nClsCardCrtIdx = -1;
                m_wndMain.setCrtCardIdx(-1);
            }
            else
            {
                m_wndMain.m_nClsCardCrtIdx = idx;
            }
            m_wndMain.setCrtCardWinDisp();
        }
    }
}

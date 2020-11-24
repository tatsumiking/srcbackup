using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Observe
{
    public partial class MainWindow : Window
    {
        private double m_dBackLat;
        private double m_dBackLnd;

        private void initMarkMouseEvent()
        {
            m_nMarkIdx = -1;
        }
        private int serchMark(int nx, int ny)
        {
            int max, idx;
            Point pt;
            int sx, sy;
            double dLen, dTLen;
            double dltx, dlty;
            int retidx;

            retidx = -1;
            dLen = 32;
            max = m_clsObserve.m_lstClsCard.Count;
            for (idx = 0; idx < max; idx++)
            {
                pt = m_clsObserve.m_lstClsCard[idx].getMarkPoint();
                sx = (int)pt.X;
                sy = (int)pt.Y;
                if (checkInCnvsMarkArea(sx, sy) == true)
                {
                    dltx = sx - nx;
                    dlty = sy - ny;
                    dTLen = Math.Sqrt(dltx * dltx + dlty * dlty);
                    if (dLen > dTLen)
                    {
                        dLen = dTLen;
                        retidx = idx;
                    }
                }
            }
            return (retidx);
        }
        private void markMouseDownEvent()
        {
            m_dBackLat = m_clsObserve.m_lstClsCard[m_nMarkIdx].m_dLat;
            m_dBackLnd = m_clsObserve.m_lstClsCard[m_nMarkIdx].m_dLnd;
        }
        private void markMouseMoveEvent(int nx, int ny)
        {
            double dLat, dLnd;

            dLnd = m_dBackLnd + (nx - m_nDownX) * m_dLndDotStep / m_dZoomTime;
            dLat = m_dBackLat + (ny - m_nDownY) * m_dLatDotStep / m_dZoomTime;
            SetCrtCardLatLnd(dLat, dLnd);
            if (m_cardWin != null)
            {
                m_cardWin.setCardLatLnd(dLat, dLnd);
            }
        }
        private void markMouseUpEvent(int nx, int ny)
        {
            double dLat, dLnd;

            dLnd = m_dBackLnd + (nx - m_nDownX) * m_dLndDotStep / m_dZoomTime;
            dLat = m_dBackLat + (ny - m_nDownY) * m_dLatDotStep / m_dZoomTime;
            SetCrtCardLatLnd(dLat, dLnd);
            if (m_cardWin != null)
            {
                m_cardWin.setCardLatLnd(dLat, dLnd);
            }
        }
        public void addCardMark()
        {
            int max, idx;
            ClsCard clsCard;
            int maxGps, idxGps;
            double dLat, dLnd;
            string sStr;
            ClsGpsPos clsGpsPos;
            Point pt;

            cnvsMarkArea.Children.Clear();
            max = m_clsObserve.m_lstClsCard.Count;
            for (idx = 0; idx < max; idx++)
            {
                clsCard = m_clsObserve.m_lstClsCard[idx];
                dLat = clsCard.m_dLat;
                dLnd = clsCard.m_dLnd;
                sStr = clsCard.m_sSyoNo;
                pt = addCnvsMoveCardMark(dLat, dLnd, sStr);
                m_clsObserve.m_lstClsCard[idx].setMarkPoint(pt);
                if (clsCard.m_sStat == "2")
                {
                    maxGps = clsCard.m_lstGpsPos.Count;
                    for (idxGps = 0; idxGps < maxGps; idxGps++)
                    {
                        clsGpsPos = clsCard.m_lstGpsPos[idxGps];
                        dLat = clsGpsPos.m_dLat;
                        dLnd = clsGpsPos.m_dLnd;
                        sStr = clsGpsPos.m_sDate;
                        addCnvsMoveGPSMark(dLat, dLnd, sStr);
                    }
                }
            }
        }
        private Point addCnvsMoveCardMark(double dLat, double dLnd, string sSetNo)
        {
            ClsLatLnd clsLapLnd;
            ClsPagePosXY cppRltvXY;
            Point pt;
            int sx, sy;

            clsLapLnd = new ClsLatLnd();
            clsLapLnd.m_dLat = dLat;
            clsLapLnd.m_dLnd = dLnd;
            cppRltvXY = convLatLndToRltvPagePosXY(clsLapLnd);
            sx = (int)((cppRltvXY.m_dPagePosX - m_nCrtX) * Constants.MAPDOTSIZE * m_dZoomTime);
            sy = (int)((cppRltvXY.m_dPagePosY - m_nCrtY) * Constants.MAPDOTSIZE * m_dZoomTime);
            sx = sx + (int)(m_nAddX * m_dZoomTime + m_nMoveX + m_nZoomAddX);
            sy = sy + (int)(m_nAddY * m_dZoomTime + m_nMoveY + m_nZoomAddY);
            pt = new Point();
            pt.X = sx;
            pt.Y = sy;
            if (checkInCnvsMarkArea(sx, sy) == false)
            {
                return (pt);
            }
            m_libCnvs.setFontSize(24);
            m_tbCrt = m_libCnvs.CreateTextBlock(0, 0, sSetNo);
            Canvas.SetLeft(m_tbCrt, sx - 24);
            Canvas.SetTop(m_tbCrt, sy - 24);
            cnvsMarkArea.Children.Add(m_tbCrt);
            m_imgCamera = new Image();
            Canvas.SetLeft(m_imgCamera, sx);
            Canvas.SetTop(m_imgCamera, sy);
            m_imgCamera.Width = 32;
            m_imgCamera.Stretch = Stretch.Fill;
            m_imgCamera.Source = new BitmapImage(new Uri("pic/camera.png", UriKind.Relative));
            cnvsMarkArea.Children.Add(m_imgCamera);
            return (pt);
        }
        private void addCnvsMoveGPSMark(double dLat, double dLnd, string sStr)
        {
            ClsLatLnd clsLapLnd;
            ClsPagePosXY cppRltvXY;
            int sx, sy;
            TextBlock tb;

            // pagepos?
            clsLapLnd = new ClsLatLnd();
            clsLapLnd.m_dLat = dLat;
            clsLapLnd.m_dLnd = dLnd;
            cppRltvXY = convLatLndToRltvPagePosXY(clsLapLnd);
            sx = (int)((cppRltvXY.m_dPagePosX - m_nCrtX) * Constants.MAPDOTSIZE * m_dZoomTime);
            sy = (int)((cppRltvXY.m_dPagePosY - m_nCrtY) * Constants.MAPDOTSIZE * m_dZoomTime);
            sx = sx + (int)(m_nAddX * m_dZoomTime + m_nMoveX + m_nZoomAddX);
            sy = sy + (int)(m_nAddY * m_dZoomTime + m_nMoveY + m_nZoomAddY);
            if (checkInCnvsMarkArea(sx, sy) == false)
            {
                return;
            }
            m_libCnvs.setFontSize(24);
            tb = m_libCnvs.CreateTextBlock(0, 0, sStr);
            tb.Foreground = Brushes.Red;
            Canvas.SetLeft(tb, sx);
            Canvas.SetTop(tb, sy);
            cnvsMarkArea.Children.Add(tb);
        }
        private Boolean checkInCnvsMarkArea(int x, int y)
        {
            if (x < 0 || y < 0 || cnvsMarkArea.ActualWidth < x || cnvsMarkArea.ActualHeight < y)
            {
                return (false);
            }
            return (true);
        }
    }
}

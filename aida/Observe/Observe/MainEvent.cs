using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Observe
{
    public partial class MainWindow : Window
    {
        private MouseButtonEventArgs m_evnt;
        private MouseButtonState m_statL, m_statR;
        private int m_nMarkIdx;
        private int m_nDownX, m_nDownY;
        private int m_nMoveX, m_nMoveY;


        private void initMouseEvent()
        {
            m_nDownX = 0;
            m_nDownY = 0;
            m_nMoveX = 0;
            m_nMoveY = 0;
            initMapMouseEvent();
            initMarkMouseEvent();

        }
        private void gridDrawArea_MouseDown(object sender, MouseButtonEventArgs evnt)
        {
            int nx, ny;

            m_statL = evnt.LeftButton;
            m_statR = evnt.RightButton;
            Point pos = evnt.GetPosition(cnvsMarkArea);
            nx = (int)pos.X;
            ny = (int)pos.Y;
            if (ny < 0)
            {
                m_bMapEdit = false;
                return;
            }
            m_nDownX = nx;
            m_nDownY = ny;
            m_nMoveX = 0;
            m_nMoveY = 0;
            m_nMarkIdx = serchMark(nx, ny);
            if (m_nMarkIdx != -1)
            {
                markMouseDownEvent();
            }else{
                m_bMapEdit = true;
                mapSetPosition();
            }

        }
        private void gridDrawArea_MouseMove(object sender, MouseEventArgs evnt)
        {
            int nx, ny;
            double dLen;

            dispMameoryCrt();
            if (evnt.LeftButton == MouseButtonState.Pressed)
            {
                Point pos = evnt.GetPosition(cnvsMarkArea);
                nx = (int)pos.X;
                ny = (int)pos.Y;
                if (m_nMarkIdx != -1)
                {
                    markMouseMoveEvent(nx, ny);
                }
                else
                {
                    if (m_bMapEdit == false)
                    {
                        return;
                    }
                    if (m_nCanvasWidth < nx)
                    {
                        return;
                    }
                    m_nMoveX = nx - m_nDownX;
                    m_nMoveY = ny - m_nDownY;
                    dLen = Math.Sqrt(m_nMoveX * m_nMoveX + m_nMoveY * m_nMoveY);
                    if (dLen >= 16)
                    {
                        if (m_bRetouMode == false)
                        {
                            m_nMoveX = properMapMoveX(m_nMoveX);
                            m_nMoveY = properMapMoveY(m_nMoveY);
                        }
                        mapSetPosition();
                    }
                }
            }
        }
        private void gridDrawArea_MouseUp(object sender, MouseButtonEventArgs evnt)
        {
            Point pos;
            int nx, ny;
            int nxmap, nymap;
            double dLen;

            m_evnt = evnt;
            pos = evnt.GetPosition(cnvsMarkArea);
            nx = (int)pos.X;
            ny = (int)pos.Y;
            if (m_nMarkIdx != -1)
            {
                markMouseUpEvent(nx, ny);
            }
            else
            {
                if (m_statR == MouseButtonState.Pressed)
                {
                    m_statR = MouseButtonState.Released;
                    initMapArea();
                    return;
                }
                if (m_statL == MouseButtonState.Pressed)
                {
                    m_statL = MouseButtonState.Released;
                }
                if (m_bMapEdit == false)
                {
                    return;
                }
                m_nMoveX = nx - m_nDownX;
                m_nMoveY = ny - m_nDownY;
                dLen = Math.Sqrt(m_nMoveX * m_nMoveX + m_nMoveY * m_nMoveY);
                if (dLen >= 16)
                {
                    if (m_bRetouMode == false)
                    {
                        m_nMoveX = properMapMoveX(m_nMoveX);
                        m_nMoveY = properMapMoveY(m_nMoveY);
                    }
                    nx = m_nMoveX + m_nDownX;
                    ny = m_nMoveY + m_nDownY;
                    mapMouseUpEvent(nx, ny);
                }
            }
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs evnt)
        {
            Point posMap;
            int nDelta;

            dispMameoryCrt();
            // 地図上の位置
            posMap = evnt.GetPosition(cnvsMarkArea);
            nDelta = evnt.Delta;

            mapMouseWheel(posMap, nDelta);
        }
    }
}

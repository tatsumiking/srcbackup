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
        private int m_nCrtX, m_nCrtY; // 地図の番号（東京都のトップからの加算XY）
        private int m_nDownX, m_nDownY;
        private int m_nZoomAddX, m_nZoomAddY;
        private double m_dZoomCenterX, m_dZoomCenterY;
        private double m_dZoomCenterTime;
        private int m_nMoveX, m_nMoveY;
        private int m_nAddX, m_nAddY;
        private Canvas m_cnvsMove;
        private double m_dZoomTime;
        private Boolean m_bMapEdit;
        private MouseButtonEventArgs m_evnt;
        private MouseButtonState m_statL, m_statR;

        private void initMouseEvent()
        {
            m_dZoomCenterX = 0;
            m_dZoomCenterY = 0;
            m_dZoomCenterTime = 1.0;
            m_dZoomTime = 1.0;
            m_nDownX = 0;
            m_nDownY = 0;
            m_nZoomAddX = 0;
            m_nZoomAddY = 0;
            m_nMoveX = 0;
            m_nMoveY = 0;
            // 東京駅 (29105 - 29030 = 75, 12903 - 12878 = 24);
            // 35.6810583,139.7665742
            m_nCrtX = (int)(79 - m_nWidthDiv);
            m_nCrtY = (int)(28 - m_nHeightDiv);
            //  左最大
            //m_nCrtX = 4 - m_nWidthDiv;
            //m_nCrtY = 4 - m_nHeightDiv;
            //m_nCrtX = 91 - m_nWidthDiv;
            //m_nCrtY = 47 - m_nHeightDiv;
            //m_nCrtX = 4 - m_nWidthDiv;
            //m_nCrtY = 8 - m_nHeightDiv;
            //m_nCrtX = 10 - m_nWidthDiv;
            //m_nCrtY = 4 - m_nHeightDiv;
            //m_nCrtX = 91 - m_nWidthDiv;
            //m_nCrtY = 26 - m_nHeightDiv;
            //m_nCrtX = 52 - m_nWidthDiv;
            //m_nCrtY = 47 - m_nHeightDiv;
            m_nAddX = -m_nWidthDiv * Constants.MAPDOTSIZE;
            m_nAddY = -m_nHeightDiv * Constants.MAPDOTSIZE;
        }
        private void gridDrawArea_MouseDown(object sender, MouseButtonEventArgs evnt)
        {
            int nx, ny;
            Point posMap;
            double cx, cy;

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
            m_bMapEdit = true;
            m_nDownX = nx;
            m_nDownY = ny;
            m_nMoveX = 0;
            m_nMoveY = 0;

            setMapPosition();

        }
        private void gridDrawArea_MouseMove(object sender, MouseEventArgs evnt)
        {
            int nx, ny;
            double dLen;
            double cx, cy;

            dispMameoryCrt();
            if (evnt.LeftButton == MouseButtonState.Pressed)
            {
                if (m_bMapEdit == false)
                {
                    return;
                }
                Point pos = evnt.GetPosition(cnvsMarkArea);
                nx = (int)pos.X;
                ny = (int)pos.Y;
                if (m_nCanvasWidth < nx)
                {
                    return;
                }

                m_nMoveX = nx - m_nDownX;
                m_nMoveY = ny - m_nDownY;
                dLen = Math.Sqrt(m_nMoveX * m_nMoveX + m_nMoveY * m_nMoveY);
               // if (dLen < 16)
               // {
               //     return;
               // }
                if (m_bRetouMode == false)
                {
                    m_nMoveX = properMapMoveX(m_nMoveX);
                    m_nMoveY = properMapMoveY(m_nMoveY);
                }
                setMapPosition();
            }
        }
        private void gridDrawArea_MouseUp(object sender, MouseButtonEventArgs evnt)
        {
            Point pos;
            int nx, ny;
            int nxmap, nymap;
            double dLen;

            m_evnt = evnt;

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
            pos = evnt.GetPosition(cnvsMarkArea);
            nx = (int)pos.X;
            ny = (int)pos.Y;
            if (m_bMapEdit == false)
            {
                return;
            }
            m_nMoveX = nx - m_nDownX;
            m_nMoveY = ny - m_nDownY;
            dLen = Math.Sqrt(m_nMoveX * m_nMoveX + m_nMoveY * m_nMoveY);
            if (dLen < 16)
            {
                if (m_nMapBase == 15)
                {
                    pos = evnt.GetPosition(m_cnvsMove);
                    nxmap = (int)pos.X;
                    nymap = (int)pos.Y;
                    setMarkPosition(nxmap, nymap, nx, ny);
                }
            }
            else
            {
                if (m_bRetouMode == false)
                {
                    m_nMoveX = properMapMoveX(m_nMoveX);
                    m_nMoveY = properMapMoveY(m_nMoveY);
                }
                nx = m_nMoveX + m_nDownX;
                ny = m_nMoveY + m_nDownY;
                mouseUpEvent(nx, ny);
            }
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs evnt)
        {
            Point posMap;
            double cx, cy;
            double dDltX, dDltY, dLen;
            int nblkx, nblky, nmodx, nmody;

            dispMameoryCrt();
            // 地図上の位置
            posMap = evnt.GetPosition(cnvsMarkArea);
            cx = posMap.X;
            cy = posMap.Y;
            //m_dZoomTime += (evnt.Delta) * 0.0005;
            if (0 < evnt.Delta)
            {
                m_dZoomTime += 0.05;
            }
            else
            {
                m_dZoomTime -= 0.05;
            }
            if (m_dZoomTime < 0.55)
            {
                if (10 < m_nMapBase && m_bRetouMode == false)
                {
                    m_nAddX = m_nAddX + m_nWidthDiv * Constants.MAPDOTSIZE + (int)(m_nZoomAddX / m_dZoomTime);
                    m_nAddY = m_nAddY + m_nHeightDiv * Constants.MAPDOTSIZE + (int)(m_nZoomAddY / m_dZoomTime);
                    m_dZoomTime = 1.0;
                    m_nCrtX = m_nCrtX + m_tblTopX[m_nMapTblIdx] + m_nWidthDiv;
                    m_nCrtY = m_nCrtY + m_tblTopY[m_nMapTblIdx] + m_nHeightDiv;
                    m_nMapBase = m_nMapBase - 1;
                    m_nMapTblIdx = m_nMapBase - 10;
                    m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
                    m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;
                    nblkx = m_nCrtX / 2;
                    nblky = m_nCrtY / 2;
                    nmodx = m_nCrtX % 2;
                    nmody = m_nCrtY % 2;

                    m_nCrtX = nblkx - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv;
                    m_nCrtY = nblky - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv;
                    m_nAddX = m_nAddX / 2 - nmodx * Constants.MAPDOTSIZE / 2;
                    m_nAddY = m_nAddY / 2 - nmody * Constants.MAPDOTSIZE / 2;

                    m_dZoomCenterX = cx;
                    m_dZoomCenterY = cy;
                    m_dZoomCenterTime = 1.0;
                    m_nZoomAddX = 0;
                    m_nZoomAddY = 0;
                    setProperValue(m_nAddX, m_nAddY);
                    return;
                }
                else
                {
                    m_dZoomTime = 0.5;
                }
            }
            if (1.95 < m_dZoomTime)
            {
                if (18 > m_nMapBase && m_bRetouMode == false)
                {
                    m_nAddX = m_nAddX + m_nWidthDiv * Constants.MAPDOTSIZE + (int)(m_nZoomAddX / m_dZoomTime);
                    m_nAddY = m_nAddY + m_nHeightDiv * Constants.MAPDOTSIZE + (int)(m_nZoomAddY / m_dZoomTime);
                    m_dZoomTime = 1.0;
                    m_nCrtX = m_nCrtX + m_tblTopX[m_nMapTblIdx] + m_nWidthDiv;
                    m_nCrtY = m_nCrtY + m_tblTopY[m_nMapTblIdx] + m_nHeightDiv;
                    m_nMapBase = m_nMapBase + 1;
                    m_nMapTblIdx = m_nMapBase - 10;
                    m_nLastX = m_tblEndX[m_nMapTblIdx] - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv * 3 + 2;
                    m_nLastY = m_tblEndY[m_nMapTblIdx] - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv * 3 + 2;
                    nblkx = m_nCrtX * 2;
                    nblky = m_nCrtY * 2;
                    m_nCrtX = nblkx - m_tblTopX[m_nMapTblIdx] - m_nWidthDiv;
                    m_nCrtY = nblky - m_tblTopY[m_nMapTblIdx] - m_nHeightDiv;
                    m_nAddX = m_nAddX * 2;
                    m_nAddY = m_nAddY * 2;
                    m_dZoomCenterX = cx;
                    m_dZoomCenterY = cy;
                    m_dZoomCenterTime = 1.0;
                    m_nZoomAddX = 0;
                    m_nZoomAddY = 0;
                    setProperValue(m_nAddX, m_nAddY);
                    return;
                }else{
                    m_dZoomTime = 2.0;
                }
            }
            dDltX = cx - m_dZoomCenterX;
            dDltY = cy - m_dZoomCenterY;
            dLen = Math.Sqrt(dDltX * dDltX + dDltY * dDltY);
            if (m_dZoomCenterTime != 1.0 && 16 < dLen)
            {
                cx = (cx - m_dZoomCenterX) / m_dZoomCenterTime + m_dZoomCenterX;
                cy = (cy - m_dZoomCenterY) / m_dZoomCenterTime + m_dZoomCenterY;
                m_nAddX = m_nAddX - (int)((cx - posMap.X) / m_dZoomTime);
                m_nAddY = m_nAddY - (int)((cy - posMap.Y) / m_dZoomTime);
            }
            else
            {
            }
            m_dZoomCenterX = cx;
            m_dZoomCenterY = cy;
            m_dZoomCenterTime = m_dZoomTime;
            m_nZoomAddX = (int)(cx * (1.0 - m_dZoomTime));
            m_nZoomAddY = (int)(cy * (1.0 - m_dZoomTime));
            setMapPosition();
        }
        private int properMapMoveX(int nMoveX)
        {
            int subdiv;
            if (m_nCrtX <= m_nWidthDiv)
            {
                if (-m_nWidthDiv * Constants.MAPDOTSIZE < (m_nAddX + nMoveX - m_nCrtX * Constants.MAPDOTSIZE))
                {
                    nMoveX = -m_nWidthDiv * Constants.MAPDOTSIZE - m_nAddX + m_nCrtX * Constants.MAPDOTSIZE;
                }
            }
            if ((m_nLastX - m_nHeightDiv) <= m_nCrtX)
            {
                subdiv = m_nCrtX - m_nLastX;
                if (-m_nWidthDiv * Constants.MAPDOTSIZE > (m_nAddX + nMoveX - subdiv * Constants.MAPDOTSIZE))
                {
                    nMoveX = -m_nWidthDiv * Constants.MAPDOTSIZE - m_nAddX + subdiv * Constants.MAPDOTSIZE;
                }
            }
            return (nMoveX);
        }
        private int properMapMoveY(int nMoveY)
        {
            int subdiv;
            if (m_nCrtY <= m_nHeightDiv)
            {
                if (-m_nHeightDiv * Constants.MAPDOTSIZE < (m_nAddY + nMoveY - m_nCrtY * Constants.MAPDOTSIZE))
                {
                    nMoveY = -m_nHeightDiv * Constants.MAPDOTSIZE - m_nAddY + m_nCrtY * Constants.MAPDOTSIZE;
                }
            }
            if ((m_nLastY - m_nHeightDiv) <= m_nCrtY)
            {
                subdiv = m_nCrtY - m_nLastY;
                if (-m_nHeightDiv * Constants.MAPDOTSIZE > (m_nAddY + nMoveY - subdiv * Constants.MAPDOTSIZE))
                {
                    nMoveY = -m_nHeightDiv * Constants.MAPDOTSIZE - m_nAddY + subdiv * Constants.MAPDOTSIZE;
                }
            }
            return (nMoveY);
        }
        private void mouseUpEvent(int nx, int ny)
        {
            int limit;
            int movex, movey;

            limit = Constants.MAPDOTSIZE;
            if (nx < 0){
                m_nMoveX = -m_nDownX;
            }else if(m_nCanvasWidth < nx)
            {
                m_nMoveX = m_nCanvasWidth-m_nDownY;
            }else{
                m_nMoveX = nx - m_nDownX;
            }
            if (ny < 0){
                m_nMoveY = -m_nDownY;
            }else if(m_nCanvasHeight < ny)
            {
                m_nMoveY = m_nCanvasHeight-m_nDownY;
            }else{
                m_nMoveY = ny - m_nDownY;
            }
            // m_nAddXの初期状態が-Constants.MAPDOTSIZE * m_nWidthDivからの変化
            movex = (int)(m_nAddX + m_nMoveX / m_dZoomTime + m_nWidthDiv * Constants.MAPDOTSIZE);
            movey = (int)(m_nAddY + m_nMoveY / m_dZoomTime + m_nHeightDiv * Constants.MAPDOTSIZE);
            if (-limit < movex && movex < limit && -limit < movey && movey < limit)
            {
                m_nAddX = m_nAddX + (int)(m_nMoveX / m_dZoomTime);
                m_nAddY = m_nAddY + (int)(m_nMoveY / m_dZoomTime);
                m_nMoveX = 0;
                m_nMoveY = 0;

                setMapPosition();

                return;
            }

            setProperValue(movex, movey);
        }
        private void setProperValue(int movex, int movey)
        {
            int limit;
            int modx, mody;
            int divx, divy;

            limit = Constants.MAPDOTSIZE;
            if (-limit < movex && movex < limit)
            {
                modx = movex;
                divx = 0;
            }else{
                modx = movex % Constants.MAPDOTSIZE;
                divx = movex / Constants.MAPDOTSIZE;
            }
            if (-limit < movey && movey < limit){
                mody = movey;
                divy = 0;
            }else{
                mody = movey % Constants.MAPDOTSIZE;
                divy = movey / Constants.MAPDOTSIZE;
            }
            m_nCrtX = m_nCrtX - divx;
            m_nCrtY = m_nCrtY - divy;

            m_nAddX = modx - m_nWidthDiv * Constants.MAPDOTSIZE;
            m_nAddY = mody - m_nHeightDiv * Constants.MAPDOTSIZE;
            m_nMoveX = 0;
            m_nMoveY = 0;

            initMapArea();
        }
        private void initMapArea()
        {
            int wd, hi;
            int size;
            int mapx, mapy;
            int sx, sy, ex, ey;
            int xidx, yidx;
            string filename;
            int posx, posy;

            if (m_cnvsMove == null)
            {
                return;
            }
            setMapPosition();
            cnvsMapArea.Children.Clear();
            freeMapCanvas();
            m_cnvsMove.Children.Clear();

            GC.Collect();

            wd = m_nCanvasWidth;
            hi = m_nCanvasHeight;
            string newPath = "M 0,0 L 0," + hi + " " + wd + "," + hi + " " + wd + ",0 Z";
            cnvsMapArea.Clip = Geometry.Parse(newPath);
            sx = m_tblTopX[m_nMapTblIdx] + m_nCrtX;
            sy = m_tblTopY[m_nMapTblIdx] + m_nCrtY;
            ex = sx + m_nWidthDiv * 3;
            ey = sy + m_nHeightDiv * 3;

            size = Constants.MAPDOTSIZE;
            for (xidx = 0, mapx = sx; mapx <= ex; xidx++, mapx++)
            {
                posx = xidx * size;
                for (yidx = 0, mapy = sy; mapy <= ey; yidx++, mapy++)
                {
                    posy = yidx * size;
                    filename = m_sMapPath + "\\" + m_nMapBase + "\\" + mapx + "\\" + mapy + ".png";
                    if (File.Exists(filename))
                    {
                        m_libCnvs.drawImage(m_cnvsMove, posx, posy, size, size, filename);
                    }
                    else
                    {
                        if (m_bRetouMode == true)
                        {
                            filename = m_sMapPath + "\\" + m_nMapBase + "\\000000.png";
                            m_libCnvs.drawImage(m_cnvsMove, posx, posy, size, size, filename);
                        }
                    }
                }
            }
            cnvsMapArea.Children.Add(m_cnvsMove);
        }
        private void setMapPosition()
        {
            double sx, sy;

            m_cnvsMove.LayoutTransform = new ScaleTransform(m_dZoomTime, m_dZoomTime);
            sx = m_nAddX * m_dZoomTime + m_nMoveX + m_nZoomAddX;
            sy = m_nAddY * m_dZoomTime + m_nMoveY + m_nZoomAddY;
            Canvas.SetLeft(m_cnvsMove, sx);
            Canvas.SetTop(m_cnvsMove, sy);
            addCardMark();

        }
        private void freeMapCanvas()
        {
            int idx, max;
            Object obj;
            Image img;

            max = m_cnvsMove.Children.Count;
            for (idx = max - 1; idx >= 0; idx--)
            {
                obj = m_cnvsMove.Children[idx];
                if(obj.GetType() == typeof(Image)){
                    img = (Image)obj;
                    img.Source = null;
                    img.Tag = null;
                }
            }
        }
    }
}

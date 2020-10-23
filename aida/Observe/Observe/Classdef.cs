using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Observe
{
    static class Constants
    {
        public const int MAPDOTSIZE = 256;
    }
    public partial class ClsXYPos
    {
        public int m_nSX;
        public int m_nSY;
        public int m_nEX;
        public int m_nEY;
    }
    public partial class ClsCanvas
    {
        public Canvas m_cnvs;
        public int m_nSX;
        public int m_nSY;
    }
    public partial class ClsLatLnd
    {
        public double m_dLat;
        public double m_dLnd;
    }
    public partial class ClsPagePosXY
    {
        public double m_dPagePosX;
        public double m_dPagePosY;
    }
    public partial class ClsObserve
    {
        [XmlElement("m_lstClsCard")]
        public List<ClsCard> m_lstClsCard;

        public ClsObserve()
        {
            m_lstClsCard = new List<ClsCard>();
        }
    }
    public partial class ClsZizuPos
    {
        [XmlElement("m_nTopPageX")]
        public int m_nTopPageX;
        [XmlElement("m_nTopPageY")]
        public int m_nTopPageY;
        [XmlElement("m_nOffsetX")]
        public int m_nOffsetX;
        [XmlElement("m_nOffsetY")]
        public int m_nOffsetY;
        [XmlElement("m_nZoomLevel")]
        public int m_nZoomLevel;
        [XmlElement("m_nZoomTime")]
        public double m_nZoomTime;
    }
    public partial class ClsCard
    {
        [XmlElement("m_sSetNo")]
        public string m_sSetNo;
        [XmlElement("m_sIP")]
        public string m_sIP;
        [XmlElement("m_sSyoNo")]
        public string m_sSyoNo;
        [XmlElement("m_sAddress1")]
        public string m_sAddress1;
        [XmlElement("m_sAddress2")]
        public string m_sAddress2;
        [XmlElement("m_sTel1")]
        public string m_sTel1;
        [XmlElement("m_sTel2")]
        public string m_sTel2;
        [XmlElement("m_sName")]
        public string m_sName;
        [XmlElement("m_sBikou")]
        public string m_sBikou;
        [XmlElement("m_dLat")]
        public double m_dLat;
        [XmlElement("m_dLnd")]
        public double m_dLnd;
        [XmlElement("m_sStat")]
        public String m_sStat;
        [XmlElement("m_lstGpsPos")]
        public List<ClsGpsPos> m_lstGpsPos;

        private int x;
        private int y;

        public ClsCard()
        {
            m_sSetNo = "";
            m_sStat = "";
            m_lstGpsPos = new List<ClsGpsPos>();
        }
        public Boolean checkIP(IPAddress checkip)
        {
            if (m_sIP == "")
            {
                return (false);
            }
            IPAddress ipadrs = IPAddress.Parse(m_sIP);
            if (checkip == ipadrs)
            {
                return (true);
            }
            return (false);
        
        }
        public void copySetElement(ClsCard clsCard)
        {
            int max, idx;
            ClsGpsPos clsGpsPosDst;

            m_dLat = clsCard.m_dLat;
            m_dLnd = clsCard.m_dLnd;
            m_sSetNo = clsCard.m_sSetNo;
            m_sIP = clsCard.m_sIP;
            m_sSyoNo = clsCard.m_sSyoNo;
            m_sAddress1 = clsCard.m_sAddress1;
            m_sAddress2 = clsCard.m_sAddress2;
            m_sTel1 = clsCard.m_sTel1;
            m_sTel2 = clsCard.m_sTel2;
            m_sName = clsCard.m_sName;
            m_sBikou = clsCard.m_sBikou;

            m_lstGpsPos.Clear();
            max = clsCard.m_lstGpsPos.Count;
            for (idx = 0; idx < max; idx++)
            {
                clsGpsPosDst = new ClsGpsPos();
                clsGpsPosDst.copySetElement(clsCard.m_lstGpsPos[idx]);
                m_lstGpsPos.Add(clsGpsPosDst);
            }
        }
        public void setMarkPoint(Point pt)
        {
            x = (int)pt.X;
            y = (int)pt.Y;
        }
        public Point getMarkPoint()
        {
            Point pt = new Point(x, y);
            return (pt);
        }
    }
    public partial class ClsGpsPos
    {
        [XmlElement("m_sDate")]
        public string m_sDate;
        [XmlElement("m_dLat")]
        public double m_dLat;
        [XmlElement("m_dLnd")]
        public double m_dLnd;

        public void copySetElement(ClsGpsPos clsGpsPos)
        {
            m_dLat = clsGpsPos.m_dLat;
            m_dLnd = clsGpsPos.m_dLnd;
            m_sDate = clsGpsPos.m_sDate;
        }
    }
}

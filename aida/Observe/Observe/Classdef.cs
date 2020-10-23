using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
    public partial class ClsObserve
    {
        [XmlElement("m_lstClsCard")]
        public List<ClsCard> m_lstClsCard;

        public ClsObserve()
        {
            m_lstClsCard = new List<ClsCard>();
        }
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
        [XmlElement("m_lstGpsPos")]
        public List<ClsGpsPos> m_lstGpsPos;

        public ClsCard()
        {
            m_sSetNo = "";
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
    }
    public partial class ClsGpsPos
    {
        [XmlElement("m_sDate")]
        public string m_sDate;
        [XmlElement("m_dLat")]
        public double m_dLat;
        [XmlElement("m_dLnd")]
        public double m_dLnd;

    }
}

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace umail
{
    public partial class ObjRecordElement
    {
        public int m_nLTid;
        public string m_sTName;
        public int m_nLUid;
        public string m_sCName;
        public string m_sCDate;
        public string m_sCTime;
        public int m_nLUserType;
        public string m_sCUnique;
        public int m_nLMode;
    }
    public partial class ObjSendRecord
    {
        public Boolean m_bRet;
        public List<ObjRecordElement> m_lstRecord = new List<ObjRecordElement>();
    }
    public partial class ObjMailElement
    {
        [XmlElement("m_nLUid")]
        public int m_nLUid; // ユーザID         
        [XmlElement("m_sCName")]
        public string m_sCName; // ユーザ名         
        [XmlElement("m_sSendName")]
        public string m_sSendName; // 送信先名         
        [XmlElement("m_sSendMail")]
        public string m_sSendMail; // 送信先アドレス
        [XmlElement("m_sCDateTime")]
        public string m_sCDateTime; // 到着年月日時分
        [XmlElement("m_nLTid")]
        public int m_nLTid; // ターミナルID         
        [XmlElement("m_sTName")]
        public string m_sTName; // ターミナル名         
    }
    public partial class ObjAdminElement
    {
        [XmlElement("m_sName")]
        public string m_sName; // 管理者名         
        [XmlElement("m_sMail")]
        public string m_sMail; // メール
    }
    public partial class ObjTerminalElement
    {
        [XmlElement("m_bCheck")]
        public Boolean m_bCheck;
        [XmlElement("m_nId")]
        public int m_nId; // ターミナルID         
        [XmlElement("m_sName")]
        public string m_sName; // ターミナル名
        [XmlElement("m_sIpAdrs")]
        public string m_sIpAdrs; // IPアドレス

        private Ping m_ping;
        private int m_nPingLoopCount;

        public ObjTerminalElement()
        {
            m_ping = null;
            m_nPingLoopCount = 0;
        }
        public void SetPing(Ping ping)
        {
            m_ping = ping;
        }
        public Ping GetPing()
        {
            return (m_ping);
        }
        public void SetPingLoopCount(int cnt)
        {
            m_nPingLoopCount = cnt;
        }
        public int GetPingLoopCount()
        {
            return (m_nPingLoopCount);
        }
    }
    public partial class MainWindow : Window
    {
        private Boolean m_bAutoStart;
        private Boolean m_bStatusDisplay;
        private double m_dIntervalSec;
        private Boolean m_bAdminSend;
        private Boolean m_bLogSave;
        private string m_sServerMail;
        private string m_sServerPassWord;
        private string m_sServerName;
        private string m_sServerHostName;
        private int m_nServerPortNo;
        private Boolean m_bServerSsl;
        private string m_sArrivalSubject;
        private string m_sArrivalBody;
        private string m_sRetHomeSubject;
        private string m_sRetHomeBody;
        private List<ObjMailElement> m_lstMailElement = new List<ObjMailElement>();
        private List<ObjAdminElement> m_lstAdminElement = new List<ObjAdminElement>();
        private List<ObjTerminalElement> m_lstTerminalElement = new List<ObjTerminalElement>();

        public void InitElement()
        {
            ObjMailElement ome;
            ObjAdminElement oae;

            m_bAutoStart = true;
            m_bStatusDisplay = true;
            m_dIntervalSec = 60;
            m_bAdminSend = true;
            m_bLogSave = true;

            m_sServerMail = "tatsumi@c-alfo.com";
            m_sServerPassWord = "im9876";
            m_sServerName = "自動送信メール";
            m_sServerHostName = "sv58.wadax.ne.jp";
            m_nServerPortNo = 587;
            m_bServerSsl = true;

            m_sArrivalSubject = "[受信者名]様へ到着のご案内";
            m_sArrivalBody = "[受信者名]様へ\n[ユーザ名]さんは[西暦年月日]に教室に到着しました\n";
            m_sRetHomeSubject = "[受信者名]様へ帰宅のご案内";
            m_sRetHomeBody = "[受信者名]様へ\n[ユーザ名]さんは[西暦年月日]に帰宅しました\n";

            oae = new ObjAdminElement();
            oae.m_sName = "管理者　太郎";
            oae.m_sMail = "kanrio@mail.co.jp";
            m_lstAdminElement.Add(oae);
        }
        private void LoadEnvFile()
        {
            LoadUMailEnvFile();
            LoadServerEnvFile();
            LoadArrivalSubjectFile();
            LoadArrivalBodyFile();
            LoadRetHomeSubjectFile();
            LoadRetHomeBodyFile();
            LoadMailElementListFile();
            LoadAdminElementListFile();
            LoadTerminalElementListFile();
        }
        private void LoadUMailEnvFile()
        {
            string sLoadFileName;
            string sData;
            string[] aryLine;

            sLoadFileName = m_sEnvPath + "\\umail.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_bAutoStart = m_libCmn.StrToBoolean(aryLine[0]);
                m_bStatusDisplay = m_libCmn.StrToBoolean(aryLine[1]);
                m_dIntervalSec = m_libCmn.StrToDouble(aryLine[2]);
                m_bAdminSend = m_libCmn.StrToBoolean(aryLine[3]);
                m_bLogSave = m_libCmn.StrToBoolean(aryLine[4]);
            }
        }
        private void LoadServerEnvFile()
        {
            string sLoadFileName;
            string sData;
            string[] aryLine;

            sLoadFileName = m_sEnvPath + "\\Server.env";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                sData = sData.Replace("\r\n", "\n");
                aryLine = sData.Split('\n');
                m_sServerMail = aryLine[0];
                m_sServerPassWord = aryLine[1];
                m_sServerName = aryLine[2];
                m_sServerHostName = aryLine[3];
                m_nServerPortNo = m_libCmn.StrToInt(aryLine[4]);
                m_bServerSsl = m_libCmn.StrToBoolean(aryLine[5]);
            }
        }
        private void LoadArrivalSubjectFile()
        {
            string sLoadFileName;
            string sData;

            sLoadFileName = m_sEnvPath + "\\ArrivalSubject.txt";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                m_sArrivalSubject = sData;
            }
        }
        private void LoadArrivalBodyFile()
        {
            string sLoadFileName;
            string sData;

            sLoadFileName = m_sEnvPath + "\\ArrivalBody.txt";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                m_sArrivalBody = sData;
            }
        }
        private void LoadRetHomeSubjectFile()
        {
            string sLoadFileName;
            string sData;

            sLoadFileName = m_sEnvPath + "\\RetHomeSubject.txt";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                m_sRetHomeSubject = sData;
            }
        }
        private void LoadRetHomeBodyFile()
        {
            string sLoadFileName;
            string sData;

            sLoadFileName = m_sEnvPath + "\\RetHomeBody.txt";
            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData != "")
            {
                m_sRetHomeBody = sData;
            }

        }
        private void LoadMailElementListFile()
        {
            string sLoadFileName;
            List<ObjMailElement> list;
            Type type;

            sLoadFileName = m_sEnvPath + "\\MailElement.xml";
            type = typeof(List<ObjMailElement>);
            list = (List<ObjMailElement>)m_libCmn.DataXmlLoad(sLoadFileName, type);
            if (list != null)
            {
                SetMailElementList(list);
            }
        }
        private void LoadAdminElementListFile()
        {
            string sLoadFileName;
            List<ObjAdminElement> list;
            Type type;

            sLoadFileName = m_sEnvPath + "\\AdminElement.xml";
            type = typeof(List<ObjAdminElement>);
            list = (List<ObjAdminElement>)m_libCmn.DataXmlLoad(sLoadFileName, type);
            if (list != null)
            {
                SetAdminElementList(list);
            }
        }
        private void LoadTerminalElementListFile()
        {
            string sLoadFileName;
            List<ObjTerminalElement> list;
            Type type;

            sLoadFileName = m_sEnvPath + "\\TerminalElement.xml";
            type = typeof(List<ObjTerminalElement>);
            list = (List<ObjTerminalElement>)m_libCmn.DataXmlLoad(sLoadFileName, type);
            if (list != null)
            {
                SetTerminalElementList(list);
            }
        }

        private void SaveEnvFile()
        {
            SaveUMailEnvFile();
            SaveServerEnvFile();
            SaveArrivalSubjectFile();
            SaveArrivalBodyFile();
            SaveRetHomeSubjectFile();
            SaveRetHomeBodyFile();
            SaveMailElementListFile();
            SaveAdminElementListFile();
            SaveTerminalElementListFile();
        }
        private void SaveUMailEnvFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\umail.env";
            sData = m_bAutoStart + "\n";
            sData = sData + m_bStatusDisplay + "\n";
            sData = sData + m_dIntervalSec + "\n";
            sData = sData + m_bAdminSend + "\n";
            sData = sData + m_bLogSave + "\n";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveServerEnvFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\Server.env";
            sData = m_sServerMail + "\n";
            sData = sData + m_sServerPassWord + "\n";
            sData = sData + m_sServerName + "\n";
            sData = sData + m_sServerHostName + "\n";
            sData = sData + m_nServerPortNo + "\n";
            sData = sData + m_bServerSsl + "\n";
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveArrivalSubjectFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\ArrivalSubject.txt";
            sData = m_sArrivalSubject;
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveArrivalBodyFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\ArrivalBody.txt";
            sData = m_sArrivalBody;
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveRetHomeSubjectFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\RetHomeSubject.txt";
            sData = m_sRetHomeSubject;
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveRetHomeBodyFile()
        {
            string sSaveFileName;
            string sData;

            sSaveFileName = m_sEnvPath + "\\RetHomeBody.txt";
            sData = m_sRetHomeBody;
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void SaveMailElementListFile()
        {
            string sSaveFileName;
            Type type;
            List<ObjMailElement> list;

            sSaveFileName = m_sEnvPath + "\\MailElement.xml";
            type = typeof(List<ObjMailElement>);
            list = GetMailElementList();
            m_libCmn.DataXmlSave(sSaveFileName, type, list);
        }
        private void SaveAdminElementListFile()
        {
            string sSaveFileName;
            Type type;
            List<ObjAdminElement> list;

            sSaveFileName = m_sEnvPath + "\\AdminElement.xml";
            type = typeof(List<ObjAdminElement>);
            list = GetAdminElementList();
            m_libCmn.DataXmlSave(sSaveFileName, type, list);
        }
        private void SaveTerminalElementListFile()
        {
            string sSaveFileName;
            Type type;
            List<ObjTerminalElement> list;

            sSaveFileName = m_sEnvPath + "\\TerminalElement.xml";
            type = typeof(List<ObjTerminalElement>);
            list = GetTerminalElementList();
            m_libCmn.DataXmlSave(sSaveFileName, type, list);
        }
        public void SetAutoStart(Boolean bAutoStart)
        {
            m_bAutoStart = bAutoStart;
        }
        public Boolean GetAutoStart()
        {
            return (m_bAutoStart);
        }
        public void SetStatusDisplay(Boolean bStatusDisplay)
        {
            m_bStatusDisplay = bStatusDisplay;
        }
        public Boolean GetStatusDisplay()
        {
            return (m_bStatusDisplay);
        }
        public void SetIntervalSec(double dIntervalSec)
        {
            m_dIntervalSec = dIntervalSec;
        }
        public double GetIntervalSec()
        {
            return (m_dIntervalSec);
        }
        public void SetAdminSend(Boolean bAdminSend)
        {
            m_bAdminSend = bAdminSend;
        }
        public Boolean GetAdminSend()
        {
            return (m_bAdminSend);
        }
        public void SetLogSave(Boolean bLogSave)
        {
            m_bLogSave = bLogSave;
        }
        public Boolean GetLogSave()
        {
            return (m_bLogSave);
        }

        public void SetServerMail(String sServerMail)
        {
            m_sServerMail = sServerMail;
        }
        public String GetServerMail()
        {
            return (m_sServerMail);
        }
        public void SetServerPassWord(String sServerPassWord)
        {
            m_sServerPassWord = sServerPassWord;
        }
        public String GetServerPassWord()
        {
            return (m_sServerPassWord);
        }
        public void SetServerName(String sServerName)
        {
            m_sServerName = sServerName;
        }
        public String GetServerName()
        {
            return (m_sServerName);
        }
        public void SetServerHostName(String sServerHostName)
        {
            m_sServerHostName = sServerHostName;
        }
        public String GetServerHostName()
        {
            return (m_sServerHostName);
        }
        public void SetServerPortNo(int nServerPortNo)
        {
            m_nServerPortNo = nServerPortNo;
        }
        public int GetServerPortNo()
        {
            return (m_nServerPortNo);
        }
        public void SetServerSsl(Boolean bServerSsl)
        {
            m_bServerSsl = bServerSsl;
        }
        public Boolean GetServerSsl()
        {
            return (m_bServerSsl);
        }
        public void SetArrivalSubject(String sArrivalSubject)
        {
            m_sArrivalSubject = sArrivalSubject;
        }
        public String GetArrivalSubject()
        {
            return (m_sArrivalSubject);
        }
        public void SetArrivalBody(String sArrivalBody)
        {
            m_sArrivalBody = sArrivalBody;
        }
        public String GetArrivalBody()
        {
            return (m_sArrivalBody);
        }
        public void SetRetHomeSubject(String sRetHomeSubject)
        {
            m_sRetHomeSubject = sRetHomeSubject;
        }
        public String GetRetHomeSubject()
        {
            return (m_sRetHomeSubject);
        }
        public void SetRetHomeBody(String sRetHomeBody)
        {
            m_sRetHomeBody = sRetHomeBody;
        }
        public String GetRetHomeBody()
        {
            return (m_sRetHomeBody);
        }
        public void SetMailElementList(List<ObjMailElement> list)
        {
            int max, idx;

            m_lstMailElement.Clear();
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (0 < list[idx].m_nLUid)
                {
                    m_lstMailElement.Add(list[idx]);
                }
            }
        }
        public List<ObjMailElement> GetMailElementList()
        {
            return (m_lstMailElement);
        }
        public int GetMailElementUserCount()
        {
            List<ObjMailElement> list;
            int max, idx;
            int umax, uidx;
            Boolean flag;


            list = new List<ObjMailElement>();
            max = m_lstMailElement.Count;
            for (idx = 0; idx < max; idx++)
            {
                flag = true;
                umax = list.Count;
                for (uidx = 0; uidx < umax; uidx++)
                {
                    if (list[uidx].m_nLUid == m_lstMailElement[idx].m_nLUid)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag == true)
                {
                    list.Add(m_lstMailElement[idx]);
                }
            }
            umax = list.Count;
            return (umax);
        }
        public void SetAdminElementList(List<ObjAdminElement> list)
        {
            int max, idx;

            m_lstAdminElement.Clear();
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (list[idx].m_sName != "")
                {
                    m_lstAdminElement.Add(list[idx]);
                }
            }
        }
        public List<ObjAdminElement> GetAdminElementList()
        {
            return (m_lstAdminElement);
        }
        public void SetTerminalElementList(List<ObjTerminalElement> list)
        {
            int max, idx;

            m_lstTerminalElement.Clear();
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (0 < list[idx].m_nId)
                {
                    m_lstTerminalElement.Add(list[idx]);
                }
            }
            m_lstTerminalElement = list;
        }
        public List<ObjTerminalElement> GetTerminalElementList()
        {
            return (m_lstTerminalElement);
        }
        public string GetTerminalName(int nTid)
        {
            int max, idx;

            max = m_lstTerminalElement.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (m_lstTerminalElement[idx].m_nId == nTid)
                {
                    return (m_lstTerminalElement[idx].m_sName);
                }
            }
            return ("");
        }
        public Boolean CheckUseTerminal(int nTid)
        {
            int max, idx;

            max = m_lstTerminalElement.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (m_lstTerminalElement[idx].m_nId == nTid)
                {
                    return (m_lstTerminalElement[idx].m_bCheck);
                }
            }
            return (false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Mail;
using System.Windows.Threading;
using System.Net.Http;

namespace umail
{
    public partial class MainWindow : Window
    {
        private string m_sUnisDBPath;
        private OdbcConnection m_conn = null;
        private OdbcCommand m_com;

        C100SendMailControl m_objSMC;
        private string m_sBaseDate;
        private string m_sBaseTime;
        private List<ObjTerminalElement> m_lstNewTE;

        private string m_sBFDate;
        private string m_sBFTime;
        private int m_nBFId;
        private int m_nDefLMode;

        public void MainWindowODBCInit()
        {
            string sPath;

            DateTime dt = DateTime.Now;
            m_sBaseDate = dt.ToString("yyyyMMdd");
            m_sBaseTime = dt.ToString("HHmmss");

            m_sBFDate = m_sBaseDate;
            m_sBFTime = m_sBaseTime;
            m_nBFId = 0;
            m_nDefLMode = 0;

            sPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            sPath = sPath + "\\UNIS\\unis.mdb";
#if DEBUG
            sPath = "d:\\temp\\unis.mdb";
#endif
            m_sUnisDBPath = sPath;
        }
        public string GetODBCDBPath()
        {
            return (m_sUnisDBPath);
        }
        public void ODBCOpenUnisDB()
        {
            Boolean bFlag;
            string sMsg;
            int idx, max;   

            bFlag = false;
            max = 10;
            m_conn = new OdbcConnection();
            m_conn.ConnectionString = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + m_sUnisDBPath + ";PWD=unisamho";
            for (idx = 0; idx < max; idx++)
            {
                try
                {
                    m_conn.Open();
                    bFlag = true;
                }
                catch (Exception ex)
                {
                    bFlag = false;
                }
                if (bFlag == true)
                {
                    break;
                }
            }
            if (bFlag == false)
            {
                sMsg = "データーベースに接続できません。";
                m_conn.Close();
                MessageBox.Show(sMsg);
                m_conn = null;
            }
        }
        public void DoDispatch()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrames(object frames)
        {
            ((DispatcherFrame)frames).Continue = false;
            return null;
        }
        public void ODBCCloseUnisDB()
        {
            if(m_conn == null){
                return;
            }
            m_conn.Close();
            m_conn = null;
        }
        public ObjSendRecord ODBCSelecttEnter()
        {
            string sSql;
            string sMsg;
            OdbcDataReader reader;
            ObjSendRecord objSendRecord;
            ObjRecordElement objRecordElement;

            objSendRecord = new ObjSendRecord();
            if (m_conn == null)
            {
                objSendRecord.m_bRet = false;
                return (objSendRecord);
            }
            sSql = "SELECT * FROM tEnter WHERE ((StrComp(C_Date,'" + m_sBaseDate + "') = 0)AND(StrComp(C_Time, '" + m_sBaseTime + "') >= 0));";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    objRecordElement = SetSendMailRecordElement(reader);
                    objSendRecord.m_lstRecord.Add(objRecordElement);
                }

            }
            catch (Exception ex)
            {
                //sMsg = "指紋照合テーブル参照に失敗";
                //m_objSMC = (C100SendMailControl)m_objCrt;
                //m_objSMC.AddLstLog(sMsg);
                objSendRecord.m_bRet = false;
                return (objSendRecord);
            }
            // 23時59分59秒より後に発生した入力を処理
            sSql = "SELECT * FROM tEnter WHERE (StrComp(C_Date,'" + m_sBaseDate + "') > 0);";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    objRecordElement = SetSendMailRecordElement(reader);
                    objSendRecord.m_lstRecord.Add(objRecordElement);
                }
            }
            catch (Exception ex)
            {
                //sMsg = "指紋照合テーブル参照に失敗";
                //m_objSMC = (C100SendMailControl)m_objCrt;
                //m_objSMC.AddLstLog(sMsg);
                objSendRecord.m_bRet = false;
                return (objSendRecord);
            }
            objSendRecord.m_bRet = true;
            return (objSendRecord);
        }
        public void ODBCSelecttTerminal()
        {
            string sSql;
            OdbcDataReader reader;
            string sMsg;

            if (m_conn == null)
            {
                return;
            }
            sSql = "SELECT * FROM tTerminal;";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                m_lstNewTE = new List<ObjTerminalElement>();
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    SetTerminalList(reader);
                }
                m_lstTerminalElement = m_lstNewTE;
            }
            catch (Exception ex)
            {
                sMsg = "端末テーブル参照に失敗(" + ex.ToString() + ")";
                MessageBox.Show(sMsg);
            }
        }
        public void ODBCSelecttUser(List<ObjMailElement> list)
        {
            string sSql;
            OdbcDataReader reader;
            string sMsg;

            if (m_conn == null)
            {
                return;
            }
            sSql = "SELECT * FROM tUser;";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    SetMailList(reader, list);
                }
            }
            catch (Exception ex)
            {
                sMsg = "ユーザーテーブル参照に失敗(" + ex.ToString() + ")";
                MessageBox.Show(sMsg);
            }
        }
        private Boolean checkNullTerminalRecord(OdbcDataReader reader)
        {
            if(reader.GetValue(0) == null)
            {
                return (true);
            }
            if(reader.GetValue(1) == null)
            {
                return (true);
            }
            if(reader.GetValue(9) == null)
            {
                return (true);
            }
            return (false);
        }
        private void SetTerminalList(OdbcDataReader reader)
        {
            string sLTid;
            List<ObjTerminalElement> list;
            int max, idx;
            ObjTerminalElement ote = new ObjTerminalElement();

            list = m_lstTerminalElement;
            ote.m_bCheck = true;
            sLTid = GetReaderString(reader, 0);
            ote.m_nId = m_libCmn.StrToInt(sLTid);
            ote.m_sName = GetReaderString(reader, 1);
            ote.m_sIpAdrs = GetReaderString(reader, 9);
            if (ote.m_sIpAdrs == "")
            {
                return;
            }
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (list[idx].m_nId == ote.m_nId)
                {
                    ote.m_bCheck = list[idx].m_bCheck;
                    break;
                }
            }
            m_lstNewTE.Add(ote);
        }
        // 
        private Boolean checkNullUserRecord(OdbcDataReader reader)
        {
            if(reader.GetValue(0) == null)
            {
                return (true);
            }
            if(reader.GetValue(1) == null)
            {
                return (true);
            }
            return (false);
        }
        private void SetMailList(OdbcDataReader reader, List<ObjMailElement> list)
        {
            string sLUid;
            int nLUid;
            string sName;
            Boolean bFlag;
            int max, idx;
            ObjMailElement ome;

            sLUid = GetReaderString(reader, 0);
            nLUid = m_libCmn.StrToInt(sLUid);
            sName = GetReaderString(reader, 1);
            bFlag = false;
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                if (nLUid == list[idx].m_nLUid)
                {
                    list[idx].m_sCName = sName;
                    bFlag = true;
                }
            }
            if (bFlag == false)
            {
                ome = new ObjMailElement();
                ome.m_nLUid = nLUid;
                ome.m_sCName = sName;
                list.Add(ome);
            }
        }
        public Boolean checkNullEnterRecord(OdbcDataReader reader)
        {
            if (reader.GetValue(0) == null)
            {
                return (true);
            }
            if (reader.GetValue(1) == null)
            {
                return (true);
            }
            if (reader.GetValue(2) == null)
            {
                return (true);
            }
            if (reader.GetValue(3) == null)
            {
                return (true);
            }
            if (reader.GetValue(4) == null)
            {
                return (true);
            }
            if (reader.GetValue(5) == null)
            {
                return (true);
            }
            if (reader.GetValue(9) == null)
            {
                return (true);
            }
            if (reader.GetValue(10) == null)
            {
                return (true);
            }
            return (false);
        }
        public ObjRecordElement SetSendMailRecordElement(OdbcDataReader reader)
        {
            ObjRecordElement objRecordElement;
            string sLUid;
            string sLTid;
            int nLTid;
            string sLUserType;
            string sLMode;

            objRecordElement = new ObjRecordElement();
            sLTid = GetReaderString(reader, 2);
            nLTid = m_libCmn.StrToInt(sLTid);
            objRecordElement.m_nLTid = nLTid;
            objRecordElement.m_sTName = GetTerminalName(nLTid);
            sLUid = GetReaderString(reader, 3);
            objRecordElement.m_nLUid = m_libCmn.StrToInt(sLUid);
            objRecordElement.m_sCName = GetReaderString(reader, 4);
            objRecordElement.m_sCDate = GetReaderString(reader, 0);
            objRecordElement.m_sCTime = GetReaderString(reader, 1);
            sLUserType = GetReaderString(reader, 9);
            objRecordElement.m_nLUserType = m_libCmn.StrToInt(sLUserType);
            objRecordElement.m_sCUnique = GetReaderString(reader, 5);
            sLMode = GetReaderString(reader, 10);
            objRecordElement.m_nLMode = m_libCmn.StrToInt(sLMode);
            return (objRecordElement);
        }
        private string GetReaderString(OdbcDataReader reader, int idx)
        {
            Type type;
            string sType;
            string str;

            if (DBNull.Value.Equals(reader.GetValue(idx)))
            {
                return ("");
            }
            type = reader.GetFieldType(idx);
            sType = type.Name;
            if (sType == "String")
            {
                str = reader.GetString(idx);
            }
            else if (sType == "Int32")
            {
                str = reader.GetInt32(idx).ToString();
            }
            else
            {
                str = "";
            }
            return (str);
        }
        // メールを送信
        public void SendMailLoop(ObjRecordElement record)
        {
            int nLUid;
            int nLTid;
            string sTName;
            string sCName;
            string sCDate;
            string sCTime;
            int nLUserType;
            string sCUnique;
            int nLMode;
            string sMsg;

            List<ObjMailElement> listOME;
            List<ObjAdminElement> listOAE;
            int max, idx;
            ObjMailElement ome;

            nLTid = record.m_nLTid;
            sTName = record.m_sTName;
            nLUid = record.m_nLUid;
            sCName = record.m_sCName;
            sCDate = record.m_sCDate;
            sCTime = record.m_sCTime;
            nLUserType = record.m_nLUserType;
            sCUnique = record.m_sCUnique;
            nLMode = record.m_nLMode;

            if (m_sBFDate == sCDate
            && m_sBFTime == sCTime 
            && m_nBFId == nLUid)
            {
                return;
            }
            m_sBFDate = sCDate;
            m_sBFTime = sCTime;
            m_nBFId = nLUid;

            if (nLUid == -1)
            {
                sMsg = sCDate.Substring(4, 2) + "月" + sCDate.Substring(6, 2) + "日 ";
                sMsg = sMsg + sCTime.Substring(0, 2) + "時" + sCTime.Substring(2, 2) + "分";
                sMsg = sMsg + " 認証失敗";
                if (m_objCrt != null)
                {
                    Type type = m_objCrt.GetType();
                    if (type == typeof(C100SendMailControl))
                    {
                        m_objSMC = (C100SendMailControl)m_objCrt;
                        m_objSMC.AddLstLog(sMsg);
                    }
                }
                return;
            }

            ome = new ObjMailElement();
            ome.m_sCName = sCName;
            ome.m_sCDateTime = sCDate + sCTime;
            ome.m_nLTid = nLTid;
            ome.m_sTName = sTName;
            SetLogInfo(ome, nLMode);

            if (nLMode == 1 || nLMode == 2 || nLMode == 3)
            {
                if (nLMode == 3)
                {
                    nLMode = m_nDefLMode;
                }
                else
                {
                    m_nDefLMode = nLMode;
                }
                listOME = GetMailElementList();
                max = listOME.Count;
                for (idx = 0; idx < max; idx++)
                {
                    if (listOME[idx].m_nLUid == nLUid)
                    {
                        listOME[idx].m_sCDateTime = sCDate + sCTime;
                        listOME[idx].m_nLTid = nLTid;
                        listOME[idx].m_sTName = sTName;
                        try
                        {
                            SendMail(listOME[idx], nLMode);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (GetAdminSend() == true)
                {
                    if (GetAdminSend() == true)
                    {
                        listOAE = GetAdminElementList();
                        max = listOAE.Count;
                        for (idx = 0; idx < max; idx++)
                        {
                            ome.m_sSendName = listOAE[idx].m_sName;
                            ome.m_sSendMail = listOAE[idx].m_sMail;
                            try
                            {
                               SendMail(ome, nLMode);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
        }
        private void SetLogInfo(ObjMailElement ome, int nLMode)
        {
            string sMsg;
            string sInfo;

            sInfo = "[MD][○○時○○分]";
            sInfo = AnlizeString(ome, sInfo);
            if (nLMode == 1)
            {
                sMsg = sInfo + "に" + ome.m_sCName + "さん到着";
            }
            else if(nLMode == 2)
            {
                sMsg = sInfo + "に" + ome.m_sCName + "さん帰宅";
            }
            else
            {
                sMsg = sInfo + "に" + ome.m_sCName + "さん認証";
            }
            if (m_objCrt != null)
            {
                Type type = m_objCrt.GetType();
                if (type == typeof(C100SendMailControl))
                {
                    m_objSMC = (C100SendMailControl)m_objCrt;
                    m_objSMC.AddLstLog(sMsg);
                }
            }
            DoDispatch();
        }
        private void SendMail(ObjMailElement ome, int nLMode)
        {
            string sMsg;
            string sFromMail;
            string sFromName;
            string sFromPassWord;
            string sToMail;
            string sToName;
            string sSubject;
            string sBody;

            DoDispatch();
            if (nLMode == 1)
            {
                sSubject = GetArrivalSubject();
                sSubject = AnlizeString(ome, sSubject);
                sBody = GetArrivalBody();
                sBody = AnlizeString(ome, sBody);
            }
            else if (nLMode == 2)
            {
                sSubject = GetRetHomeSubject();
                sSubject = AnlizeString(ome, sSubject);
                sBody = GetRetHomeBody();
                sBody = AnlizeString(ome, sBody);
            }
            else　// ファンクションが押されていないときのメール(nLMode = 0)
            {
                sSubject = "[受信者名]様へ認証のご案内";
                sSubject = AnlizeString(ome, sSubject);
                sBody = "[受信者名]様へ\n[ユーザ名]さんは[西暦年月日]に認証されました";
                sBody = AnlizeString(ome, sBody);
            }

            sFromName = GetServerName();
            sFromMail = GetServerMail();
            sFromPassWord = GetServerPassWord();

            sToMail = ome.m_sSendMail;
            sToName = ome.m_sSendName + "様";

            string url;

            //url = "http://www.unismail.net/sendmail.php";
            url = "http://www.hurrymulti.com/unis/sendmail.php";

            HttpClient httpClient = new HttpClient();
            var aryPair = new Dictionary<string, string>();
            aryPair["tomail"] = sToMail;
            aryPair["toname"] = sToName;
            aryPair["frommail"] = sFromMail;
            aryPair["fromname"] = sFromName;
            aryPair["subject"] = sSubject;
            aryPair["body"] = sBody;
            var content = new FormUrlEncodedContent(aryPair);
            Task<HttpResponseMessage> response = httpClient.PostAsync(url, content);
            response.Wait();
            String sRet = response.Result.Content.ReadAsStringAsync().Result;
            String[] sAry = sRet.Split(',');
            if (sAry[0] == "0")
            {
                sMsg = sSubject + "メール送信失敗(" + sToName + ")";
                if (m_objCrt != null)
                {
                    Type type = m_objCrt.GetType();
                    if (type == typeof(C100SendMailControl))
                    {
                        m_objSMC = (C100SendMailControl)m_objCrt;
                        m_objSMC.AddLstLog(sMsg);
                    }
                }
            }
        }
    }
}

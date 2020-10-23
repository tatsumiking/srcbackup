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

namespace CsvOut
{
    public partial class MainWindow : Window
    {
        private string m_sUnisDBPath;
        private OdbcConnection m_conn = null;
        private OdbcCommand m_com;

        private int m_nLastDate;
        private int m_nLastTime;

        public void MainWindowODBCInit()
        {
            string sPath;

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
        public Boolean ODBCOpenUnisDB()
        {
            Boolean bFlag;
            string sMsg;
            int idx, max;

            m_lstCsvStr = new List<string>();
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
                return (false);
                /*
                sMsg = "データーベースに接続できません。";
                m_conn.Close();
                MessageBox.Show(sMsg);
                m_conn = null;
                */
            }
            return (true);
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
        public void ODBCSelecttEnter()
        {
            string sSql;
            OdbcDataReader reader;
            string sCsvStr;

            sSql = "SELECT * FROM tEnter WHERE ((StrComp(C_Date,'" + m_sBaseDate + "') = 0)AND(StrComp(C_Time, '" + m_sBaseTime + "') >= 0));";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    sCsvStr = PicupCsvStrRecordElement(reader);
                    m_lstCsvStr.Add(sCsvStr);
                }

            }
            catch (Exception ex)
            {
                return;
            }
            // 23時59分59秒より後に発生した入力を処理
            sSql = "SELECT * FROM tEnter WHERE (StrComp(C_Date,'" + m_sBaseDate + "') > 0);";
            m_com = new OdbcCommand(sSql, m_conn);
            try
            {
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    sCsvStr = PicupCsvStrRecordElement(reader);
                    m_lstCsvStr.Add(sCsvStr);
                }
            }
            catch (Exception ex)
            {
                return;
            }
            return;
        }
        public String PicupCsvStrRecordElement(OdbcDataReader reader)
        {
            String strRet;
            string sLTid;
            int nLTid;
            string sLMode;
            int nLMode;
            string sDate;
            string sTime;
            int nDate;
            int nTime;
            string sLUid;

            sDate = GetReaderString(reader, 0);
            sTime = GetReaderString(reader, 1);
            sLTid = GetReaderString(reader, 2);
            sLUid = GetReaderString(reader, 3);
            sLMode = GetReaderString(reader, 10);
            nDate = m_libCmn.StrToInt(sDate);
            nTime = m_libCmn.StrToInt(sTime);
            nLTid = m_libCmn.StrToInt(sLTid);
            nLMode = m_libCmn.StrToInt(sLMode);
            if (m_nLastDate < nDate)
            {
                m_nLastDate = nDate;
                m_nLastTime = 0;
                m_sLastDate = sDate;
                m_sLastTime = "000000";
            }
            else if (m_nLastDate == nDate)
            {
                if (m_nLastTime < nTime)
                {
                    m_nLastTime = nTime;
                    m_sLastTime = sTime;
                }
            }
            if (sDate.Length < 8)
            {
                sDate = "00000000";
            }
            if (sTime.Length < 8)
            {
                sTime = "000000";
            }
            strRet = "";
            strRet = strRet + sLUid + ",";
            strRet = strRet + sDate.Substring(0, 4) + "/" + sDate.Substring(4, 2) + "/" + sDate.Substring(6, 2) + ",";
            strRet = strRet + sTime.Substring(0, 2) + ":" + sDate.Substring(2, 2) + ",";
            if (nLMode == 1)
            {
                strRet = strRet + "出勤" + ",0,\n";
            }
            else
            {
                strRet = strRet + "退勤" + ",0,\n";
            }
            return (strRet);
        }
        private string GetReaderString(OdbcDataReader reader, int idx)
        {
            Type type;
            string sType;
            string str;

            type = reader.GetFieldType(idx);
            sType = type.Name;
            if(sType == "String"){
                str = reader.GetString(idx); 
            }else if(sType == "Int32"){
                str = reader.GetInt32(idx).ToString();
            }
            else
            {
                str = "";
            }
            return(str);
        }
    }
}

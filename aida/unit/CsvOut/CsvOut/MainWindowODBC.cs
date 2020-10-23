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
        private string m_sUnisDBFile;
        private OdbcConnection m_conn = null;
        private OdbcCommand m_com;

        private int m_nLastDate;
        private int m_nLastTime;

        public void MainWindowODBCInit()
        {
            m_sUnisDBFile = m_sUnisDBPath+"\\unis.mdb";
        }
        public string GetODBCDBPath()
        {
            return (m_sUnisDBFile);
        }
        public Boolean ODBCOpenUnisDB()
        {
            Boolean bFlag;
            int idx, max;

            m_lstCsvStr = new List<string>();
            bFlag = false;
            max = 10;
            m_conn = new OdbcConnection();
            m_conn.ConnectionString = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + m_sUnisDBFile + ";PWD=unisamho";
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
                App.LogOut("データーベースに接続できません。");
                return (false);
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
            string sCheckDate;
            string sCheckTime;
            string sDate;
            string sTime;

            sCheckDate = m_sCheckTime.Substring(0, 8);
            sCheckTime = m_sCheckTime.Substring(8, 6);
            if (m_sBaseDate == sCheckDate)
            {
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique";
                sSql = sSql + " FROM tEnter";
                sSql = sSql + " WHERE (";
                sSql = sSql + "(StrComp(C_Date,'" + m_sBaseDate + "') = 0)";
                sSql = sSql + "AND(StrComp(C_Time, '" + m_sBaseTime + "00') >= 0)";
                sSql = sSql + "AND(StrComp(C_Time, '" + sCheckTime + "00') < 0)";
                sSql = sSql + ");";
                m_com = new OdbcCommand(sSql, m_conn);
                try
                {
                    reader = m_com.ExecuteReader();
                    while (reader.Read())
                    {
                        sDate = GetReaderString(reader, 0);
                        sTime = GetReaderString(reader, 1);
                        UpdateLastDateTime(reader);
                        sCsvStr = PicupCsvStrRecordElement(reader);
                        m_lstCsvStr.Add(sCsvStr);
                    }

                }
                catch (Exception ex)
                {
                    App.LogOut(ex.ToString());
                    return;
                }

            }
            else
            {
                // 最後に集計した日の集計時間以降に発生したデータを取得
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique";
                sSql = sSql + " FROM tEnter";
                sSql = sSql + " WHERE (";
                sSql = sSql + "(StrComp(C_Date,'" + m_sBaseDate + "') = 0)";
                sSql = sSql + "AND(StrComp(C_Time, '" + m_sBaseTime + "00') >= 0)";
                sSql = sSql + ");";
                m_com = new OdbcCommand(sSql, m_conn);
                try
                {
                    reader = m_com.ExecuteReader();
                    while (reader.Read())
                    {
                        sDate = GetReaderString(reader, 0);
                        sTime = GetReaderString(reader, 1);
                        UpdateLastDateTime(reader);
                        sCsvStr = PicupCsvStrRecordElement(reader);
                        m_lstCsvStr.Add(sCsvStr);
                    }

                }
                catch (Exception ex)
                {
                    App.LogOut(ex.ToString());
                    return;
                }
                // 最後に集計した日以降で締め日前までに発生したデータを取得
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique";
                sSql = sSql + " FROM tEnter";
                sSql = sSql + " WHERE (";
                sSql = sSql + "(StrComp(C_Date,'" + m_sBaseDate + "') > 0)";
                sSql = sSql + "AND (StrComp(C_Date,'" + sCheckDate + "') < 0)";
                sSql = sSql + ");";
                m_com = new OdbcCommand(sSql, m_conn);
                try
                {
                    reader = m_com.ExecuteReader();
                    while (reader.Read())
                    {
                        sDate = GetReaderString(reader, 0);
                        sTime = GetReaderString(reader, 1);
                        UpdateLastDateTime(reader);
                        sCsvStr = PicupCsvStrRecordElement(reader);
                        m_lstCsvStr.Add(sCsvStr);
                    }
                }
                catch (Exception ex)
                {
                    App.LogOut(ex.ToString());
                    return;
                }
                // 最後に集計した日以降で締め日前までに発生したデータを取得
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique";
                sSql = sSql + " FROM tEnter";
                sSql = sSql + " WHERE (";
                sSql = sSql + "(StrComp(C_Date,'" + sCheckDate + "') = 0)";
                sSql = sSql + "AND (StrComp(C_Time,'" + sCheckTime + "') < 0)";
                sSql = sSql + ");";
                m_com = new OdbcCommand(sSql, m_conn);
                try
                {
                    reader = m_com.ExecuteReader();
                    while (reader.Read())
                    {
                        sDate = GetReaderString(reader, 0);
                        sTime = GetReaderString(reader, 1);
                        UpdateLastDateTime(reader);
                        sCsvStr = PicupCsvStrRecordElement(reader);
                        m_lstCsvStr.Add(sCsvStr);
                    }
                }
                catch (Exception ex)
                {
                    App.LogOut(ex.ToString());
                    return;
                }
            }
            return;
        }
        private void UpdateLastDateTime(OdbcDataReader reader)
        {
            string sDate;
            string sTime;
            int nDate;
            int nTime;

            sDate = GetReaderString(reader, 0);
            sTime = GetReaderString(reader, 1);
            nDate = m_libCmn.StrToInt(sDate);
            nTime = m_libCmn.StrToInt(sTime);
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
        }
        public String PicupCsvStrRecordElement(OdbcDataReader reader)
        {
            string strRet;
            int fldmax, fldidx;
            string key;
            int fldno;
            string sStr, sTmp;
            int nLMode;

            strRet = "";
            fldmax = m_aryFildKeyTbl.Length;
            for (fldidx = 0; fldidx < fldmax; fldidx++)
            {
                key = m_aryFildKeyTbl[fldidx];
                fldno = CnvOdbcKeyToIndex(key);
                if (fldno == -1)
                {
                    sStr = key;
                }
                else
                {
                    sStr = GetReaderString(reader, fldno);
                }
                if (fldno == 0)
                {
                    if (sStr.Length < 8)
                    {
                        sStr = "00000000";
                    }
                    sTmp = sStr.Substring(0, 4) + "/" + sStr.Substring(4, 2) + "/" + sStr.Substring(6, 2);
                    sStr = sTmp;
                }
                else if (fldno == 1)
                {
                    if (sStr.Length < 6)
                    {
                        sStr = "0000";
                    }
                    sTmp = sStr.Substring(0, 2) + ":" + sStr.Substring(2, 2);
                    sStr = sTmp;
                }
                else if (fldno == 5)
                {
                    nLMode = m_libCmn.StrToInt(sStr);
                    if (0 <= nLMode && nLMode <= m_aryFucStrTbl.Length)
                    {
                        sStr = m_aryFucStrTbl[nLMode-1];
                    }
                }
                strRet = strRet + sStr + m_sDelimiter;
            }
            strRet = strRet + "\n"; 
            return (strRet);
        }
        private string GetReaderString(OdbcDataReader reader, int idx)
        {
            Type type;
            string sType;
            string str;

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
        private int CnvOdbcKeyToIndex(string key)
        {
            if (key == "%C_Date%")
            {
                return (0);
            }
            else if (key == "%C_Time%")
            {
                return (1);
            }
            else if (key == "%L_TID%")
            {
                return (2);
            }
            else if (key == "%L_UID%")
            {
                return (3);
            }
            else if (key == "%C_Name%")
            {
                return (4);
            }
            else if (key == "%L_Mode%")
            {
                return (5);
            }
            else if (key == "%C_OCODE%")
            {
                return (6);
            }
            return (-1);
        }
    }
}

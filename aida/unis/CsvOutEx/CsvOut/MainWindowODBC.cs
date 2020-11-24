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
            try
            {
                m_sUnisDBFile = m_sUnisDBPath+"\\unis.mdb";
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("MainWindowODBCInit");
                App.LogOut(ex.ToString());
            }
        }
        public string GetODBCDBPath()
        {
            return (m_sUnisDBFile);
        }
        public Boolean ODBCOpenUnisDB()
        {
            Boolean bFlag;
            int idx, max;

            try
            {
                m_lstCsvStr = new List<string>();
                m_lstErrorStr = new List<string>();
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
                    catch (Exception ex1)
                    {
                        App.ErrorLogAppend(m_sUnisDBFile+"　Connect");
                        App.LogOut(ex1.ToString());
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("ODBCOpenUnisDB");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        public void DoDispatch()
        {
            try
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
                Dispatcher.PushFrame(frame);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("DoDispatch");
                App.LogOut(ex.ToString());
            }
        }
        public object ExitFrames(object frames)
        {
            try
            {
                ((DispatcherFrame)frames).Continue = false;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("ExitFrames");
                App.LogOut(ex.ToString());
            }
            return null;
        }
        public void ODBCCloseUnisDB()
        {
            try
            {
                if(m_conn == null){
                    return;
                }
                m_conn.Close();
                m_conn = null;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("ODBCCloseUnisDB");
                App.LogOut(ex.ToString());
            }
        }
        public void ODBCSelecttEnterAll()
        {
            string sSql;
            OdbcDataReader reader;
            string sCsvStr;
            string sDate;
            string sTime;

            sSql = "";
            try
            {
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique,L_Result";
                sSql = sSql + " FROM tEnter";
                m_com = new OdbcCommand(sSql, m_conn);
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    sDate = GetReaderString(reader, 0);
                    sTime = GetReaderString(reader, 1);
                    sCsvStr = PicupCsvStrRecordElement(reader);
                }

            }
            catch (Exception ex)
            {
                App.ErrorLogAppend(sSql + " ODBCSelecttEnterAll");
                App.LogOut(ex.ToString());
            }

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
            int max;
            System.Diagnostics.Stopwatch sw;
            TimeSpan ts;
            string str;

            sSql = "";
            try
            {
                sCheckDate = m_sCheckTime.Substring(0, 8);
                sCheckTime = m_sCheckTime.Substring(8, 6);
                sSql = "SELECT ";
                sSql = sSql + "C_Date,C_Time,L_TID,L_UID";
                sSql = sSql + ",C_Name,L_Mode,C_Unique,L_Result";
                sSql = sSql + " FROM tEnter";
                sSql = sSql + " WHERE (";
                sSql = sSql + " (StrComp(C_Date+C_Time,'" + m_sBaseDate + m_sBaseTime + "') >= 0)";
                sSql = sSql + " AND (StrComp(C_Date+C_Time,'" + sCheckDate + sCheckTime + "') < 0)";
                sSql = sSql + ");";
                m_com = new OdbcCommand(sSql, m_conn);
                reader = m_com.ExecuteReader();
                while (reader.Read())
                {
                    sDate = GetReaderString(reader, 0);
                    sTime = GetReaderString(reader, 1);
                    UpdateLastDateTime(reader);
                    if (checkRecordError(reader) == false)
                    {
                        sCsvStr = PicupCsvStrRecordElement(reader);
                        m_lstCsvStr.Add(sCsvStr);
                    }
                }
                max = m_lstCsvStr.Count;
                if (max < 10000){
                    SortListCsv();
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend(sSql + "　ODBCSelecttEnter");
                App.LogOut(ex.ToString());
            }
        }
        private Boolean checkRecordError(OdbcDataReader reader)
        {
            string sStr, sName;
            int uid, mode, result;
            string sError;

            try
            {
                sStr = GetReaderString(reader, 3);
                uid = m_libCmn.StrToInt(sStr);
                sName = GetReaderString(reader, 4);
                sStr = GetReaderString(reader, 5);
                mode = m_libCmn.StrToInt(sStr);
                sStr = GetReaderString(reader, 7);
                result = m_libCmn.StrToInt(sStr);
                if (uid != -1 && result == 0 && (mode == 1 || mode == 2 || mode == 4 || mode == 5))
                {
                    return (false);
                }
                sStr = GetReaderString(reader, 0);
                sError = sStr.Substring(0, 4) + "/" + sStr.Substring(4, 2) + "/" + sStr.Substring(6, 2);
                sError = sError + m_sDelimiter;
                sStr = GetReaderString(reader, 0);
                sError = sError + sStr.Substring(0, 2) + ":" + sStr.Substring(2, 2) + ":" + sStr.Substring(4, 2);
                sError = sError + m_sDelimiter + "L_UID=" + uid + m_sDelimiter + "C_Name=" + sName;
                sError = sError + m_sDelimiter + "L_Mode=" + mode + "L_Result=" + result;
                m_lstErrorStr.Add(sError);
                return (true);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("checkRecordError");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        private void SortListCsv()
        {
            int max, i, j;
            string sCsvTmp;
            string[] ary;
            string sDateI, sTimeI;
            string sDateJ, sTimeJ;
            string sDateTmp, sTimeTmp;

            try
            {
                max = m_lstCsvStr.Count;
                for (i = 0; i < max - 1; i++)
                {
                    ary = m_lstCsvStr[i].Split(',');
                    sDateI = ary[1];
                    sTimeI = ary[2];
                    for (j = i; j < max; j++)
                    {
                        ary = m_lstCsvStr[j].Split(',');
                        sDateJ = ary[1];
                        sTimeJ = ary[2];
                        if (sDateI.CompareTo(sDateJ) > 0)
                        {
                            sCsvTmp = m_lstCsvStr[i];
                            m_lstCsvStr[i] = m_lstCsvStr[j];
                            m_lstCsvStr[j] = sCsvTmp;

                            sDateTmp = sDateI;
                            sDateI = sDateJ;
                            sDateJ = sDateTmp;

                            sTimeTmp = sTimeI;
                            sTimeI = sTimeJ;
                            sTimeJ = sTimeTmp;
                        }
                        else if (sDateI == sDateJ && sTimeI.CompareTo(sTimeJ) > 0)
                        {
                            sCsvTmp = m_lstCsvStr[i];
                            m_lstCsvStr[i] = m_lstCsvStr[j];
                            m_lstCsvStr[j] = sCsvTmp;

                            sDateTmp = sDateI;
                            sDateI = sDateJ;
                            sDateJ = sDateTmp;

                            sTimeTmp = sTimeI;
                            sTimeI = sTimeJ;
                            sTimeJ = sTimeTmp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("SortListCsv");
                App.LogOut(ex.ToString());
            }
        }
        private void UpdateLastDateTime(OdbcDataReader reader)
        {
            string sDate;
            string sTime;
            int nDate;
            int nTime;

            try
            {
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
            catch (Exception ex)
            {
                App.ErrorLogAppend("UpdateLastDateTime");
                App.LogOut(ex.ToString());
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
            int nTNo;

            strRet = "";
            try
            {
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
                        if (m_sDelimiter != "")
                        {
                            sTmp = sStr.Substring(0, 4) + "/" + sStr.Substring(4, 2) + "/" + sStr.Substring(6, 2);
                            sStr = sTmp;
                        }
                    }
                    else if (fldno == 1)
                    {
                        if (sStr.Length < 6)
                        {
                            sStr = "0000";
                        }
                        if (m_sDelimiter != "")
                        {
                            sTmp = sStr.Substring(0, 2) + ":" + sStr.Substring(2, 2);
                            sStr = sTmp;
                        }
                    }
                    else if (fldno == 2)
                    {
                        if (m_sDelimiter == "")
                        {
                            nTNo = m_libCmn.StrToInt(sStr);
                            sStr = nTNo.ToString("00");
                        }
                    }
                    else if (fldno == 5)
                    {
                        nLMode = m_libCmn.StrToInt(sStr);
                        if (nLMode == 1)
                        {
                            sStr = m_aryFucStrTbl[0];
                        }
                        else if (nLMode == 2)
                        {
                            sStr = m_aryFucStrTbl[1];
                        }
                        else if (nLMode == 3)
                        {
                            if (m_sDelimiter != "")
                            {
                                sStr = "その他";
                            }
                            else
                            {
                                sStr = "00";
                            }
                        }
                        else if (nLMode == 4)
                        {
                            sStr = m_aryFucStrTbl[2];
                        }
                        else if (nLMode == 5)
                        {
                            sStr = m_aryFucStrTbl[3];
                        }
                    }
                    strRet = strRet + sStr + ",";
                }
                //strRet = strRet + "\r\n";
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("PicupCsvStrRecordElement");
                App.LogOut(ex.ToString());
            }
            return (strRet);
        }
        private string GetReaderString(OdbcDataReader reader, int idx)
        {
            Type type;
            string sType;
            string str;

            str = "";
            try
            {
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
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("GetReaderString");
                App.LogOut(ex.ToString());
            }
            return (str);
        }
        private int CnvOdbcKeyToIndex(string key)
        {
            try
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
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("CnvOdbcKeyToIndex");
                App.LogOut(ex.ToString());
            }
            return (-1);
        }
    }
}

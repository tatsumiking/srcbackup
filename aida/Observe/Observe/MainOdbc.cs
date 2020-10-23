using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Observe
{
    public partial class MainWindow : Window
    {
        private string m_sOdbcCrtMdbName;
        private int m_nOdbcCrtNo;

        public void odbcLoadEnv()
        {
            string sFileName;
            string[] aryLine;

            sFileName = m_sEnvPath + "\\mdb.env";
            aryLine = m_libCmn.LoadFileLineSJIS(sFileName);
            if (3 <= aryLine.Length)
            {
                m_sOdbcCrtMdbName = aryLine[1];
                m_nOdbcCrtNo = m_libCmn.StrToInt(aryLine[2]);
            }
        }
        public void odbcSaveEnv()
        {
            string sFileName;
            string sData;

            sData = "//\n";
            sData = sData + m_sOdbcCrtMdbName + "\n";
            sData = sData + m_nOdbcCrtNo + "\n";
            sFileName = m_sEnvPath + "\\mdb.env";
            m_libCmn.SaveFileSJIS(sFileName, sData);
        }
        public void checkIO6Event()
        {
            OdbcConnection con;
            string sSql;
            OdbcCommand com;
            OdbcDataReader reader;
            string sMsg;
            string sDateTime;

            // m_nCrtNo = 20;
            // m_sDateTime = "2018-07-04 09:45:27";
            // DateTime dt = new DateTime(2018,07,04,9,45,00);
            // SELECT count(Id) FROM hoge WHERE Created = { ts '1985-01-01 00:00:00'};
            con = openMdb();
            if (con != null)
            {
                sSql = "SELECT ";
                sSql = sSql + "NUM, MESSAGE, MESSAGETIME";
                sSql = sSql + " FROM EVENT";
                sSql = sSql + " WHERE (NUM > " + m_nOdbcCrtNo + ");";
                com = new OdbcCommand(sSql, con);
                try
                {
                    reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        m_nOdbcCrtNo = reader.GetInt32(0);
                        sMsg = GetReaderString(reader, 1);
                        if (sMsg == "I/Oモージュル 1 - インプット 6 作動検知")
                        {
                            odbcSaveEnv();
                            setCheckMdbFlag(false);
                        }
                        sDateTime = GetReaderString(reader, 2);
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
                closeMdb(con);
                odbcSaveEnv();
            }
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
            else if (sType == "DateTime")
            {
                str = reader.GetString(idx);
            }
            else
            {
                str = "";
            }
            return (str);
        }
        private OdbcConnection openMdb()
        {
            string sMdbName;
            string sCnct;
            OdbcConnection con;

            DateTime dt = DateTime.Now;
            sMdbName = "E"+dt.ToString("yyyyMMdd")+".mdb";
            //sMdbName = "E20180704.mdb";
            if (m_sOdbcCrtMdbName != sMdbName)
            {
                m_sOdbcCrtMdbName = sMdbName;
                m_nOdbcCrtNo = 0;
                odbcSaveEnv();
            }
            try
            {
                sCnct = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + m_sMdbPath + "\\" + sMdbName;
                con = new OdbcConnection(sCnct);
                con.Open();
            }
            catch (Exception ex)
            {
                return (null);
            }
            return (con);
        }
        private string execSqlNoError(OdbcConnection con, string sSql)
        {
            OdbcCommand cmd;
            try
            {
                cmd = new OdbcCommand(sSql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                return ("0");
            }
            return ("1");
        }
        private string execSql(OdbcConnection con, string sSql)
        {
            OdbcCommand cmd;
            try
            {
                cmd = new OdbcCommand(sSql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "確認", MessageBoxButton.OK);
                return ("0");
            }
            return ("1");
        }
        private string closeMdb(OdbcConnection con)
        {
            try
            {
                con.Close();
                con.Dispose();
            }
            catch (Exception ex)
            {
                return ("0");
            }
            return ("1");
        }
        private string getReaderString(OdbcDataReader reader, int idx)
        {
            Type type;
            string sType;
            string str;

            if (reader.IsDBNull(idx) == true)
            {
                str = "";
            }
            else
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
                else
                {
                    str = "";
                }
            }
            return (str);
        }

    }
}

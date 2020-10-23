using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Observe
{
    public class LibOdbc
    {
        public LibCommon m_libCmn;
        public string m_sExecPath;
        public string m_sEnvPath;
        private string m_sMsg;

        public void setLibCommonClass(LibCommon libCmn)
        {
            m_libCmn = libCmn;
        }
        public void setExecPath(string sExecPath)
        {
            m_sExecPath = sExecPath;
        }
        public void setEnvPath(string sEnvPath)
        {
            m_sEnvPath = sEnvPath;
        }
        public Boolean createAdrsLatLndTable()
        {
            OdbcConnection con;
            string sSql;

            con = openMdb();
            if (con == null)
            {
                return (false);
            }
            sSql = "DROP TABLE adrslatlnd;";
            execSqlNoError(con, sSql);

            sSql = "CREATE TABLE adrslatlnd (id AUTOINCREMENT PRIMARY KEY";
            sSql = sSql + ", address VARCHAR(96)"; // 区市町村丁目
            sSql = sSql + ", lat DOUBLE"; // 緯度
            sSql = sSql + ", lnd DOUBLE"; // 経度
            sSql = sSql + ");";
            execSql(con, sSql);

            closeMdb(con);
            return (true);
        }
        public void initAdrsLatLndTable(string[] sLine)
        {
            OdbcConnection con;
            string sSql, sInsSql;
            int idx, max;
            string[] sAry;
            string sData;

            con = openMdb();
            if (con != null)
            {
                sSql = "INSERT INTO adrslatlnd (";
                sSql = sSql + "address";
                sSql = sSql + ",lat";
                sSql = sSql + ",lnd";
                sSql = sSql + ") VALUES (";

                max = sLine.Length;
                for (idx = 1; idx < max; idx++)
                {
                    sAry = sLine[idx].Split(',');
                    if (sAry.Length == 4)
                    {
                        sData = "'" + sAry[0] + sAry[1] + "'";
                        sData = sData + "," + sAry[2] + "," + sAry[3];
                        sInsSql = sSql + sData + ");";
                        execSql(con, sInsSql);
                    }
                }
                closeMdb(con);
            }
        }
        public OdbcConnection openMdb()
        {
            string sCnct;
            OdbcConnection con;

            try
            {
                sCnct = "Driver={Microsoft Access Driver (*.mdb)};DBQ=" + m_sEnvPath + "\\adrslatlnd.mdb";
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
                return ("0");
            }
            return ("1");
        }
        public string closeMdb(OdbcConnection con)
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
        public string getReaderString(OdbcDataReader reader, int idx)
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

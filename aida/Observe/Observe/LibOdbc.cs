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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CsvOut
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static string m_sArgv = "";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
                return;
            m_sArgv = e.Args[0];
        }
        public static void LogOut(string msg)
        {
            if (m_sArgv != "")
            {
                string sFileName = "c:\\ProgramData\\csvout\\csvoutlog.txt";
                System.IO.File.AppendAllText(sFileName, msg+"\r\n");
            }
        }
        // 正常に出力された勤怠データの件数ログ
        public static void OutLogAppend(string msg)
        {
            DateTime dt;
            string sYYYY;
            string sHead;
            string sFileName;

            dt = DateTime.Now;
            sYYYY = dt.ToString("yyyy");
            sFileName = "c:\\UsesProgram\\csvout\\log\\" + sYYYY + "OutDataCount.log";
            if (!File.Exists(sFileName))
            {
                System.IO.FileStream fs = System.IO.File.Create(sFileName);
                fs.Close();
                fs.Dispose();
            }
            sHead = "NORMAL," + dt.ToString("yyyy/mm/dd hh:MM:ss") + ",";
            System.IO.File.AppendAllText(sFileName, sHead + msg + "\r\n");
        }
        // 通常のExceptionを伴うエラーログ
        public static void ErrorLogAppend(string msg)
        {
            DateTime dt;
            string sYYYY;
            string sHead;
            string sFileName;

            dt = DateTime.Now;
            sYYYY = dt.ToString("yyyy");
            sFileName = "c:\\UsesProgram\\csvout\\log\\" + sYYYY + "ExceptionError.log";
            if (!File.Exists(sFileName))
            {
                System.IO.FileStream fs = System.IO.File.Create(sFileName);
                fs.Close();
                fs.Dispose();
            }
            sHead = "ERROR," + dt.ToString("yyyy/mm/dd hh:MM:ss") + ",";
            System.IO.File.AppendAllText(sFileName, sHead+msg + "\r\n");
        }
        // 勤怠エラーデータ保存ログファイル
        public static void ErrorDataAppend(string msg)
        {
            DateTime dt;
            string sYYYY;
            string sHead;
            string sFileName;

            dt = DateTime.Now;
            sYYYY = dt.ToString("yyyy");
            sFileName = "c:\\UsesProgram\\csvout\\log\\" + sYYYY + "ErrorData.log";
            if (!File.Exists(sFileName))
            {
                System.IO.FileStream fs = System.IO.File.Create(sFileName);
                fs.Close();
                fs.Dispose();
            }
            sHead = "ERROR," + dt.ToString("yyyy/mm/dd hh:MM:ss") + ",";
            System.IO.File.AppendAllText(sFileName, sHead+msg + "\r\n");
        }
    }
}

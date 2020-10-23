using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CsvOut
{
    public partial class MainWindow : Window
    {
        private string InitExePath()
        {
            string path;

            try
            {
                Assembly myAssembly = Assembly.GetEntryAssembly();
                path = myAssembly.Location;
                return (path.Substring(0, path.LastIndexOf("\\")));
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitExePath ERROR");
                App.LogOut(ex.ToString());
                return ("");
            }
        }
        private string InitEnvPath()
        {
            string sEnvPath;
            string sCsvOutEnvPath;
            string sLogPath;

            sCsvOutEnvPath = "";
            try
            {
                sEnvPath = "c:\\UsesProgram";
                if (!Directory.Exists(sEnvPath))
                {
                    Directory.CreateDirectory(sEnvPath);
                    sCsvOutEnvPath = sEnvPath + "\\" + m_sProgName;
                    Directory.CreateDirectory(sCsvOutEnvPath);
                    sLogPath = sEnvPath + "\\" + m_sProgName + "\\log";
                    Directory.CreateDirectory(sLogPath);

                }
                else
                {
                    sCsvOutEnvPath = sEnvPath + "\\" + m_sProgName;
                    if (!Directory.Exists(sCsvOutEnvPath))
                    {
                        Directory.CreateDirectory(sCsvOutEnvPath);
                        sLogPath = sEnvPath + "\\" + m_sProgName + "\\log";
                        Directory.CreateDirectory(sLogPath);
                    }
                    else
                    {
                        sLogPath = sEnvPath + "\\" + m_sProgName + "\\log";
                        if (!Directory.Exists(sLogPath))
                        {
                            Directory.CreateDirectory(sLogPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("InitEnvPath");
                App.LogOut(ex.ToString());
            }
            return (sCsvOutEnvPath);
        }
    }
}

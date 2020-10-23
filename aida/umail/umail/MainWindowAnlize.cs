using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace umail
{
    public partial class MainWindow : Window
    {
        private string AnlizeString(ObjMailElement ome, String sSrcStr)
        {
            string retStr;
            string cmdStr;
            string[] aryVal;
            int max, i;
            Boolean flag;

            flag = false;
            cmdStr = "";
            retStr = "";
            max = sSrcStr.Length;
            for (i = 0; i < max; i++)
            {
                if(sSrcStr[i] == '['){
                    flag = true;
                    cmdStr = "";
                }
                else if (sSrcStr[i] == ']')
                {
                    if (flag == true)
                    {
                        retStr = retStr + AnlizeCommand(ome, cmdStr);
                    }
                    flag = false;
                }else{
                    if(flag == true){
                        cmdStr = cmdStr + sSrcStr[i];
                    }else{
                        retStr = retStr + sSrcStr[i];
                    }
                }
            }
            return (retStr);
        }
        private string AnlizeCommand(ObjMailElement ome, String sCommand)
        {
            string sYear;
            string sMonth;
            string sDay;
            string sHour;
            string sMinute;
            string sSecond;
            string sGG;

            sYear = ome.m_sCDateTime.Substring(0, 4);
            sMonth = ome.m_sCDateTime.Substring(4, 2);
            sDay = ome.m_sCDateTime.Substring(6, 2);
            sHour = ome.m_sCDateTime.Substring(8, 2);
            sMinute = ome.m_sCDateTime.Substring(10, 2);
            sSecond = ome.m_sCDateTime.Substring(12, 2);
            if (sCommand == "ターミナルID")
            {
                return (ome.m_nLTid.ToString());
            }
            else if (sCommand == "ターミナル名")
            {
                return (ome.m_sTName);
            }
            if (sCommand == "ユーザID")
            {
                return (ome.m_nLUid.ToString());
            }
            else if (sCommand == "ユーザ名")
            {
                return (ome.m_sCName);
            }
            else if (sCommand == "受信者名")
            {
                return (ome.m_sSendName);
            }
            else if (sCommand == "受信者メール")
            {
                return (ome.m_sSendMail);
            }
            else if (sCommand == "西暦年月日")
            {
                return (sYear + "年" + sMonth + "月" + sDay + "日");
            }
            else if (sCommand == "和暦年月日")
            {
                sGG = AnlizeNengou(sYear);
                return (sGG + "年" + sMonth + "月" + sDay + "日");
            }
            else if (sCommand == "MD")
            {
                return (sMonth + "月" + sDay + "日");
            }
            else if (sCommand == "○○時○○分")
            {
                return (sHour + "時" + sMinute + "分");
            }
            else if (sCommand == "○○：○○")
            {
                return (sHour + "：" + sMinute);
            }
            else if (sCommand == "hhmmss")
            {
                return (sHour + "：" + sMinute + ":" + sSecond);
            }
            return ("");
        }
        private string AnlizeNengou(String sYear)
        {
            int nYear;
            int nGG;

            nYear = Int32.Parse(sYear);
            if (1989 < nYear)
            {
                nGG = nYear - 1989 + 1;
                return ("平成" + nGG);
            }
            return (sYear);
        }
    }

}

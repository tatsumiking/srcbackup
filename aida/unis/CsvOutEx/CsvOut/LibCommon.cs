using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;
using System.Xml;

namespace CsvOut
{
    public class Constants
    {
        public const int USBPROTECTNO = 315;
        public const int NUMMAX = 1000000;
    }
    public class ClsFontGlyph
    {
        public string str;
        public double fontsize;
        public ClsMatrix mtrx;
        public PathGeometry pg;
    }
    public class ClsMatrix
    {
        public double xsize, ysize;
        public double a11, a12, a13;
        public double a21, a22, a23;

        public ClsMatrix()
        {
            InitMatrix();
        }

        public void InitMatrix()
        {
            a11 = 1.0; a12 = 0.0; a13 = 0.0;
            a21 = 0.0; a22 = 1.0; a23 = 0.0;
        }
        public Point TrnsPoint(Point pt)
        {
            double x, y;

            x = pt.X-xsize/2;
            y = pt.Y+ysize/2;
            pt.X = x * a11 + y * a12 + a13 + xsize/2;
            pt.Y = x * a21 + y * a22 + a23 - ysize/2;
            return (pt);
        }
    }
    public class ClsArea
    {
        public Boolean flag;
        public double sx;
        public double sy;
        public double ex;
        public double ey;
        public double sizex;
        public double sizey;
        
        public ClsArea()
        {
            flag = false;
            sx = Constants.NUMMAX; // 格納される可能性のある最大値
            sy = Constants.NUMMAX;
            ex = -Constants.NUMMAX; //  格納される可能性のある最小値
            ey = -Constants.NUMMAX;
        }
        public void SetPoint(Point pt)
        {
            flag = true;
            sx = (pt.X < sx) ? pt.X : sx; // if(pt.X<sx){sx=pt.X;}else{sx=sx;}
            sy = (pt.Y < sy) ? pt.Y : sy;
            ex = (ex < pt.X) ? pt.X : ex;
            ey = (ey < pt.Y) ? pt.Y : ey;
        }
    }

    public class LibCommon
    {
        private double m_dFontSize;

        public int StrToInt(string str)
        {
            int num;

            num = 0;
            try
            {
                if (int.TryParse(str, out num))
                { }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("StrToInt");
                App.LogOut(ex.ToString());
            }
            return (num);
        }

        public Boolean StrToBoolean(string str)
        {
            Boolean num;

            num = false;
            try
            {
                if (Boolean.TryParse(str, out num))
                { }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("StrToBoolean");
                App.LogOut(ex.ToString());
            }
            return (num);
        }

        public double StrToDouble(string str)
        {
            double num;

            num = 0;
            try
            {
                if (double.TryParse(str, out num))
                { }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("StrToDouble");
                App.LogOut(ex.ToString());
            }
            return (num);
        }
        public void CreatePath(string sPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(sPath))
                {
                    System.IO.Directory.CreateDirectory(sPath);
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("CreatePath");
                App.LogOut(ex.ToString());
            }
        }

        public string[] LoadFileLineSJIS(string sFileName)
        {
            string strData;
            string[] aryLine;

            aryLine = new string[1]{""};
            try
            {
                strData = LoadFileSJIS(sFileName);
                strData = strData.Replace("\r\n", ";");
                strData = strData.Replace("\n", ";");
                strData = strData.Replace("\r", ";");
                aryLine = strData.Split(';');
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("LoadFileLineSJIS");
                App.LogOut(ex.ToString());
            }
            return (aryLine);
        }

        public string[] LoadFileLineUTF8(string sFileName)
        {
            string strData;
            string[] aryLine;

            aryLine = new string[1]{""};
            try
            {
                strData = LoadFileUTF8(sFileName);
                strData = strData.Replace("\r\n", ";");
                strData = strData.Replace("\n", ";");
                strData = strData.Replace("\r", ";");
                aryLine = strData.Split(';');
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("LoadFileLineUTF8");
                App.LogOut(ex.ToString());
            }
            return (aryLine);
        }
        public string LoadFileSJIS(string sFileName)
        {
            string strData;

            strData = "";
            try
            {
                if (File.Exists(sFileName))
                {
                    StreamReader sr = new StreamReader(sFileName, Encoding.GetEncoding("Shift_JIS"));
                    strData = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("LoadFileSJIS");
                App.LogOut(ex.ToString());
            }
            return (strData);
        }
        public string LoadFileUTF8(string sFileName)
        {
            string strData;

            strData = "";
            try
            {
                if (File.Exists(sFileName))
                {
                    StreamReader sr = new StreamReader(sFileName);
                    strData = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("LoadFileUTF8");
                App.LogOut(ex.ToString());
            }
            return (strData);
        }
        public Boolean AppendSaveFileSJIS(string sFileName, string sData)
        {
            try
            {
                System.IO.File.AppendAllText(sFileName, sData, Encoding.GetEncoding("shift_jis")); 
                return (true);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend(sFileName + "　AppendSaveFileSJIS");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        public Boolean SaveFileSJIS(string sFileName, string sData)
        {
            try
            {
                StreamWriter sw = new StreamWriter(sFileName, false, Encoding.GetEncoding("shift_jis"));
                sw.Write(sData);
                sw.Close();
                return (true);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend(sFileName + "　SaveFileSJIS");
                App.LogOut(ex.ToString());
                return (false);
            }
        }

        public Boolean SaveFileUTF8(string sFileName, string sData)
        {
            try
            {
                StreamWriter sw = new StreamWriter(sFileName);
                sw.Write(sData);
                sw.Close();
                return (true);
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend(sFileName + "　SaveFileUTF8");
                App.LogOut(ex.ToString());
                return (false);
            }
        }
        public void CopyFile(string sSrcFileName, string sDstFileName)
        {
            try
            {
                System.IO.File.Copy(sSrcFileName, sDstFileName, true);
            }
            catch (Exception ex)
            {//例外発生
                App.ErrorLogAppend(sSrcFileName + "->" + sDstFileName + "　CopyFile");
                App.LogOut(ex.ToString());
            }

        }
        public void DataXmlSave(string xmlfile, Type type, Object obj)
        {
            // ファイルパス(カレントディレトリ直下のText.xml固定)
            XmlTextWriter xtw = null;
            XmlSerializer xs = null;

            // XMLファイル保存
            try
            {
                //XMLファイル書き込みインスタンス作成
                xtw = new XmlTextWriter(xmlfile, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 4;

                //XMLシリアライザクラスインスタンス作成
                xs = new XmlSerializer(type);
                //XMLファイルシリアライズ
                xs.Serialize(xtw, obj);
            }
            catch (Exception ex)
            {//例外発生
                App.ErrorLogAppend(xmlfile + "　DataXmlSave");
                App.LogOut(ex.ToString());
            }
            finally
            {//インスタンス破棄
                if (xtw != null)
                {
                    xtw.Close();
                }
            }
        }

        public Object DataXmlLoad(string xmlfile, Type type)
        {
            StreamReader sr = null;
            XmlSerializer xs = null;
            Object obj = null;

            // XMLファイル読み込み(XmlSerializerによるデシリアライズ)
            try
            {
                //ファイル読み込みインスタンス作成
                sr = new StreamReader(xmlfile, Encoding.ASCII);
                //XMLシリアライザクラスインスタンス作成
                xs = new XmlSerializer(type);
                //XMLファイルデシリアライズ
                obj = (Object)xs.Deserialize(sr);
            }
            catch (Exception ex)
            {//例外発生
                App.ErrorLogAppend(xmlfile + "　DataXmlLoad");
                App.LogOut(ex.ToString());
                return (null);
            }
            finally
            {//インスタンス破棄
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
            return (obj);
        }
    }
}

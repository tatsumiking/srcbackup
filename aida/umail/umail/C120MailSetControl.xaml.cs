using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace umail
{
    /// <summary>
    /// C120MailSetWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class C120MailSetControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;
        private DataTable m_dt;
        private Boolean m_bInEdit;

        public C120MailSetControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            InitControl();

        }
        private void ExitControl()
        {
            List<ObjMailElement> lst;

            lst = GetDataGrid();
            m_Wnd.SetMailElementList(lst);
        }
        private void InitControl()
        {
            List<ObjMailElement> lst;

            InitDataGrid();
            lst = m_Wnd.GetMailElementList();
            SetDataGrid(lst);
        }
        private void InitDataGrid()
        {

            m_bInEdit = false;
            grdMail.Columns.Clear();
            m_libCmn.CreateAddTextCol(grdMail, "ID", "col_uid", 50);
            m_libCmn.CreateAddTextCol(grdMail, "名前", "col_uname", 100);
            m_libCmn.CreateAddTextCol(grdMail, "受信者名", "col_sendname", 100);
            m_libCmn.CreateAddTextCol(grdMail, "受信者メール", "col_sendmail", 220);
        }
        private void SetDataGrid(List<ObjMailElement> list)
        {
            int max, idx;
            DataRow newRowItem;

            m_dt = new DataTable("DataGridSend");
            m_dt.Columns.Add(new DataColumn("col_uid", typeof(string)));
            m_dt.Columns.Add(new DataColumn("col_uname", typeof(string)));
            m_dt.Columns.Add(new DataColumn("col_sendname", typeof(string)));
            m_dt.Columns.Add(new DataColumn("col_sendmail", typeof(string)));
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                newRowItem = m_dt.NewRow();
                newRowItem["col_uid"] = list[idx].m_nLUid.ToString();
                newRowItem["col_uname"] = list[idx].m_sCName;
                newRowItem["col_sendname"] = list[idx].m_sSendName;
                newRowItem["col_sendmail"] = list[idx].m_sSendMail;
                m_dt.Rows.Add(newRowItem);
            }
            grdMail.DataContext = m_dt;
        }
        private List<ObjMailElement> GetDataGrid()
        {
            List<ObjMailElement> list;
            int max, idx;
            DataRow drItem;
            ObjMailElement ome;
            string sId;

            list = new List<ObjMailElement>();
            max = m_dt.Rows.Count;
            for (idx = 0; idx < max; idx++)
            {
                drItem = m_dt.Rows[idx];
                ome = new ObjMailElement();
                sId = drItem["col_uid"] as string;
                ome.m_nLUid = m_libCmn.StrToInt(sId);
                ome.m_sCName = drItem["col_uname"] as string;
                ome.m_sSendName = drItem["col_sendname"] as string;
                ome.m_sSendMail = drItem["col_sendmail"] as string;
                list.Add(ome);
            }
            return (list);
        }
        private void btnUnisLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<ObjMailElement> list;

            list = GetDataGrid();
            m_Wnd.ODBCOpenUnisDB();
            m_Wnd.ODBCSelecttUser(list);
            m_Wnd.ODBCCloseUnisDB();
            InitDataGrid();
            SetDataGrid(list);

        }
        private void btnCsvLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<ObjMailElement> list;
            string sLoadFileName = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSVファイル(*.csv)|*.csv||";
            ofd.Title = "CSVファイルを選択してください";
            var result = ofd.ShowDialog();
            if (result == true)
            {
                sLoadFileName = ofd.FileName;
            }
            if (sLoadFileName == "")
            {
                return;
            }
            list = LoadMailElement(sLoadFileName);
            SetDataGrid(list);
        }
        private void btnCsvSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string sSaveFileName = "";
            List<ObjMailElement> lst;

            if (m_bInEdit == true)
            {
                MessageBox.Show("リターンキーで入力を確定してください");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSVファイル(*.csv)|*.csv||";
            sfd.Title = "CSVファイルを選択してください";
            var result = sfd.ShowDialog();
            if (result == true)
            {
                sSaveFileName = sfd.FileName;
            }
            if (sSaveFileName == "")
            {
                return;
            }
            lst = GetDataGrid();
            SaveMailElement(sSaveFileName, lst);
        }
        private List<ObjMailElement> LoadMailElement(string sLoadFileName)
        {
            List<ObjMailElement> lst;
            string sData;
            string[] aryLine;
            int max, idx;
            string[] aryClm;
            ObjMailElement ome;

            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData == "")
            {
                return (null);
            }
            sData = sData.Replace("\r\n", "\n");
            aryLine = sData.Split('\n');
            lst = new List<ObjMailElement>();
            max = aryLine.Length;
            for (idx = 0; idx < max; idx++)
            {
                if (aryLine[idx] == "")
                {
                    break;
                }
                aryClm = aryLine[idx].Split(',');
                ome = new ObjMailElement();
                ome.m_nLUid = m_libCmn.StrToInt(aryClm[0]);
                ome.m_sCName = aryClm[1];
                ome.m_sSendName = aryClm[2];
                ome.m_sSendMail = aryClm[3];
                lst.Add(ome);
            }
            return (lst);

        }
        private void SaveMailElement(string sSaveFileName, List<ObjMailElement> lst)
        {
            string sData;
            int max, idx;

            if (lst == null)
            {
                return;
            }
            sData = "";
            max = lst.Count;
            if (max == 0)
            {
                return;
            }
            for (idx = 0; idx < max; idx++)
            {
                sData = sData + lst[idx].m_nLUid + "," + lst[idx].m_sCName + ",";
                sData = sData + lst[idx].m_sSendName + "," + lst[idx].m_sSendMail + "\r\n";
            }
            m_libCmn.SaveFileSJIS(sSaveFileName, sData);
        }
        private void btnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_bInEdit == true)
            {
                MessageBox.Show("リターンキーで入力を確定してください");
                return;
            }
            ExitControl();
            m_Wnd.InitC100SendMailControl();
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Wnd.InitC100SendMailControl();
        }
        private void grdMail_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (m_bInEdit == true)
            {
                MessageBox.Show("リターンキーで入力を確定してください");
                return;
            }
            m_bInEdit = true;
        }
        private void grdMail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            m_bInEdit = false;
        }
    }
}

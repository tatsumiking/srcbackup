using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace umail
{
    /// <summary>
    /// C113AdminMailSettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class C113AdminMailSettingControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;
        private DataTable m_dt;
        private Boolean m_bInEdit;

        public C113AdminMailSettingControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            InitControl();
        }
        private void InitControl()
        {
            List<ObjAdminElement> lst;
            InitDataGrid();
            lst = m_Wnd.GetAdminElementList();
            SetDataGrid(lst);
        }
        private void ExitControl()
        {
            List<ObjAdminElement> lst;

            lst = GetDataGrid();
            m_Wnd.SetAdminElementList(lst);
        }
        private void InitDataGrid()
        {
            m_bInEdit = false;
            grdAdminMail.Columns.Clear();
            m_libCmn.CreateAddTextCol(grdAdminMail, "管理者名", "col_name", 200);
            m_libCmn.CreateAddTextCol(grdAdminMail, "メールアドレス", "col_mail", 270);
        }
        private void SetDataGrid(List<ObjAdminElement> lst)
        {
            int max, idx;
            DataRow newRowItem;

            m_dt = new DataTable("DataGridSend");
            m_dt.Columns.Add(new DataColumn("col_name", typeof(string)));
            m_dt.Columns.Add(new DataColumn("col_mail", typeof(string)));
            max = lst.Count;
            for (idx = 0; idx < max; idx++)
            {
                newRowItem = m_dt.NewRow();
                newRowItem["col_name"] = lst[idx].m_sName;
                newRowItem["col_mail"] = lst[idx].m_sMail;
                m_dt.Rows.Add(newRowItem);
            }
            grdAdminMail.DataContext = m_dt;
        }
        private List<ObjAdminElement> GetDataGrid()
        {
            List<ObjAdminElement> lst;
            int max, idx;
            DataRow drItem;
            ObjAdminElement oae;

            lst = new List<ObjAdminElement>();
            max = m_dt.Rows.Count;
            for (idx = 0; idx < max; idx++)
            {
                drItem = m_dt.Rows[idx];
                oae = new ObjAdminElement();
                oae.m_sName = drItem["col_name"] as string;
                oae.m_sMail = drItem["col_mail"] as string;
                lst.Add(oae);
            }
            return (lst);
        }
        private void btnCsvLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<ObjAdminElement> lst;
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
            lst = LoadAdminElement(sLoadFileName);
            SetDataGrid(lst);
        }
        private void btnCsvSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string sSaveFileName = "";
            List<ObjAdminElement> lst;

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
            SaveAdminElement(sSaveFileName, lst);
        }
        public List<ObjAdminElement> LoadAdminElement(string sLoadFileName)
        {
            List<ObjAdminElement> lst;
            string sData;
            string[] aryLine;
            int max, idx;
            string[] aryClm;
            ObjAdminElement oae;

            sData = m_libCmn.LoadFileSJIS(sLoadFileName);
            if (sData == "")
            {
                return (null);
            }
            sData = sData.Replace("\r\n", "\n");
            aryLine = sData.Split('\n');
            lst = new List<ObjAdminElement>();
            max = aryLine.Length;
            for (idx = 0; idx < max; idx++)
            {
                if (aryLine[idx] == "")
                {
                    break;
                }
                aryClm = aryLine[idx].Split(',');
                oae = new ObjAdminElement();
                oae.m_sName = aryClm[0];
                oae.m_sMail = aryClm[1];
                lst.Add(oae);
            }
            return (lst);
        }
        public void SaveAdminElement(string sSaveFileName, List<ObjAdminElement> lst)
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
                sData = sData + lst[idx].m_sName + "," + lst[idx].m_sMail + "\r\n";
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
            m_Wnd.InitC110EnvSetControl();
        }
        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Wnd.InitC110EnvSetControl();
        }

        private void grdAdminMail_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (m_bInEdit == true)
            {
                MessageBox.Show("リターンキーで入力を確定してください");
                return;
            }
            m_bInEdit = true;
        }

        private void grdAdminMail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            m_bInEdit = false;
        }
    }
}

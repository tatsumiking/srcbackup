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
    /// C111TerminalSelectControl.xaml の相互作用ロジック
    /// </summary>
    public partial class C111TerminalSelectControl : UserControl
    {
        private umail.MainWindow m_Wnd;
        private LibCommon m_libCmn;
        private DataTable m_dt;
        private Boolean m_bInEdit;

        public C111TerminalSelectControl()
        {
            InitializeComponent();
            m_Wnd = (umail.MainWindow)Application.Current.MainWindow;
            m_libCmn = m_Wnd.GetClassLibCommon();
            InitControl();
        }
        private void ExitControl()
        {
            GetDataGrid();
        }
        private void InitControl()
        {
            InitDataGrid();
            m_Wnd.ODBCOpenUnisDB();
            m_Wnd.ODBCSelecttTerminal();
            m_Wnd.ODBCCloseUnisDB();
            SetDataGrid();
        }
        private void InitDataGrid()
        {
            m_bInEdit = false;
            grdTerminal.Columns.Clear();
            m_libCmn.CreateAddCheckBoxCol(grdTerminal, "使用する", "col_chk", 50);
            m_libCmn.CreateAddTextCol(grdTerminal, "ターミナルID", "col_tid", 100);
            m_libCmn.CreateAddTextCol(grdTerminal, "ターミナル名", "col_tname", 320);
        }
        private void SetDataGrid()
        {
            int max, idx;
            DataRow newRowItem;
            List<ObjTerminalElement> list;

            m_dt = new DataTable("DataGridSend");
            m_dt.Columns.Add(new DataColumn("col_chk", typeof(Boolean)));
            m_dt.Columns.Add(new DataColumn("col_tid", typeof(string)));
            m_dt.Columns.Add(new DataColumn("col_tname", typeof(string)));
            list = m_Wnd.GetTerminalElementList();
            max = list.Count;
            for (idx = 0; idx < max; idx++)
            {
                newRowItem = m_dt.NewRow();
                newRowItem["col_chk"] = list[idx].m_bCheck;
                newRowItem["col_tid"] = list[idx].m_nId;
                newRowItem["col_tname"] = list[idx].m_sName;
                m_dt.Rows.Add(newRowItem);

            }
            grdTerminal.DataContext = m_dt;
        }
        private void GetDataGrid()
        {
            List<ObjTerminalElement> list;
            int max, idx;
            DataRow drItem;
            string sStr;

            list = m_Wnd.GetTerminalElementList();
            max = m_dt.Rows.Count;
            for (idx = 0; idx < max; idx++)
            {
                drItem = m_dt.Rows[idx];
                list[idx].m_bCheck = (Boolean)drItem["col_chk"];
                sStr = drItem["col_tid"] as string;
                list[idx].m_nId = m_libCmn.StrToInt(sStr);
                list[idx].m_sName = drItem["col_tname"] as string;
            }
        }
        private void btnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExitControl();
            m_Wnd.InitC110EnvSetControl();
        }
        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Wnd.InitC110EnvSetControl();
        }

        private void grdTerminal_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (m_bInEdit == true)
            {
                MessageBox.Show("リターンキーで入力を確定してください");
                return;
            }
            m_bInEdit = true;
        }

        private void grdTerminal_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            m_bInEdit = false;
        }
    }
}

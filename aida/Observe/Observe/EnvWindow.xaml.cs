using System;
using System.Collections.Generic;
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

namespace Observe
{
    /// <summary>
    /// EnvWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EnvWindow : Window
    {
        private MainWindow m_wndMain;
        private String m_sMapPath;

        public EnvWindow()
        {
            InitializeComponent();
        }
        public void SetMainWindow(MainWindow wnd)
        {
            m_wndMain = wnd;
        }
        public void SetMapPath(string sPath)
        {
            m_sMapPath = sPath;
        }
        private void txtMapPath_Loaded(object sender, RoutedEventArgs e)
        {
            txtMapPath.Text = m_sMapPath;
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.SelectedPath = txtMapPath.Text;
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtMapPath.Text = fbd.SelectedPath;
            }
        }
        private void btnRet_Click(object sender, RoutedEventArgs e)
        {
            m_wndMain.ResetEnv();
        }

    }
}

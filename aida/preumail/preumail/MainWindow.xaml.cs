using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace preumail
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean m_bCheckUnisSystem;
        private DispatcherTimer m_dsptCheckUnis;
        private String m_sUNISV3ProcessName = "UNIS_Access";
        private String m_sUNISV4ProcessName = "UNIS_RManager";
        private String m_sUMailProcessName = "umail";

        public MainWindow()
        {
            m_bCheckUnisSystem = false;
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            m_dsptCheckUnis = new DispatcherTimer(DispatcherPriority.Normal);
            m_dsptCheckUnis.Interval = TimeSpan.FromMilliseconds(1000);
            m_dsptCheckUnis.Tick += new EventHandler(TickCheckUnisLoop);
            m_dsptCheckUnis.Start();

        }
        private void TickCheckUnisLoop(object sender, EventArgs e)
        {
            Process[] processesUnisV3;
            Process[] processesUnisV4;
            Process[] processesUmail;
            processesUnisV3 = Process.GetProcessesByName(m_sUNISV3ProcessName);
            processesUnisV4 = Process.GetProcessesByName(m_sUNISV4ProcessName);
            if (processesUnisV3.Length != 0 || processesUnisV4.Length != 0)
            {
                if (m_bCheckUnisSystem == false)
                {
                    m_bCheckUnisSystem = true;
                    processesUmail = Process.GetProcessesByName(m_sUMailProcessName);
                    if (processesUmail.Length == 0)
                    {
                        Process.Start("umail.exe");
                    }
                }
            }
            else
            {
                m_bCheckUnisSystem = false;
            }
        }
    }
}

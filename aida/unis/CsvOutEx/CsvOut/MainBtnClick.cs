using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CsvOut
{
    public partial class MainWindow : Window
    {
        private void lblbtnSlct_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnSlct.BorderBrush = Brushes.LightGray;
                lblbtnSlct.Background = Brushes.LightCyan;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSlct_PreviewMouseDown");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnSlct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnSlct.BorderBrush = Brushes.Black;
                lblbtnSlct.Background = Brushes.LightGray;
                btnSlctClick();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSlct_PreviewMouseUp");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnSlct_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblbtnSlct.BorderBrush = Brushes.Black;
                lblbtnSlct.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSlct_MouseLeave");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMDBSlct_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnMDBSlct.BorderBrush = Brushes.LightGray;
                lblbtnMDBSlct.Background = Brushes.LightCyan;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMDBSlct_PreviewMouseDown");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMDBSlct_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnMDBSlct.BorderBrush = Brushes.Black;
                lblbtnMDBSlct.Background = Brushes.LightGray;
                btnMDBSlctClick();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMDBSlct_PreviewMouseUp");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMDBSlct_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblbtnMDBSlct.BorderBrush = Brushes.Black;
                lblbtnMDBSlct.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMDBSlct_MouseLeave");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnSet_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnSet.BorderBrush = Brushes.LightGray;
                lblbtnSet.Background = Brushes.LightCyan;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSet_PreviewMouseDown");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnSet_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnSet.BorderBrush = Brushes.Black;
                lblbtnSet.Background = Brushes.LightGray;
                btnSetClick();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSet_PreviewMouseUp");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnSet_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblbtnSet.BorderBrush = Brushes.Black;
                lblbtnSet.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnSet_MouseLeave");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMini_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnMini.BorderBrush = Brushes.LightGray;
                lblbtnMini.Background = Brushes.LightCyan;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMini_PreviewMouseDown");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMini_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnMini.BorderBrush = Brushes.Black;
                lblbtnMini.Background = Brushes.LightGray;
                btnMiniClick();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMini_PreviewMouseUp");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnMini_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblbtnMini.BorderBrush = Brushes.Black;
                lblbtnMini.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnMini_MouseLeave");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnExit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnExit.BorderBrush = Brushes.LightGray;
                lblbtnExit.Background = Brushes.LightCyan;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnExit_PreviewMouseDown");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnExit_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                lblbtnExit.BorderBrush = Brushes.Black;
                lblbtnExit.Background = Brushes.LightGray;
                btnExitClick();
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnExit_PreviewMouseUp");
                App.LogOut(ex.ToString());
            }
        }
        private void lblbtnExit_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                lblbtnExit.BorderBrush = Brushes.Black;
                lblbtnExit.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                App.ErrorLogAppend("lblbtnExit_MouseLeave");
                App.LogOut(ex.ToString());
            }
        }
    }
}

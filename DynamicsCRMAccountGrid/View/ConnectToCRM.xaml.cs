using DynamicsCRMAccountGrid.Domain;
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

namespace DynamicsCRMAccountGrid.View
{
    /// <summary>
    /// Interaction logic for ConnectToCRM.xaml
    /// </summary>
    public partial class ConnectToCRM : Window
    {
        public ConnectToCRM()
        {
            InitializeComponent();
        }


        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {


            Connection.UserName = txtUserName.Text.ToString();
            Connection.Password = txtPassword.Password.ToString();

            var mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel();
            mainWindow.DataContext = viewModel;
            if (Connection.Result == true)
            {
                result.Content = "Connection successful";
                result.Foreground = System.Windows.Media.Brushes.Green;
                spinner.Visibility = Visibility.Hidden;
                mainWindow.Show();
            }
            else
            {
                result.Content = "Connection failed";
                spinner.Visibility = Visibility.Hidden;
            }
        }

        private void btnSaveData_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spinner.Visibility = Visibility.Visible;
        }
    }
}

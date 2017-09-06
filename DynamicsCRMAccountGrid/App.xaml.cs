using DynamicsCRMAccountGrid.View;
using System.Windows;

namespace DynamicsCRMAccountGrid
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var connect = new ConnectToCRM();         
            connect.Show();
        }
    }
}
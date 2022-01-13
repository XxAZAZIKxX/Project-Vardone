using System.Collections.Generic;
using System.Windows;
using Vardone.Core;

namespace Vardone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var listArgs = new List<string>(e.Args);
            var mainWindow = Vardone.MainWindow.GetInstance();

            if (listArgs.Contains("-autostart"))
            {
                if (ConfigWorker.GetStartMinimized())
                {
                    mainWindow.HideAppInTaskbar();
                    return;
                }
            }
            mainWindow.Show();
        }
    }
}

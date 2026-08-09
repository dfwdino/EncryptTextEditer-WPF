using EncryptTextEditer_WPF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EncryptTextEditer_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Closing PasswordWindow would otherwise trigger the default OnLastWindowClose
            // shutdown before MainWindow ever gets shown, since it's the only open window.
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var passwordWindow = new PasswordWindow(DefaultSettingsModel.OptionsFileLocation);
            bool? unlocked = passwordWindow.ShowDialog();

            if (unlocked != true || passwordWindow.ResultOption == null || passwordWindow.Password == null)
            {
                Shutdown();
                return;
            }

            var mainWindow = new MainWindow(passwordWindow.ResultOption, passwordWindow.Password);
            MainWindow = mainWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainWindow.Show();
        }
    }
}

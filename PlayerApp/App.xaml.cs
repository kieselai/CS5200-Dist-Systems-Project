using PlayerApp.Views;
using System.Windows;
using Microsoft.Practices.Unity;
using PlayerApp.ViewModels;
using Player;
using AppCommon;

namespace PlayerApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Connection = new PlayerConnection();
            Bootstrapper.BootStrapAndConfig<Bootstrapper, MainWindow, MainWindowViewModel>();
        }
    }
}

using Player.Views;
using System.Windows;
using Player.ViewModels;
using PlayerProcess;
using AppCommon;

namespace Player
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Launcher = new PlayerLauncher();
            Bootstrapper.BootStrapAndConfig<Bootstrapper, MainWindow, MainWindowViewModel>();
        }
    }
}
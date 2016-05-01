using PlayerApp.Views;
using System.Windows;
using Microsoft.Practices.Unity;
using PlayerApp.ViewModels;
using Player;

namespace PlayerApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Connection = new PlayerConnection();

            var bootstrapper = new Bootstrapper();
            AppState.Bootstrapper = bootstrapper;
            bootstrapper.Run();
            var mainWindow = AppState.Container.Resolve<MainWindow>();
            mainWindow.DataContext = new MainWindowViewModel();
            ((MainWindowViewModel)mainWindow.DataContext).initialize();
        }
    }
}

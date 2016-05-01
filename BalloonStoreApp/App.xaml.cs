using BalloonStoreApp.Views;
using System.Windows;
using Microsoft.Practices.Unity;
using BalloonStoreApp.ViewModels;
using BalloonStore;

namespace BalloonStoreApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Connection = new BalloonStoreConnection();

            var bootstrapper = new Bootstrapper();
            AppState.Bootstrapper = bootstrapper;
            bootstrapper.Run();
            var mainWindow = AppState.Container.Resolve<MainWindow>();
            mainWindow.DataContext = new MainWindowViewModel();
            ((MainWindowViewModel)mainWindow.DataContext).initialize();
        }
    }
}
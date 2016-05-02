using BalloonStore.Views;
using System.Windows;
using AppCommon;
using BalloonStore.ViewModels;
using BalloonStoreProcess;

namespace BalloonStore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Connection = new BalloonStoreConnection();
            Bootstrapper.BootStrapAndConfig<Bootstrapper, MainWindow, MainWindowViewModel>();
        }
    }
}
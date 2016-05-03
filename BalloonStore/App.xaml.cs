using BalloonStore.Views;
using System.Windows;
using AppCommon;
using BalloonStore.ViewModels;
using BalloonStoreProcess;
using System;

namespace BalloonStore
{
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            AppState.Launcher = new BalloonStoreLauncher();
            Bootstrapper.BootStrapAndConfig<Bootstrapper, MainWindow, MainWindowViewModel>();
        }
    }
}
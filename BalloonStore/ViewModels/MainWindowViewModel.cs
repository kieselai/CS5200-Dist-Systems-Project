using AppCommon.ViewModels;
using BalloonStore.Views;
using AppCommon;
using System.Windows.Media;
using System.Windows;

namespace BalloonStore.ViewModels
{
    public class MainWindowViewModel : AbstractMainWindowViewModel {
        public override void initialize() {
            base.initialize();
            AppDispatcher.Navigate<OwnInfoView>("OwnInfoRegion");
            if( AppState.Launcher.IsRunning == false ) {
                AppState.Launcher.Start();
            }
            AppDispatcher.DispatchUI( ()=> {
                Application.Current.MainWindow.Background = new SolidColorBrush(Colors.CadetBlue);
            });
        }
    }
}
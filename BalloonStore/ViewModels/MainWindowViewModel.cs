using AppCommon.ViewModels;
using BalloonStore.Views;
using AppCommon;

namespace BalloonStore.ViewModels
{
    public class MainWindowViewModel : AbstractMainWindowViewModel {
        public override void initialize() {
            base.initialize();
            AppDispatcher.Navigate<OwnInfoView>("OwnInfoRegion");
            if( AppState.Connection.IsRunning == false ) {
                AppState.Connection.Start( true );
            }
        }
    }
}

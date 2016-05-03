using AppCommon.ViewModels;
using Player.Views;
using AppCommon;
using AppCommon.Views;

namespace Player.ViewModels
{
    public class MainWindowViewModel : AbstractMainWindowViewModel {
        public override void initialize() {
            AppDispatcher.Navigate<LoginView>("MainRegion");
            base.initialize();
            AppDispatcher.Navigate<OwnInfoView>("OwnInfoRegion");
            AppDispatcher.DispatchUI(()=> {
                AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").Visibility = System.Windows.Visibility.Hidden;
                AppDispatcher.GetView<OwnInfoView>("OwnInfoRegion").Visibility = System.Windows.Visibility.Hidden;
            });
        }
    }
}
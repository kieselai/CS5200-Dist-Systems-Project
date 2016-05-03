using AppCommon;
using Player.Views;
using AppCommon.Views;
using AppCommon.ViewModels;


namespace Player.ViewModels
{
    public class LogoutViewModel : AbstractLogoutViewModel {
        public override void CompleteLogout() {
            base.CompleteLogout();
            AppDispatcher.DispatchUI(()=> {
                AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").Visibility = System.Windows.Visibility.Hidden;
                AppDispatcher.GetView<OwnInfoView>   ("OwnInfoRegion"   ).Visibility = System.Windows.Visibility.Hidden;
            });
        }
    }
}
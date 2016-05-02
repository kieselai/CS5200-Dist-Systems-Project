using AppCommon;
using BalloonStore.Views;
using AppCommon.Views;
using AppCommon.ViewModels;
using System.Windows;

namespace BalloonStore.ViewModels {
    public class LogoutViewModel : AbstractLogoutViewModel {
        public override void CompleteLogout() {
            base.CompleteLogout();
            Application.Current.Shutdown();
        }
    }
}
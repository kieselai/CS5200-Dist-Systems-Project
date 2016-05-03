using Prism.Commands;
using AppCommon.Generic;
using AppCommon.Views;
using AppCommon;
using Player.Views;


namespace Player.ViewModels
{
    public class LoginViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogin;
        private bool isLocal;
        public bool IsLocal {
            get { return isLocal; }
            set { SetProperty(ref isLocal, value); }
        }
        public DelegateCommand InitiateLogin { get { return _initiateLogin; } }
        public LoginViewModel() {
            _initiateLogin = UnconditionalDelegateCommand( ()=> {
                if( AppState.Launcher.IsRunning == false ) {
                    AppState.Launcher.Options.UseLocalSettings = isLocal;
                    AppState.Launcher.Start();
                    AppDispatcher.DispatchUI(()=> {
                        AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").Visibility = System.Windows.Visibility.Visible;
                        AppDispatcher.GetView<OwnInfoView>   ("OwnInfoRegion"   ).Visibility = System.Windows.Visibility.Visible;
                        AppDispatcher.GetView<LoginView>     ("MainRegion"      ).Visibility = System.Windows.Visibility.Hidden;
                    });
                }
            });
        }
    }
}
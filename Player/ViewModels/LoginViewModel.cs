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
            set {
                AppState.Launcher.Options.UseLocalSettings = value;
                SetProperty(ref isLocal, value);
            }
        }
        public DelegateCommand InitiateLogin { get { return _initiateLogin; } }
        public LoginViewModel() {
            _initiateLogin = UnconditionalDelegateCommand( ()=> {
                ExecuteLogin();
            });
        }

        public static void ExecuteLogin() {
            if( AppState.Launcher != null && AppState.Launcher.IsRunning == false ) {
                    AppState.Launcher.Start();
                    AppDispatcher.DispatchUI(()=> {
                        AppDispatcher.GetView<CommonInfoView>("CommonInfoRegion").Visibility = System.Windows.Visibility.Visible;
                        AppDispatcher.GetView<OwnInfoView>   ("OwnInfoRegion"   ).Visibility = System.Windows.Visibility.Visible;
                        AppDispatcher.GetView<LoginView>     ("MainRegion"      ).Visibility = System.Windows.Visibility.Hidden;
                    });
                }
        }
    }
}
using Prism.Commands;
using PlayerApp.Generic;
using PlayerApp.Views;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PlayerApp.ViewModels
{
    public class LogoutViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogout;
        public DelegateCommand InitiateLogout { get { return _initiateLogout; } }
        public LogoutViewModel() : base("Log In") {
            _initiateLogout = TrueDelegateCommand( ()=> {
                AppState.Connection.Player.Logout((success)=> {
                    if(success) {
                        AppState.Connection.Player.PlayerState.CurrentMessage = "Successfully Logged out";
                        Navigate<LoginView>("MainRegion");
                        AppState.Connection.Player.SubSystem.Dispatcher.Stop();
                        AppState.Connection.Stop();
                        AppState.Connection.Player.Reset();
                    }
                });
            });
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "IsShutDown") {
                if(AppState.Connection.Player.PlayerState.IsShutDown) {
                    AppState.Connection.Player.SubSystem.Dispatcher.Stop();
                    AppState.Connection.Player.Stop();
                }
            }
        }
    }
}
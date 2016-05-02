using Prism.Commands;
using BalloonStoreApp.Generic;
using BalloonStoreApp.Views;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BalloonStoreApp.ViewModels
{
    public class LogoutViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogout;
        public DelegateCommand InitiateLogout { get { return _initiateLogout; } }
        public LogoutViewModel() : base("Log In") {
            _initiateLogout = TrueDelegateCommand( ()=> {
                AppState.Connection.BalloonStore.Logout((success)=> {
                    if(success) {
                        AppState.Connection.BalloonStore.BalloonStoreState.CurrentMessage = "Successfully Logged out";
                        Navigate<LoginView>("MainRegion");
                        AppState.Connection.BalloonStore.SubSystem.Dispatcher.Stop();
                        AppState.Connection.Stop();
                        AppState.Connection.BalloonStore.Reset();
                    }
                });
            });
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "IsShutDown") {
                if(AppState.Connection.BalloonStore.BalloonStoreState.IsShutDown) {
                    AppState.Connection.BalloonStore.SubSystem.Dispatcher.Stop();
                    AppState.Connection.BalloonStore.Stop();
                }
            }
        }
    }
}
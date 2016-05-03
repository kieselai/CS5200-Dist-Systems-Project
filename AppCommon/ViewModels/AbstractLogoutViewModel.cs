using Prism.Commands;
using AppCommon.Generic;
using AppCommon.Views;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AppCommon.ViewModels
{
    public abstract class AbstractLogoutViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogout;
        public DelegateCommand InitiateLogout { get { return _initiateLogout; } }
        public AbstractLogoutViewModel() {
            _initiateLogout = UnconditionalDelegateCommand( ()=> {
                AppState.Launcher.Process.Logout((success)=> {
                    if(success) CompleteLogout();
                });
            });
        }

        public virtual void CompleteLogout() {
            AppState.Launcher.Process.State.CurrentMessage = "Successfully Logged out";
            AppState.Launcher.Process.SubSystem.Dispatcher.Stop();
            AppState.Launcher.Stop();
            AppState.Launcher.Process.State.Reset();
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "IsShutDown") {
                if(AppState.Launcher.Process.State.IsShutDown) {
                    AppState.Launcher.Process.SubSystem.Dispatcher.Stop();
                    AppState.Launcher.Process.Stop();
                }
            }
        }
    }
}
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
                AppState.Connection.Process.Logout((success)=> {
                    if(success) CompleteLogout();
                });
            });
        }

        public virtual void CompleteLogout() {
            AppState.Connection.Process.State.CurrentMessage = "Successfully Logged out";
            AppState.Connection.Process.SubSystem.Dispatcher.Stop();
            AppState.Connection.Stop();
            AppState.Connection.Process.State.Reset();
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "IsShutDown") {
                if(AppState.Connection.Process.State.IsShutDown) {
                    AppState.Connection.Process.SubSystem.Dispatcher.Stop();
                    AppState.Connection.Process.Stop();
                }
            }
        }
    }
}
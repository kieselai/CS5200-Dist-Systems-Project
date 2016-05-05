using Prism.Commands;
using AppCommon.Generic;
using System.Threading;
using SharedObjects;

namespace AppCommon.ViewModels
{
    public class LogoutViewModel : BaseViewModel {
        private readonly DelegateCommand _initiateLogout;
        public DelegateCommand InitiateLogout { get { return _initiateLogout; } }
        public LogoutViewModel() {
            _initiateLogout = UnconditionalDelegateCommand( ()=> {
                ThreadPool.QueueUserWorkItem(StartLogout, null);
            });
        }

        
        public void CompleteLogout() {
            AppState.Launcher.Process.State.CurrentMessage = "Successfully Logged out";
            AppState.Launcher.Process.SubSystem.Dispatcher.Stop();
            AppState.Launcher.Stop();
            AppState.Launcher.Process.State.SetStatus(ProcessInfo.StatusCode.Terminating);
        }

        public void StartLogout(object state=null) {
            AppState.Launcher.Process.Logout((success)=> {
                if(success) CompleteLogout();
            });
        }
    }
}
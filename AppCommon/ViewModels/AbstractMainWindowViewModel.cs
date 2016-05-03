using AppCommon.Generic;
using System.ComponentModel;
using AppCommon.Views;
namespace AppCommon.ViewModels
{
    public class AbstractMainWindowViewModel : BaseViewModel{
        public virtual void initialize() {
            AppDispatcher.Navigate<MessageView>("MessageRegion");
            AppDispatcher.Navigate<LogView>("LogRegion");
            AppDispatcher.Navigate<CommonInfoView>("CommonInfoRegion");
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Status") {
                if(AppState.Launcher.Process.State.Status == SharedObjects.ProcessInfo.StatusCode.Terminating) {
                    AppState.Launcher.Process.Stop();
                }
            }
        }
    }
}

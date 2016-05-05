using AppCommon.Generic;
using System.ComponentModel;
using AppCommon.Views;
using System;
using System.Windows;

namespace AppCommon.ViewModels
{
    public class AbstractMainWindowViewModel : BaseViewModel{
        public virtual void initialize() {
            AppDispatcher.Navigate<MessageView>("MessageRegion");
            AppDispatcher.Navigate<LogView>("LogRegion");
            AppDispatcher.Navigate<CommonInfoView>("CommonInfoRegion");
            AppState.Launcher.Process.State.ProcessInfo.PropertyChanged += new PropertyChangedEventHandler(ChangeState);
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Status") {
                if(AppState.Launcher.Process.State.Status == SharedObjects.ProcessInfo.StatusCode.Terminating) {
                    AppState.Launcher.Process.SubSystem.Dispatcher.Stop();
                    AppState.Launcher.Process.Stop();
                    AppDispatcher.DispatchUI(()=>Application.Current.Shutdown());
                }
            }
        }
    }
}

using BalloonStoreApp.Generic;
using BalloonStore;
using System.ComponentModel;
using BalloonStoreApp.Views;
using System.Windows;

namespace BalloonStoreApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel{

        public MainWindowViewModel() : base("BalloonStore") {}

        public void initialize() {
            Navigate<LoginView>("MainRegion");
            Navigate<MessageView>("MessageRegion");
            Navigate<LogView>("LogRegion");
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Status") {
                if(AppState.Connection.BalloonStore.BalloonStoreState.Status == SharedObjects.ProcessInfo.StatusCode.Terminating) {
                    AppState.Connection.BalloonStore.Stop();
                }
            }
        }
    }
}

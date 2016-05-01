using PlayerApp.Generic;
using Player;
using System.ComponentModel;
using PlayerApp.Views;
using System.Windows;

namespace PlayerApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel{

        public MainWindowViewModel() : base("Player") {}

        public void initialize() {
            Navigate<LoginView>("MainRegion");
            Navigate<MessageView>("MessageRegion");
            Navigate<LogView>("LogRegion");
        }

        public void ChangeState(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Status") {
                if(AppState.Connection.Player.PlayerState.Status == SharedObjects.ProcessInfo.StatusCode.Terminating) {
                    AppState.Connection.Player.Stop();
                }
            }
        }
    }
}

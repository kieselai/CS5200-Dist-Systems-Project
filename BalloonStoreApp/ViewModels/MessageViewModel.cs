using BalloonStoreApp.Generic;
using System.ComponentModel;

namespace BalloonStoreApp.ViewModels
{
    public class MessageViewModel : BaseViewModel {
        private string _message;

        public string Message  {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        public MessageViewModel() : base("Log in") {
            AppState.Connection.BalloonStore.BalloonStoreState.PropertyChanged += new PropertyChangedEventHandler(UpdateMessage);
        }
        public void UpdateMessage(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "CurrentMessage") {
                Message = AppState.Connection.BalloonStore.BalloonStoreState.CurrentMessage;
            }
        }
    }
}

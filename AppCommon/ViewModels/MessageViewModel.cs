using AppCommon.Generic;
using System.ComponentModel;

namespace AppCommon.ViewModels
{
    public class MessageViewModel : BaseViewModel {
        private string _message;

        public string Message  {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        public MessageViewModel(){
            AppState.Connection.Process.State.PropertyChanged += new PropertyChangedEventHandler(UpdateMessage);
        }
        public void UpdateMessage(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "CurrentMessage") {
                Message = AppState.Connection.Process.State.CurrentMessage;
            }
        }
    }
}

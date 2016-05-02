using MyUtilities;
using SharedObjects;
using System.Collections.ObjectModel;

namespace CommunicationLayer {
    public class ProcessState : BindableEventObject {

        public ProcessState() {
            _procInfo = new ProcessInfo();
            _currGame = new GameInfo();
            CurrentMessage = "";
        }

        private string _currentMessage;
        public  string CurrentMessage {
            get { return _currentMessage; }
            set { SetProperty( ref _currentMessage, value );  }
        }

        private BindableProcessInfo _procInfo;
        public BindableProcessInfo ProcessInfo {
            get { return _procInfo;                  }
            set { SetProperty( _procInfo, value, (p) => _procInfo.Info = value); }
        }

        public ProcessInfo.StatusCode Status {
            get { return ProcessInfo.Status;  }
        }

        private BindableGameInfo _currGame;
        public  BindableGameInfo CurrentGame {
            get { return _currGame; }
            set { SetProperty( _currGame, value, (c)=> _currGame.Info = value); }
        }


        private bool _isShutDown;
        public bool IsShutDown {
            get { return _isShutDown;                  }
            set { SetProperty(ref _isShutDown, value); }
        }


        public virtual void Reset() {
            CurrentMessage = "";
            ProcessInfo.Reset();
            CurrentGame.Reset();
        }
    }
}

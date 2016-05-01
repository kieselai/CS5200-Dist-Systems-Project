using MyUtilities;
using SharedObjects;
using System.ComponentModel;

namespace CommunicationLayer {
    public class ProcessState : BindableEventObject {

        public ProcessState() {
            _procInfo = new ProcessInfo();
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


        private bool _isShutDown;
        public bool IsShutDown {
            get { return _isShutDown;                  }
            set { SetProperty(ref _isShutDown, value); }
        }

        

        public void Reset() {
            ProcessInfo = new ProcessInfo {
                AliveReties = 0,
                AliveTimestamp = null,
                EndPoint = null,
                Label = null,
                ProcessId = 0,
                Status = SharedObjects.ProcessInfo.StatusCode.NotInitialized,
                Type = SharedObjects.ProcessInfo.ProcessType.Unknown
            };
            CurrentMessage = "";
        }
    }
}

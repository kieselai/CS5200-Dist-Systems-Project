using log4net;
using MyUtilities;
using SharedObjects;
using System.ComponentModel;

namespace CommunicationLayer {

    public class ProcessState : BindableEventObject {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessState));

        public ProcessState() {
            _procInfo = new ProcessInfo();
            _identityInfo = new IdentityInfo();
            _currGame = new GameInfo();
            CurrentMessage = "";
            IdentityInfo.PropertyChanged += new PropertyChangedEventHandler(OnIdentityInfoChanged);
        }

        public virtual void initialize(string firstName, string lastName, string alias, string aNumber) {
            log.Debug("Initializing Player Details.");
            IdentityInfo  = new IdentityInfo {
                FirstName = firstName,
                LastName  = lastName,
                Alias     = alias,
                ANumber   = aNumber
            };
            ProcessInfo.Label = alias;
        }

        protected void OnIdentityInfoChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Alias") {
                ProcessInfo.Label = IdentityInfo.Alias;
            }
        }

        private string _currentMessage;
        public  string CurrentMessage {
            get { return _currentMessage; }
            set { SetProperty( ref _currentMessage, value );  }
        }

        private BindableIdentityInfo _identityInfo;
        public  BindableIdentityInfo IdentityInfo {
            get { return _identityInfo; }
            set { SetProperty( _identityInfo, value, (i) => _identityInfo.Info = value ); }
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

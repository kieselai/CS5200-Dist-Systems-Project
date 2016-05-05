using log4net;
using MyUtilities;
using SharedObjects;
using System.ComponentModel;

namespace CommunicationLayer {

    abstract public class ProcessState : BindableEventObject {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessState));

        public ProcessState() {
            _leftGame      = false;
            _procInfo      = new ProcessInfo();
            _identityInfo  = new IdentityInfo();
            _currGame      = new GameInfo();
            _thisGameProc  = new GameProcessData();
            CurrentMessage = "";
            IdentityInfo.PropertyChanged += new PropertyChangedEventHandler(OnIdentityInfoChanged);
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
            get { return _procInfo; }
            set { SetProperty( _procInfo, value, (p) => _procInfo.Info = value); }
        }

        public ProcessInfo.StatusCode Status {
            get { return ProcessInfo.Status;  }
        }

        private bool _leftGame;
        public bool LeftGame {
            get { return _leftGame; }
            set { SetProperty( ref _leftGame, value); }
        }

        private BindableGameInfo _currGame;
        public  BindableGameInfo CurrentGame {
            get { return _currGame; }
            set { SetProperty( _currGame, value, (c)=> _currGame.Info = value); }
        }

        private BindableGameProcessData _thisGameProc;
        public BindableGameProcessData ThisGameProc {
            get { return _thisGameProc; }
            set { SetProperty( _thisGameProc, value, (c)=> _thisGameProc.GameProcessData = value); }
        }

        public virtual void Reset() {
            CurrentMessage = "";
            ProcessInfo.Reset();
            CurrentGame.Reset();
            ThisGameProc = new GameProcessData();
        }

        public void SetStatus(ProcessInfo.StatusCode status) {
            if(StatusIsPossible(status)) {
                ProcessInfo.Status = status;
                CurrentMessage = GetMessageFromStatus(status)?? CurrentMessage;
            }
        }

        public bool StatusIsPossible( ProcessInfo.StatusCode status ) {
            var isPlayer = ProcessInfo.Type == SharedObjects.ProcessInfo.ProcessType.Player;
            var isGM     = ProcessInfo.Type == SharedObjects.ProcessInfo.ProcessType.GameManager;
            switch ( status ) {
                case SharedObjects.ProcessInfo.StatusCode.HostingGame: return isGM;
                case SharedObjects.ProcessInfo.StatusCode.Won:         return isPlayer;
                case SharedObjects.ProcessInfo.StatusCode.Lost:        return isPlayer; 
                case SharedObjects.ProcessInfo.StatusCode.Tied:        return isPlayer;
                default: return true;
            }
        }

        public string GetDefaultMessageFromStatus(ProcessInfo.StatusCode status) {
            if ( !StatusIsPossible(status)) {
                log.Error("Status code is not possible for this process.");
                return null;
            }
            switch ( status ) {
                case SharedObjects.ProcessInfo.StatusCode.NotInitialized: return "Not Initialized";
                case SharedObjects.ProcessInfo.StatusCode.Initializing:   return "Initializing";
                case SharedObjects.ProcessInfo.StatusCode.Registered:     return "Registered, Retrieving game list";
                case SharedObjects.ProcessInfo.StatusCode.JoiningGame:    return "Joining game";
                case SharedObjects.ProcessInfo.StatusCode.JoinedGame:     return "Joined a game, waiting to start";
                case SharedObjects.ProcessInfo.StatusCode.PlayingGame:    return "Playing ( In game )";
                case SharedObjects.ProcessInfo.StatusCode.LeavingGame:    return "Leaving Game";
                case SharedObjects.ProcessInfo.StatusCode.Won:            return "You Won!!!";
                case SharedObjects.ProcessInfo.StatusCode.Lost:           return "You Lost :(";
                case SharedObjects.ProcessInfo.StatusCode.Tied:           return "You Tied :/";
                case SharedObjects.ProcessInfo.StatusCode.Terminating:    return "Process Terminating";
                default: log.Error("Unknown error while settings status code message."); return null;
            }
        }
        abstract public string GetMessageFromStatus(ProcessInfo.StatusCode status);
    }
}

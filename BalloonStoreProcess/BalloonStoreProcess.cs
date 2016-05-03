using System.Threading;
using BalloonStoreProcess.Conversation;
using SharedObjects;
using log4net;
using ProcessCommon;

namespace BalloonStoreProcess
{
    public class BalloonStoreProcess : CommonProcessBase {
        private static readonly ILog log = LogManager.GetLogger(typeof(BalloonStoreProcess));
        private Properties.Settings Settings { get { return Properties.Settings.Default; } }

        public BalloonStoreProcess(int minPort, int maxPort): base(new BalloonStoreState(), new BalloonStoreConversationFactory(), minPort, maxPort) {
            State.ProcessInfo.Type = ProcessInfo.ProcessType.BalloonStore;
        }
        
        override protected void Process(object state) {
            SubSystem.Dispatcher.Start();
            while( KeepGoing ) {
                //log.Debug("------------- At top of BalloonStore process loop -------------");
                switch ( State.Status ) {
                    case ProcessInfo.StatusCode.NotInitialized:  Login();       break;
                    case ProcessInfo.StatusCode.Initializing:    break;
                    case ProcessInfo.StatusCode.Registered:      JoinGame();    break;
                    case ProcessInfo.StatusCode.JoiningGame:     break;
                    case ProcessInfo.StatusCode.JoinedGame:      break;
                    case ProcessInfo.StatusCode.PlayingGame:     PlayGame();    break;
                    case ProcessInfo.StatusCode.LeavingGame:     LeavingGame(); break;
                    case ProcessInfo.StatusCode.Terminating:     break;
                }
                Thread.Sleep(100);
            }
        }

        protected void PlayGame() {
            log.Debug("In Process PlayGame function.");
            SetStatus(ProcessInfo.StatusCode.PlayingGame);
            while(State.Status == ProcessInfo.StatusCode.PlayingGame) {
                Thread.Sleep(100);
            }
        }

        override protected void SetStatus(ProcessInfo.StatusCode status) {
            if(StatusIsPossible(status)) {
                State.ProcessInfo.Status = status;
                string message = GetMessageFromStatus(status);
                if( message != null ) {
                     State.CurrentMessage = message;
                }
            }
            else {
                log.Error("Status code is not possible for this process.");
            }
        }

        protected string GetMessageFromStatus(ProcessInfo.StatusCode status) {
            switch ( status ) {
                case ProcessInfo.StatusCode.NotInitialized:  return "Not Initialized";
                case ProcessInfo.StatusCode.Initializing:    return "Initializing";
                case ProcessInfo.StatusCode.Registered:      return "Registered";
                case ProcessInfo.StatusCode.JoiningGame:     return "Joining game";
                case ProcessInfo.StatusCode.JoinedGame:      return "Joined a game, waiting to start";
                case ProcessInfo.StatusCode.PlayingGame:     return "Playing ( In game )";
                case ProcessInfo.StatusCode.LeavingGame:     return "Logging out";
                default: return null;
            }
        }

        public void Login() {
            SetStatus( ProcessInfo.StatusCode.Initializing );
            log.Debug( "In Process Login function." );
            var success = SubSystem.Dispatcher.DispatchConversation<LoginConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.Registered );
            else SetStatus( ProcessInfo.StatusCode.NotInitialized );
        }

        public void JoinGame() {
            log.Debug("In Process JoinGame function.");
            var success = SubSystem.Dispatcher.DispatchConversation<JoinGameConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.JoinedGame );
        }

        public void LeavingGame() {
            log.Debug("In Process LeavingGame function.");
            State.CurrentMessage = "Game is ending without ranking.";
        }
    }
}
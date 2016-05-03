using System.Threading;
using PlayerProcess.Conversation;
using SharedObjects;
using log4net;
using System.Threading.Tasks;
using ProcessCommon;

namespace PlayerProcess
{
    public class PlayerProcess : CommonProcessBase {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlayerProcess));

        public PlayerProcess(int minPort, int maxPort): base(new PlayerState(), new PlayerConversationFactory(), minPort, maxPort) {
            State.ProcessInfo.Type = ProcessInfo.ProcessType.Player;
        }

        override protected void Process(object state) {
            SubSystem.Dispatcher.Start();
            while( KeepGoing ) {
                //log.Debug("------------- At top of Player process loop -------------");
                switch ( State.Status ) {
                    case ProcessInfo.StatusCode.NotInitialized:  Login();       break;
                    case ProcessInfo.StatusCode.Initializing:    break;
                    case ProcessInfo.StatusCode.Registered:      GetGameList(); break;
                    case ProcessInfo.StatusCode.JoiningGame:     JoinGame();    break;
                    case ProcessInfo.StatusCode.JoinedGame:      break;
                    case ProcessInfo.StatusCode.PlayingGame:     PlayGame();   break;
                    case ProcessInfo.StatusCode.LeavingGame:     LeavingGame(); break;
                    case ProcessInfo.StatusCode.Won:             Won();         break;
                    case ProcessInfo.StatusCode.Lost:            Lost();        break;
                    case ProcessInfo.StatusCode.Terminating:     break;
                    case ProcessInfo.StatusCode.Tied:            Tied();        break;
                }
                Thread.Sleep(100);
            }
        }

        protected void PlayGame() {
            log.Debug("In Process PlayGame function.");
            SetStatus(ProcessInfo.StatusCode.PlayingGame);
            while(State.Status == ProcessInfo.StatusCode.PlayingGame) {
                for( int i = 0; i < (State as PlayerState).Pennies.AvailableCount; i+=3 ) {
                    Task.Run( ()=>SubSystem.Dispatcher.DispatchConversationAsync<BuyBalloonConversation>() );
                }
                for( int i = 0; i < (State as PlayerState).Balloons.AvailableCount; i++ ) {
                    Task.Run( ()=>SubSystem.Dispatcher.DispatchConversationAsync<FillBalloonConversation>() );
                }
                for ( int i = 0; i < (State as PlayerState).FilledBalloons.AvailableCount; i++ ) {
                    Task.Run( ()=>SubSystem.Dispatcher.DispatchConversationAsync<ThrowBalloonConversation>());
                }
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
                case ProcessInfo.StatusCode.Registered:      return "Registered, Retrieving game list";
                case ProcessInfo.StatusCode.JoiningGame:     return "Joining game";
                case ProcessInfo.StatusCode.JoinedGame:      return "Joined a game, waiting to start";
                case ProcessInfo.StatusCode.PlayingGame:     return "Playing ( In game )";
                case ProcessInfo.StatusCode.LeavingGame:     return "Logging out";
                case ProcessInfo.StatusCode.Won:             return "Won!!!";
                case ProcessInfo.StatusCode.Lost:            return "Lost :(";
                case ProcessInfo.StatusCode.Terminating:     return "Process Terminating";
                case ProcessInfo.StatusCode.Tied:            return "Tied :/";
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

        public void GetGameList() {
            log.Debug("In Process GetGameList function.");
            (State as PlayerState).InitialLifePoints = 0;
            (State as PlayerState).OpenGames = null;
            var success = SubSystem.Dispatcher.DispatchConversation<GameListConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.JoiningGame );
        }

        public void JoinGame() {
            log.Debug("In Process JoinGame function.");
            var success = SubSystem.Dispatcher.DispatchConversation<JoinGameConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.JoinedGame );
        }

        public void LeavingGame() {
            log.Debug("In Process LeavingGame function.");
            State.CurrentMessage = "Game is ending without ranking.";
            GameEnd();
        }
        public void Tied() {
            log.Debug("In Process Tied function.");
            GameEnd();
        }
        public void Won() {
            log.Debug("In Process Won function.");
            GameEnd();
        }
        public void Lost() {
            log.Debug("In Process Lost function.");
            GameEnd();
        }
        public void GameEnd() {
            log.Debug("In Process GameEnd function.");
            Thread.Sleep(3000);
            SetStatus(ProcessInfo.StatusCode.Registered);
        }
    }
}

using System.Threading;
using CommunicationLayer;
using BalloonStore.Conversation;
using SharedObjects;
using log4net;
using System;
using ProcessCommon.Conversation;

namespace BalloonStore
{
    public class BalloonStoreProcess : CommunicationProcess  {
        private static readonly ILog log = LogManager.GetLogger(typeof(BalloonStoreProcess));

        public BalloonStoreState BalloonStoreState {
            get { return  (BalloonStoreState)base.State; }
        }

        public BalloonStoreProcess(): base( new BalloonStoreState() ) {
            SetStatus(ProcessInfo.StatusCode.NotInitialized);
        }

        public void initializeSubsystem(string registryEp) {
            log.Debug("Initializing BalloonStore Subsystem.");
            initializeSubsystem(new BalloonStoreConversationFactory());
            SubSystem.EndpointLookup.Add("Registry", new PublicEndPoint(registryEp));
        }

        public void initializeBalloonStore( string alias ) {
            log.Debug("Initializing BalloonStore Details.");
            BalloonStoreState.ProcessInfo.Label = alias;
        }

        public void Reset() {
            BalloonStoreState.Pennies = new ResourceSet<Penny>();
            BalloonStoreState.Balloons = new ResourceSet<Balloon>();
            BalloonStoreState.CurrentGame = new GameInfo();
            BalloonStoreState.FilledBalloons = new ResourceSet<Balloon>();
            BalloonStoreState.MyBalloonStore = new MyUtilities.BindableGameProcessData();
        }
        
        override protected void Process(object state) {
            SubSystem.Dispatcher.Start();
            while( KeepGoing ) {
                //log.Debug("------------- At top of BalloonStore process loop -------------");
                switch ( BalloonStoreState.Status ) {
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
            while(BalloonStoreState.Status == ProcessInfo.StatusCode.PlayingGame) {
                Thread.Sleep(100);
            }
        }

        override protected void SetStatus(ProcessInfo.StatusCode status) {
            if(StatusIsPossible(status)) {
                BalloonStoreState.ProcessInfo.Status = status;
                string message = GetMessageFromStatus(status);
                if( message != null ) {
                     BalloonStoreState.CurrentMessage = message;
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
            BalloonStoreState.CurrentMessage = "Game is ending without ranking.";
        }

        async public void Logout(Action<bool> callback) {
            log.Debug("In Process Logout function.");
            BalloonStoreState.CurrentMessage = "Requesting log out";
            var success = await SubSystem.Dispatcher.DispatchConversationAsync<LogoutConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.NotInitialized );
            callback(success);
        }
    }
}
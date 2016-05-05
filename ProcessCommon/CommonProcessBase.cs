using CommunicationLayer;
using log4net;
using System;
using SharedObjects;
using ProcessCommon.Conversation;
using System.Threading;

namespace ProcessCommon
{
    public abstract class CommonProcessBase : CommunicationProcess {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommonProcessBase));
        public CommonProcessBase(ProcessState _state, ConversationFactory factory, int minPort, int maxPort) : base(_state, factory, minPort, maxPort) {}


        override protected void Process(object state) {
            SubSystem.Dispatcher.Start();
            while( KeepGoing ) {
                DecideMainAction();
                Thread.Sleep(100);
            }
            SubSystem.Dispatcher.Stop();
        }

        public virtual void Login(){
            State.SetStatus( ProcessInfo.StatusCode.Initializing );
            log.Debug( "In Process Login function." );
            var success = SubSystem.Dispatcher.DispatchConversation<LoginConversation>();
            if( success ) State.SetStatus( ProcessInfo.StatusCode.Registered );
            else State.SetStatus( ProcessInfo.StatusCode.NotInitialized );
        }

        public virtual void DecideMainAction(){
            switch ( State.Status ) {
                case ProcessInfo.StatusCode.NotInitialized:     Login(); break;
                case ProcessInfo.StatusCode.JoiningGame:     JoinGame(); break;
                case ProcessInfo.StatusCode.LeavingGame:    LeaveGame(); break;
                case ProcessInfo.StatusCode.JoinedGame:
                case ProcessInfo.StatusCode.PlayingGame:     PlayGame(); break;
                case ProcessInfo.StatusCode.Tied:           
                case ProcessInfo.StatusCode.Lost:    
                case ProcessInfo.StatusCode.Won:            ResetGame(); break;        
            }
        }

        protected virtual void PlayGame() {
            switch(State.CurrentGame.Status) {
                case GameInfo.StatusCode.Cancelled:  ResetGame("Game Cancelled"); break;
                case GameInfo.StatusCode.Ending:     LeavingGameWithoutRanking(); break;
                case GameInfo.StatusCode.Complete:   ResetGame("Game Complete");  break;
            }
        }

        public void LeavingGameWithoutRanking(){
            log.Debug("In Process LeavingGame function.");
            ResetGame("Game is ending without ranking.");
        }
        public virtual void ResetGame(string message="") {
            log.Debug("Reset Game: " + message);
            if(!string.IsNullOrWhiteSpace(message)) State.CurrentMessage = message;
            Thread.Sleep(3000);
            State.CurrentGame.Reset();
        }

        public abstract void JoinGame();
        protected virtual void JoinGame<T>() where T : AbstractJoinGameConversation {
            log.Debug("In Process JoinGame function.");
            var success = SubSystem.Dispatcher.DispatchConversation<T>();
            if( success ) State.SetStatus( ProcessInfo.StatusCode.JoinedGame );
        }

        public virtual bool LeaveGame() {
            if(State.CurrentGame.GameManagerId != 0) {
                State.SetStatus(ProcessInfo.StatusCode.LeavingGame);
                log.Debug("In Process LeaveGame function.");
                var success = SubSystem.Dispatcher.DispatchConversation<LeaveGameConverastion>();
                if( !success ) State.SetStatus(ProcessInfo.StatusCode.Unknown);
                return success;
            }
            else return true;
        }

        public override void Logout(Action<bool> callback) {
            log.Debug("In Process Logout function.");
            if (LeaveGame()) {
                State.CurrentMessage = "Requesting log out";
                var success = SubSystem.Dispatcher.DispatchConversation<LogoutConversation>();
                if( success ) {
                    State.SetStatus( ProcessInfo.StatusCode.NotInitialized );
                    callback(success);
                }
            }
        }
    }
}

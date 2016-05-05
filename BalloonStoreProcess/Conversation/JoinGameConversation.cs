using SharedObjects;
using ProcessCommon.Conversation;

namespace BalloonStoreProcess.Conversation {
    public class JoinGameConversation : AbstractJoinGameConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(JoinGameConversation));
        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }
        override protected bool SetAndVerifyIds() {
            if ( BalloonStoreState.GameManagerId != 0 && BalloonStoreState.GameId != 0 ) {
                GameManagerId = BalloonStoreState.GameManagerId;
                GameId = BalloonStoreState.GameId;
                return true;
            }
            if( BalloonStoreState.GameManagerId == 0 && BalloonStoreState.GameId == 0 ) 
                return MessageFailure("Unexpected GameId of zero, and unexpected GameManagerId of zero");
            else if( BalloonStoreState.GameManagerId == 0)
                 return MessageFailure("Unexpected GameManagerId of zero");
            else return MessageFailure("Unexpected GameId of zero");
        }
        protected override bool ProcessReply() {
            if ( base.ProcessReply() == true ) {
                SubSystem.State.SetStatus(ProcessInfo.StatusCode.JoinedGame);
                return MessageSuccess();
            }
            else return false;
        }
    }
}
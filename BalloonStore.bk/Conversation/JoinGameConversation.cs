using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using ProcessCommon.Conversation;
using Utils;

namespace BalloonStore.Conversation {
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
                MessageFailure("Unexpected GameId of zero, and unexpected GameManagerId of zero");
            else if( BalloonStoreState.GameManagerId == 0)
                 MessageFailure("Unexpected GameManagerId of zero");
            else MessageFailure("Unexpected GameId of zero");
            return false;
        }
    }
}
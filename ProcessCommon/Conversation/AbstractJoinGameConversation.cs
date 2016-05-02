using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace ProcessCommon.Conversation {
    public abstract class AbstractJoinGameConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractJoinGameConversation));

        protected int GameId        { get; set; }
        protected int GameManagerId { get; set; }

        protected abstract bool SetAndVerifyIds();
        protected virtual bool ProcessSuccess(JoinGameReply reply) {
            SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            return true;
        }
        protected virtual void ProcessFailure(JoinGameReply reply) {
            if ( IncomingMessage == null ) MessageFailure("JoinGameReply was null.");
            else if( reply == null )       MessageFailure("Error while casting message to JoinGameReply.");
            else MessageFailure("Join game request was unsuccessfull, Reason: " + reply.Note);
        }

        override protected bool CreateRequest() {
            if ( SetAndVerifyIds() == true ) { 
                log.Debug("Queuing join game request. ");
                log.Debug("Requesting GameId: "+ GameId);
                OutgoingMessage = RouteTo( new JoinGameRequest {
                    GameId = GameId,
                    Process = SubSystem.State.ProcessInfo,
                },  GameManagerId );
                return true;
            }
            return false;
        }

        override protected bool ProcessReply() {
            JoinGameReply reply = null;
            // Handle a game join reply
            if( IncomingMessage != null ) reply = Cast<JoinGameReply>(IncomingMessage);
            if ( reply != null && reply.Success == true) {
                if( ProcessSuccess(reply)){
                    Success = true;
                    return true;
                }
            }
            ProcessFailure(reply);
            return false;
        }
    }
}

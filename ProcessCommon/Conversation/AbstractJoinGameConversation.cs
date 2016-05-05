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

        override protected bool CreateRequest() {
            if ( SetAndVerifyIds() == true ) { 
                log.Debug("Queuing join game request, Requesting GameId: "+ GameId);
                OutgoingMessage = SubSystem.AddressManager.RouteTo( new JoinGameRequest {
                    GameId  = GameId,
                    Process = SubSystem.State.ProcessInfo,
                },  GameManagerId );
                return true;
            }
            return false;
        }

        override protected bool ProcessReply() {
            var reply = IncomingMessage.Unwrap<JoinGameReply>();
                 if ( IncomingMessage == null  ) return MessageFailure("JoinGameReply was null.");
            else if ( reply           == null  ) return MessageFailure("Error while casting message to JoinGameReply.");
            else if ( reply.Success   == false ) return MessageFailure("JoinGameReply.Success was false, Note: "+reply.Note);
            else return true;
        }
    }
}

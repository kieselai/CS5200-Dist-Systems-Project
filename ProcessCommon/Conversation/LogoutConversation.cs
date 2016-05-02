using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace ProcessCommon.Conversation
{
    public class LogoutConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogoutConversation));
        override protected bool CreateRequest(){
            log.Debug("Queuing logout request. ");
            OutgoingMessage = AddressTo( new LogoutRequest(),"Registry");
            return true;
        }
        override protected bool ProcessReply(){
            var logoutReply = Cast<Reply>(IncomingMessage);
            if ( logoutReply.Success ) {
                Success = true;
                return true;
            }
            else MessageFailure("Logout request failed");
            return false;
        }
    }
}

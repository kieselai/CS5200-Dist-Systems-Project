using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace PlayerProcess.Conversation
{
    public class UmbrellaLoweredConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UmbrellaLoweredConversation));

        private PlayerState PlayerState {
            get { return SubSystem.State as PlayerState; }
        }
        
        override protected bool ProcessRequest() {
            var notification = IncomingMessage.Unwrap<UmbrellaLoweredNotification>();
            if(IncomingMessage == null)
                return MessageFailure("Umbrella Lowered Notification was null");
            else if (notification == null)
                return MessageFailure("Failed to cast Umbrella Lowered Notification");
            else return true;
        }

        protected override bool CreateResponse() {
            OutgoingMessage = SubSystem.AddressManager.RouteTo(new Reply {
                Note = "Received",
                Success = true
            }, IncomingMessage );
            return true;
        }
    }
}

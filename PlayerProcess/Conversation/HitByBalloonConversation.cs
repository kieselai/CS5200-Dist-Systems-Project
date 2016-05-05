using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace PlayerProcess.Conversation
{
    public class HitByBalloonConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HitByBalloonConversation));

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        
        override protected bool ProcessRequest() {
            var hitNotification = IncomingMessage.Unwrap<HitNotification>();
            if(IncomingMessage == null)
                return MessageFailure("Hit Notification was null");
            else if (hitNotification == null)
                return MessageFailure("Failed to cast Hit Notification");
            else {
                PlayerState.HitPoints += 1;
                Success = true;
                return true;
            }
        }

        protected override bool CreateResponse() {
            OutgoingMessage = SubSystem.AddressManager.RouteTo(new Reply {
                Note = "Received",
                Success = true
            }, PlayerState.CurrentGame.GameManagerId );
            return true;
        }
    }
}

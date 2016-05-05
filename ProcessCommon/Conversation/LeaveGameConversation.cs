using CommunicationLayer;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class LeaveGameConverastion : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LeaveGameConverastion));
        protected override bool CreateRequest() {
            OutgoingMessage = SubSystem.AddressManager.RouteTo(new LeaveGameRequest{}, SubSystem.State.CurrentGame.GameManagerId);
            return true;
        }

        protected override bool ProcessReply() {
            var leaveGameReply = IncomingMessage.Unwrap<Reply>();
            if(IncomingMessage == null)
                return MessageFailure("Incoming message is null");
            else if(leaveGameReply == null )
                return MessageFailure("Leave game reply is null");
            else if( leaveGameReply.Success == false )
                return MessageFailure("LeaveGameReply.Success was false, Note: " + leaveGameReply.Note);
            else {
                SubSystem.State.CurrentGame.Reset();
                return MessageSuccess();
            }
        }
    }
}

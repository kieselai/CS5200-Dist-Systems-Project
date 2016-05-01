using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using System;
using Messages.RequestMessages;

namespace Player.Conversation
{
    public class LeaveGameConverastion : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LeaveGameConverastion));
        protected override bool CreateRequest() {
            OutgoingMessage = RouteTo(new LeaveGameRequest{}, (SubSystem.State as PlayerState).CurrentGame.GameManagerId);
            return true;
        }

        protected override bool ProcessReply() {
            var leaveGameReply = Cast<Reply>(IncomingMessage);
            if(IncomingMessage == null) {
                MessageFailure("Incoming message is null");
                return false;
            }
            else if(leaveGameReply == null ) {
                MessageFailure("Leave game reply is null");
                return false;
            }
            else if( leaveGameReply.Success == false ) {
                MessageFailure("Leave game message was set as unsuccessful");
                return false;
            }
            return true;
        }
    }
}

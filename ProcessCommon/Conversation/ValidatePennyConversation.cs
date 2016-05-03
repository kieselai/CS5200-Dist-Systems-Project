using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class ValidatePennyConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ValidatePennyConversation));

        public Penny[] PenniesToValidate { get; set; }
        override protected bool CreateRequest(){
            log.Debug("Queuing Validate Penny request. ");
            OutgoingMessage = AddressTo( new PennyValidation {
                Pennies = PenniesToValidate
            },"Registry");
            return true;
        }
        override protected bool ProcessReply(){
            var reply = Cast<Reply>(IncomingMessage);
            if ( reply.Success ) {
                Success = true;
                return true;
            }
            else MessageFailure("Penny validation failed, Note: "+ reply.Note);
            return false;
        }
    }
}

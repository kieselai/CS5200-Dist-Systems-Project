using CommunicationLayer;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class GetPublicKeyConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GetPublicKeyConversation));

        public PublicKeyReply PublicKeyReply { get; set; }
        public int ProcessId { get; set; }

        override protected bool ProcessReply(){
            var reply = IncomingMessage.Unwrap<PublicKeyReply>();
            if (IncomingMessage == null )
                 return MessageFailure("Public key reply was null.");
            else if(reply == null)
                return MessageFailure("Cast to PublicKeyReply failed");
            else if (reply.Success == false)
                 return MessageFailure("PublicKeyReply was unsuccessful");
            else {
                PublicKeyReply = reply;
                return MessageSuccess();
            }
        }

        override protected bool CreateRequest(){
            log.Debug("Creating login request. ");
            OutgoingMessage  = SubSystem.AddressManager.AddressTo( new GetKeyRequest {
                ProcessId = ProcessId 
            }, "Registry");
            return true;
        }
    }
}
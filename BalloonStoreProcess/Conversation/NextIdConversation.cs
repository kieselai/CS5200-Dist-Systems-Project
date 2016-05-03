using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Linq;
using System.Collections.Generic;

namespace BalloonStoreProcess.Conversation
{
     public class NextIdConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NextIdConversation));

        public int NumOfIds { get; set; }
        public int NextId { get; set; }

        override protected bool CreateRequest() {
            log.Debug("Creating NextId request. ");
            OutgoingMessage = AddressTo( new NextIdRequest {
                NumberOfIds = NumOfIds
            }, "Registry");
            return true;
        }
        override protected bool ProcessReply() {
            var reply = Cast<NextIdReply>( IncomingMessage );
            if (IncomingMessage == null ) {
                 MessageFailure("NextId Reply was null.");
                return false;
            }
            else if(reply == null) {
                MessageFailure("Cast to NextIdReply failed");
                return false;
            }
            else if (reply.Success == false) {
                 MessageFailure("NextIdReply.Success was false, Note: "+reply.Note);
                return false;
            }
            else {
                NextId = reply.NextId;
                NumOfIds = reply.NumberOfIds;
                Success = true;
                return true;
            }
        }
    }
}

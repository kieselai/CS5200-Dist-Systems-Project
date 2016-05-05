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
        public int NextId   { get; set; }

        override protected bool CreateRequest() {
            log.Debug("Creating NextId request. ");
            OutgoingMessage = SubSystem.AddressManager.AddressTo( new NextIdRequest { NumberOfIds = NumOfIds }, "Registry");
            return true;
        }
        override protected bool ProcessReply() {
            var reply = IncomingMessage.Unwrap<NextIdReply>();
                 if ( IncomingMessage == null  ) return MessageFailure("NextId Reply was null.");
            else if ( reply           == null  ) return MessageFailure("Cast to NextIdReply failed");
            else if ( reply.Success   == false ) return MessageFailure("NextIdReply.Success was false, Note: "+reply.Note);
            else {
                NextId   = reply.NextId;
                NumOfIds = reply.NumberOfIds;
                Success  = true;
                return true;
            }
        }
    }
}

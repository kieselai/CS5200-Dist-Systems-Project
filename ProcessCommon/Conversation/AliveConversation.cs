using CommunicationLayer;
using Messages.ReplyMessages;
using System;
using Messages.RequestMessages;

namespace ProcessCommon.Conversation
{
    public class AliveConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AliveConversation));
        protected override bool ProcessRequest() {
            var aliveRequest = Cast<AliveRequest>(IncomingMessage);
            log.Debug( aliveRequest.ToString() );
            return true;
        }

        protected override bool CreateResponse() {
            log.Debug("Creating Alive Response");
            OutgoingMessage = AddressTo(new Reply {Success = true, Note = "I'm Alive"}, "Registry");
            SubSystem.State.ProcessInfo.AliveTimestamp = DateTime.Now;
            return true;
        }
    }
}

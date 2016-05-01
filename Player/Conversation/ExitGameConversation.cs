using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using System;
using Messages.RequestMessages;
using System.Threading;

namespace Player.Conversation
{
    public class ExitGameConversation :  ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AliveConversation));
        protected override bool ProcessRequest() {
            var req = Cast<ExitGameRequest>(IncomingMessage);
            if(IncomingMessage == null) { log.Error("Message is null");                   return false; }
            else if( req == null)       { log.Error("Exit game requst message was null"); return false; }
            else return true;
        }

        protected override bool CreateResponse() {
            SubSystem.State.CurrentMessage = "Exit Game Request received";

            RouteTo(new Reply { Note="Message received", Success = true });
            SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.Registered;
            return true;
        }
    }
}

using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using System.Threading;

namespace ProcessCommon.Conversation
{
    public class ExitGameConversation :  ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExitGameConversation));
        protected override bool ProcessRequest() {
            var req = IncomingMessage.Unwrap<ExitGameRequest>();
            if(IncomingMessage == null) { log.Error("Message is null");                   return false; }
            else if( req == null)       { log.Error("Exit game requst message was null"); return false; }
            else return true;
        }

        protected override bool CreateResponse() {
            SubSystem.State.CurrentMessage = "Exit Game Request received";
            SubSystem.AddressManager.RouteTo(new Reply { Note="Message received", Success = true });
            ThreadPool.QueueUserWorkItem(ExitGameMessage, null);
            return true;
        }

        protected void ExitGameMessage(object state=null) {
            Thread.Sleep(1000);
            SubSystem.State.CurrentMessage = "Exiting Game";
            SubSystem.State.CurrentGame.Reset();
            Thread.Sleep(2000);
            SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.Registered;
        }
    }
}

using CommunicationLayer;
using Messages.RequestMessages;

namespace ProcessCommon.Conversation
{
    public class ShutdownConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ShutdownConversation));
        protected override bool ProcessIncoming() {
            var shutdownReq = Cast<ShutdownRequest>(IncomingMessage);
            log.Debug( shutdownReq.ToString() );
            SubSystem.State.IsShutDown = true;
            return true;
        }
    }
}

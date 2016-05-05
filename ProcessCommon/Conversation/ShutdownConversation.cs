using CommunicationLayer;
using Messages.RequestMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class ShutdownConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ShutdownConversation));
        protected override bool ProcessIncoming() {
            var shutdownReq = IncomingMessage.Unwrap<ShutdownRequest>();
            log.Debug( shutdownReq.ToString() );
            SubSystem.State.SetStatus(ProcessInfo.StatusCode.Terminating);
            return true;
        }
    }
}

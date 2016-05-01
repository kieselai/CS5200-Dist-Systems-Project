using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using System;
using Messages.RequestMessages;

namespace Player.Conversation
{
    public class ShutdownConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AliveConversation));
        protected override bool ProcessIncoming() {
            var shutdownReq = Cast<ShutdownRequest>(IncomingMessage);
            log.Debug( shutdownReq.ToString() );
            SubSystem.State.IsShutDown = true;
            return true;
        }
    }
}

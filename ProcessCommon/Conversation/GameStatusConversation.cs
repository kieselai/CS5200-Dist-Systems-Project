using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
namespace ProcessCommon.Conversation
{
    public class GameStatusConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStatusConversation));
        protected override bool ProcessIncoming() {
            var gameMsg = IncomingMessage.Unwrap<GameStatusNotification>();
            if(gameMsg != null) {
                SubSystem.State.CurrentGame = gameMsg.Game;
                return MessageSuccess();
            }
            else return MessageFailure("Unable to cast Game status message.");
        }
    }
}

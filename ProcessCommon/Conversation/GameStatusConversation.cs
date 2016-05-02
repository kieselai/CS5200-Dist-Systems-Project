using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
namespace ProcessCommon.Conversation
{
    public class GameStatusConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStatusConversation));
        protected override bool ProcessIncoming() {
            var gameMsg = Cast<GameStatusNotification>(IncomingMessage);
            if(gameMsg != null) {
                SubSystem.State.CurrentGame = gameMsg.Game;
                if(gameMsg.Game.Status == GameInfo.StatusCode.Ending) {
                    log.Warn("Game status: Game ending");
                    SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
                }
                Success = true;
                return true;
            }
            else {
                MessageFailure("Unable to cast Game status message.");
                return false;
            }
        }
    }
}

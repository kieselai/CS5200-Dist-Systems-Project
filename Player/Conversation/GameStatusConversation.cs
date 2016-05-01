using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using System.Linq;
using Messages.RequestMessages;
namespace Player.Conversation
{
    public class GameStatusConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStatusConversation));
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        protected override bool ProcessIncoming() {
            var gameMsg = Cast<GameStatusNotification>(IncomingMessage);
            if(gameMsg != null) {
                PlayerState.CurrentGame = gameMsg.Game;
                var myInfo = gameMsg.Game.CurrentProcesses.Where( p=> p.ProcessId == PlayerState.ProcessInfo.ProcessId ).FirstOrDefault();
                if(myInfo != null) {
                    PlayerState.MyPlayer = myInfo;
                }
                if(gameMsg.Game.Status == GameInfo.StatusCode.Ending) {
                    log.Warn("Game status: Game ending");
                    PlayerState.ProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
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

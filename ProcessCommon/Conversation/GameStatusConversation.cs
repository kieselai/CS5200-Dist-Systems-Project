using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using System.Linq;

namespace ProcessCommon.Conversation
{
    public class GameStatusConversation : ReceivedMulticast {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStatusConversation));
        protected override bool ProcessIncoming() {
            var gameMsg = IncomingMessage.Unwrap<GameStatusNotification>();
            if(gameMsg != null) {
                SubSystem.State.CurrentGame = gameMsg.Game;
                var thisProcess = from proc in gameMsg.Game.CurrentProcesses
                                  where proc.ProcessId == SubSystem.State.ProcessInfo.ProcessId
                                  select proc;
                SubSystem.State.ThisGameProc = thisProcess.FirstOrDefault();
                return MessageSuccess();
            }
            else return MessageFailure("Unable to cast Game status message.");
        }
    }
}

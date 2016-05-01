using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;

namespace Player.Conversation
{
    public class GameStartConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStartConversation));
        protected override bool ProcessRequest() {
            return true;
        }
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }

        protected override bool CreateResponse() {
            log.Debug("Responding to Game Start Request");
            OutgoingMessage = RouteTo(new StartGame { Success = true, Note = "Ready!" }, PlayerState.CurrentGame.GameManagerId );
            PlayerState.ProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            return true;
        }
    }
}
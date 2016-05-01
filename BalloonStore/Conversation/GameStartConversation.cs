using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;

namespace BalloonStore.Conversation
{
    public class GameStartConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameStartConversation));
        protected override bool ProcessRequest() {
            return true;
        }
        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }

        protected override bool CreateResponse() {
            log.Debug("Responding to Game Start Request");
            OutgoingMessage = RouteTo(new StartGame { Success = true, Note = "Ready!" }, BalloonStoreState.CurrentGame.GameManagerId );
            BalloonStoreState.ProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            return true;
        }
    }
}
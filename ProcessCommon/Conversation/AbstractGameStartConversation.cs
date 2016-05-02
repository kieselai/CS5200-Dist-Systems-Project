using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;

namespace ProcessCommon.Conversation
{
    public abstract class AbstractGameStartConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractGameStartConversation));
        protected int GameManagerId { get; set; }

        protected abstract bool SetAndVerifyIds();
        protected override bool ProcessRequest() {
            return SetAndVerifyIds();
        }

        protected override bool CreateResponse() {
            log.Debug("Responding to Game Start Request");
            OutgoingMessage = RouteTo(new StartGame { Success = true, Note = "Ready!" }, GameManagerId );
            SubSystem.State.ProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            return true;
        }
    }
}
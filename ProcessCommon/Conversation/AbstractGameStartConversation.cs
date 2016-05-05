using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using MyUtilities;

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
            OutgoingMessage = SubSystem.AddressManager.RouteTo(new StartGame { Success = true, Note = "Ready!" }, GameManagerId );
            ThreadUtil.RunAfterDelay( ()=>SubSystem.State.SetStatus( ProcessInfo.StatusCode.PlayingGame ));
            return true;
        }
    }
}
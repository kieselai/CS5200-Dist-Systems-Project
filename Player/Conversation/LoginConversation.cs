using Messages.RequestMessages;
using SharedObjects;
using ProcessCommon.Conversation;

namespace Player.Conversation
{
    public class LoginConversation : AbstractLoginConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoginConversation));

        override protected bool CreateRequest(){
            log.Debug("Creating login request. ");
            OutgoingMessage = AddressTo( new LoginRequest {
                Identity     = ((PlayerState)SubSystem.State).IdentityInfo,
                ProcessLabel = ((PlayerState)SubSystem.State).ProcessInfo.Label,
                ProcessType  = ProcessInfo.ProcessType.Player
            }, "Registry");
            return true;
        }
    }
}

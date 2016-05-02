using CommunicationLayer;
using Messages.ReplyMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public abstract class AbstractLoginConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractLoginConversation));

        // Each process overrides the abstract CreateRequest function inherited from InitiatedConversation

        override protected bool ProcessReply(){
            var loginReply = Cast<LoginReply>( IncomingMessage );
            if (IncomingMessage == null ) {
                 MessageFailure("Login reply was null.");
                return false;
            }
            else if(loginReply == null) {
                MessageFailure("Cast to LoginReply failed");
                return false;
            }
            else if (loginReply.Success == false) {
                 MessageFailure("LoginReply.Success was false");
                return false;
            }
            else {
                SubSystem.State.ProcessInfo.Info         = loginReply.ProcessInfo;
                MessageNumber.LocalProcessId             = loginReply.ProcessInfo.ProcessId;
                SubSystem.EndpointLookup.Add( "Proxy",     loginReply.ProxyEndPoint );
                log.Debug( "Proxy endpoint is "          + loginReply.PennyBankEndPoint);
                SubSystem.EndpointLookup.Add( "PennyBank", loginReply.PennyBankEndPoint);
                Success = true;
                return true;
            }
        }
    }
}
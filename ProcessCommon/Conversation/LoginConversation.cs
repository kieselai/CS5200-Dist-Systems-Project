using CommunicationLayer;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class LoginConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoginConversation));
        override protected bool ProcessReply(){
            var loginReply = IncomingMessage.Unwrap<LoginReply>();
            if (IncomingMessage == null )
                 return MessageFailure("Login reply was null.");
            else if(loginReply == null)
                return MessageFailure("Cast to LoginReply failed");
            else if (loginReply.Success == false)
                 return MessageFailure("LoginReply.Success was false");
            else {
                SubSystem.State.ProcessInfo.Info = loginReply.ProcessInfo;
                MessageNumber.LocalProcessId     = loginReply.ProcessInfo.ProcessId;
                SubSystem.AddressManager.Lookup.Add( "Proxy", loginReply.ProxyEndPoint );
                log.Debug( "Proxy endpoint is " + loginReply.PennyBankEndPoint);
                SubSystem.AddressManager.Lookup.Add( "PennyBank", loginReply.PennyBankEndPoint);
                return MessageSuccess();
            }
        }

        override protected bool CreateRequest(){
            log.Debug("Creating login request. ");
            OutgoingMessage  = SubSystem.AddressManager.AddressTo( new LoginRequest {
                Identity     = SubSystem.State.IdentityInfo,
                ProcessLabel = SubSystem.State.ProcessInfo.Label,
                ProcessType  = SubSystem.State.ProcessInfo.Type
            }, "Registry");
            if(SubSystem.State.ProcessInfo.Type == ProcessInfo.ProcessType.BalloonStore) {
                OutgoingMessage.Unwrap<LoginRequest>().PublicKey = CryptoService.PublicKey;
            }
            return true;
        }
    }
}
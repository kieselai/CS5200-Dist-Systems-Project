using CommunicationLayer;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;
using System;

namespace BalloonStoreProcess.Conversation
{
    public class LoginConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoginConversation));
        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }

        override protected bool CreateRequest(){
            log.Debug("Creating login request. ");
            OutgoingMessage = AddressTo( new LoginRequest {
                Identity     = BalloonStoreState.IdentityInfo,
                ProcessLabel = BalloonStoreState.ProcessInfo.Label,
                ProcessType  = ProcessInfo.ProcessType.BalloonStore,
                PublicKey    = CryptoService.PublicKey
            }, "Registry");
            return true;
        }

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

using CommunicationLayer;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using MyUtilities;
using SharedObjects;

namespace ProcessCommon.Conversation
{
    public class LogoutConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogoutConversation));
        override protected bool CreateRequest(){
            Chain.Create(ProcessInfo.StatusCode.JoinedGame, ProcessInfo.StatusCode.PlayingGame)
                .Tap((s)=> {
                    if(s==SubSystem.State.Status) {
                        var success = SubSystem.Dispatcher.DispatchConversation<LeaveGameConverastion>();
                        if(success == false) MessageFailure();
                    }
                });
            if(!Failure) {
                log.Debug("Queuing logout request. ");
                OutgoingMessage = SubSystem.AddressManager.AddressTo( new LogoutRequest(),"Registry");
            }
            return !Failure;
        }
        override protected bool ProcessReply(){
            var logoutReply = IncomingMessage.Unwrap<Reply>();
            if ( logoutReply.Success ) return MessageSuccess();
            else return MessageFailure("Logout request failed");
        }
    }
}

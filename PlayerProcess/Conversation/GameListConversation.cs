using CommunicationLayer;
using System.Collections.Concurrent;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace PlayerProcess.Conversation
{
    public class GameListConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameListConversation));
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        override protected bool CreateRequest() {
            log.Debug("Queuing game list request. ");
            OutgoingMessage = SubSystem.AddressManager.AddressTo( new GameListRequest { StatusFilter = 4 }, "Registry");
            return true;
        }
        override protected bool ProcessReply() {
            var gameListReply = IncomingMessage.Unwrap<GameListReply>();
            if(IncomingMessage == null) return MessageFailure("GameListReply was null");
            else if (gameListReply == null)
                return MessageFailure("Failed to cast GameListReply");
            else if( gameListReply.GameInfo.Length == 0 )
                return MessageFailure("No games are available. ");
            else {
                PlayerState.OpenGames = new ConcurrentQueue<GameInfo>(gameListReply.GameInfo); 
                Success = true;
            }
            return true;
        }
    }
}

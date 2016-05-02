using CommunicationLayer;
using System.Collections.Concurrent;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace Player.Conversation
{
    public class GameListConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GameListConversation));
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        override protected bool CreateRequest() {
            log.Debug("Queuing game list request. ");
            OutgoingMessage = AddressTo( new GameListRequest { StatusFilter = 4 }, "Registry");
            return true;
        }
        override protected bool ProcessReply() {
            var gameListReply = Cast<GameListReply>( IncomingMessage );
            if(IncomingMessage == null) {
                MessageFailure("GameListReply was null");
                return false;
            }
            else if (gameListReply == null) {
                MessageFailure("Failed to cast GameListReply");
                return false;
            }
            else if( gameListReply.GameInfo.Length == 0 ) {
                MessageFailure("No games are available. ");
                return false;
            }
            else {
                PlayerState.OpenGames = new ConcurrentQueue<GameInfo>(gameListReply.GameInfo); 
                Success = true;
            }
            return true;
        }
    }
}

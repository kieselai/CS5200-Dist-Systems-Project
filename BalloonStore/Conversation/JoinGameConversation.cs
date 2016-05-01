using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using Messages;
using System;
using Utils;

namespace BalloonStore.Conversation {
    public class JoinGameConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(JoinGameConversation));
        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }
        override protected bool CreateRequest() {
            if ( BalloonStoreState.OpenGames.Count > 0 ) {
                var openGames = BalloonStoreState.OpenGames;
                GameInfo nextAvailableGame = null;
                SyncUtils.WaitForCondition( ()=> openGames.TryDequeue( out nextAvailableGame ) || openGames.Count == 0, 10, 10);
                if ( nextAvailableGame != null ) {
                    log.Debug("Queuing join game request. ");
                    log.Debug("Requesting GameId: "+ nextAvailableGame.GameId);
                    OutgoingMessage = RouteTo( new JoinGameRequest {
                        GameId = nextAvailableGame.GameId,
                        Process = SubSystem.State.ProcessInfo,
                    },  nextAvailableGame.GameManagerId );
                    return true;
                }
            }
            MessageFailure("No games available");
            return false;
        }

        override protected bool ProcessReply() {
            // Handle a game join reply
            var joinGameReply = Cast<JoinGameReply>(IncomingMessage);
            if ( joinGameReply != null && joinGameReply.Success == true && joinGameReply.InitialLifePoints > 0 ) {
                BalloonStoreState.ProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
                Success = true;
                return true;
            }
            else {
                var games = BalloonStoreState.OpenGames;
                if ( games == null || games.Count < 1 ) log.Error("No games available.  ");
                else log.Error("An unknown error occured while attempting to join a game.");
                MessageFailure();
            }
            return false;
        }
    }
}

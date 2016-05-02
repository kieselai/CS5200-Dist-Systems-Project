using SharedObjects;
using Messages.ReplyMessages;
using ProcessCommon.Conversation;
using Utils;

namespace Player.Conversation
{
    public class JoinGameConversation : AbstractJoinGameConversation {
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        override protected bool SetAndVerifyIds() {
            if ( PlayerState.OpenGames != null && PlayerState.OpenGames.Count > 0 ) {
                var openGames = PlayerState.OpenGames;
                GameInfo nextAvailableGame = null;
                SyncUtils.WaitForCondition( ()=> openGames.TryDequeue( out nextAvailableGame ) || openGames.Count == 0, 10, 10);
                if ( nextAvailableGame != null ) {
                    GameId = nextAvailableGame.GameId;
                    GameManagerId = nextAvailableGame.GameManagerId;
                    return true;
                }
            }
            MessageFailure("No games available");
            return false;
        }

        protected override bool ProcessSuccess(JoinGameReply reply) {
            // Handle a game join reply
            if ( reply.InitialLifePoints > 0 ) {
                PlayerState.InitialLifePoints = reply.InitialLifePoints;
                return base.ProcessSuccess(reply);
            }
            else {
                ProcessFailure(reply);
                return false;
            }
        }
        protected override void ProcessFailure(JoinGameReply reply) {
            if( reply != null && reply.InitialLifePoints <= 0 ) {
                MessageFailure("Initial life points is less than or equal to zero.");
            }
            else base.ProcessFailure(reply);
        }
    }
}

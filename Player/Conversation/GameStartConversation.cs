

using ProcessCommon.Conversation;

namespace Player.Conversation
{
    public class GameStartConversation : AbstractGameStartConversation {
        protected override bool SetAndVerifyIds() {
            GameManagerId = SubSystem.State.CurrentGame.GameManagerId;
            return GameManagerId != 0;
        }
    }
}
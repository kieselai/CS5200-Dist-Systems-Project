

using ProcessCommon.Conversation;

namespace PlayerProcess.Conversation
{
    public class GameStartConversation : AbstractGameStartConversation {
        protected override bool SetAndVerifyIds() {
            GameManagerId = SubSystem.State.CurrentGame.GameManagerId;
            return true;
        }
    }
}
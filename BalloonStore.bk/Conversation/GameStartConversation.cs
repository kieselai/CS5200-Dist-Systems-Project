using ProcessCommon.Conversation;

namespace BalloonStore.Conversation
{
    public class GameStartConversation : AbstractGameStartConversation {
        protected override bool SetAndVerifyIds() {
            GameManagerId = (SubSystem.State as BalloonStoreState).GameManagerId;
            return GameManagerId != 0;
        }        
    }
}
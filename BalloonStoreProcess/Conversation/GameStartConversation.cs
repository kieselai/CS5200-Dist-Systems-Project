using ProcessCommon.Conversation;

namespace BalloonStoreProcess.Conversation
{
    public class GameStartConversation : AbstractGameStartConversation {
        protected override bool SetAndVerifyIds() {
            GameManagerId = (SubSystem.State as BalloonStoreState).GameManagerId;
            return true;
        }        
    }
}
using System.Threading;
using PlayerProcess.Conversation;
using SharedObjects;
using log4net;
using ProcessCommon;
using MyUtilities;
using ProcessCommon.Conversation;

namespace PlayerProcess
{
    public class PlayerProcess : CommonProcessBase {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlayerProcess));

        public PlayerProcess(int minPort, int maxPort): base(new PlayerState(), new PlayerConversationFactory(), minPort, maxPort) {
            State.ProcessInfo.Type = ProcessInfo.ProcessType.Player;
        }

        public PlayerState PlayerState { get { return State as PlayerState; } }

        public override void DecideMainAction() {
            if( State.Status == ProcessInfo.StatusCode.Registered) GetGameList();
            else base.DecideMainAction();
        }

        protected override void PlayGame() {
            if (State.CurrentGame.Status == GameInfo.StatusCode.InProgress)
                MakeGameDecisions();
            else base.PlayGame();
        }

        protected void MakeGameDecisions() {
             if( PlayerState.FilledBalloons.AvailableCount > 0)
                ThrowBalloon();
             else if( PlayerState.Balloons.AvailableCount  > 0 && PlayerState.Pennies.AvailableCount > 1)
                FillBalloon();
             else if( PlayerState.Pennies.AvailableCount   > 0)
                BuyBalloon();
        }

        public void BuyBalloon() {
            ThreadUtil.Run( ()=> SubSystem.Dispatcher.DispatchConversation<BuyBalloonConversation>() );
        }
        public void FillBalloon() {
            ThreadUtil.Run( ()=> SubSystem.Dispatcher.DispatchConversation<FillBalloonConversation>() );
        }
        public void ThrowBalloon() {
            ThreadUtil.Run( ()=> SubSystem.Dispatcher.DispatchConversation<ThrowBalloonConversation>() );
        }

        public void GetGameList() {
            log.Debug("In Process GetGameList function.");
            PlayerState.InitialLifePoints = 0;
            PlayerState.OpenGames = null;
            var success = SubSystem.Dispatcher.DispatchConversation<GameListConversation>();
            if( success ) State.SetStatus( ProcessInfo.StatusCode.JoiningGame );
        }

        public override void ResetGame(string message) {
            if( State.CurrentGame.Status == GameInfo.StatusCode.Complete) {
                Chain.Create(ProcessInfo.StatusCode.Won, ProcessInfo.StatusCode.Lost, ProcessInfo.StatusCode.Tied)
                .Tap( (status) => message += (status==State.Status)? " " + State.GetMessageFromStatus(status) : "");
            }
            base.ResetGame(message);
            State.SetStatus(ProcessInfo.StatusCode.Registered);
        }

        public override void JoinGame() { base.JoinGame<JoinGameConversation>(); }
    }
}

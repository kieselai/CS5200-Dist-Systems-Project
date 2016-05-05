using System.Threading;
using BalloonStoreProcess.Conversation;
using SharedObjects;
using log4net;
using ProcessCommon;
using CommunicationLayer;
using System.Linq;
using MyUtilities;

namespace BalloonStoreProcess
{
    public class BalloonStoreProcess : CommonProcessBase {
        private static readonly ILog log = LogManager.GetLogger(typeof(BalloonStoreProcess));
        private Properties.Settings Settings { get { return Properties.Settings.Default; } }

        public BalloonStoreProcess(int minPort, int maxPort): base(new BalloonStoreState(), new BalloonStoreConversationFactory(), minPort, maxPort) {
            State.ProcessInfo.Type = ProcessInfo.ProcessType.BalloonStore;
        }

        public BalloonStoreState BalloonStoreState { get { return State as BalloonStoreState; } }

        public override void DecideMainAction() {
            if ( State.Status == ProcessInfo.StatusCode.Registered) RequestIds();
            else base.DecideMainAction();
        }

        public override void JoinGame() { base.JoinGame<JoinGameConversation>(); }
        
        public override void ResetGame(string message) {
            base.ResetGame(message);
            Logout((success)=> {
                if(success) State.SetStatus(ProcessInfo.StatusCode.Terminating);
            });
        }

        protected void RequestIds() {
            IConversation conv;
            var success = SubSystem.Dispatcher.DispatchConversation<NextIdConversation>(
              out conv,
              (c)=> c.NumOfIds = BalloonStoreState.StartingBalloons
            );
            if(success) {
                var c = conv as NextIdConversation;
                SignBalloons(c.NextId, c.NumOfIds);
                State.SetStatus(ProcessInfo.StatusCode.JoiningGame);
            }
        }

        public void SignBalloons(int NextId, int count) {
            Enumerable.Range(NextId, count).Tap( (i)=> {
                var b = new Balloon {
                    Id = i,
                    SignedBy = State.ProcessInfo.ProcessId,
                    IsFilled = false
                };
                b.DigitalSignature = CryptoService.HashAndSign(b.DataBytes());
                BalloonStoreState.Balloons.AddOrUpdate(b);
            });
        }
    }
}
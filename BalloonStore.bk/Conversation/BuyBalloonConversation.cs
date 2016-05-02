using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Linq;
using System.Collections.Generic;

namespace BalloonStore.Conversation
{
     public class BuyBalloonConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BuyBalloonConversation));

        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }

        // Ensure a new balloon store is chosen if the last one was out.
        protected static int BalloonStoreIndex { get; set; }
        protected Penny ReservedPenny { get; set; }

        protected override void MessageFailure(string msg="") {
            if(ReservedPenny != null) BalloonStoreState.Pennies.Unreserve(ReservedPenny.Id);
            base.MessageFailure(msg);
        }

        protected List<int> BalloonStoreIDs {
            get {
                var query = from process in BalloonStoreState.CurrentGame.CurrentProcesses
                where process.Type == ProcessInfo.ProcessType.BalloonStore
                select process.ProcessId;
                return query.ToList();
            }
        }    

        override protected bool CreateRequest() {
            if(BalloonStoreState.Pennies.AvailableCount < 1) {
                log.Error("No pennies available to spend");
                Failure = true;
                return false;
            }
            
            if(BalloonStoreIDs.Count > 0) {
                ReservedPenny = BalloonStoreState.Pennies.ReserveOne();

                if(BalloonStoreIndex >= BalloonStoreIDs.Count) {
                    BalloonStoreIndex = 0;
                }
                var balloonStore = BalloonStoreIDs[BalloonStoreIndex];
                BalloonStoreIndex++;
                OutgoingMessage = RouteTo( new BuyBalloonRequest {
                    Penny = ReservedPenny
                }, balloonStore );
                return true;
            }
            return false;
        }
        override protected bool ProcessReply() {
            var buyBalloonReply = Cast<BalloonReply>( IncomingMessage );
            if(IncomingMessage == null) {
                MessageFailure("Buy Balloon Reply was null");
                return false;
            }
            else if (buyBalloonReply == null) {
                MessageFailure("Failed to cast buyBalloonReply");
                return false;
            }
            else if( buyBalloonReply.Success == false ) {
                log.Error("Balloon purchase unsuccessful");
                MessageFailure(buyBalloonReply.Note);
                return false;
            }
            else {
                if(ReservedPenny != null) BalloonStoreState.Pennies.MarkAsUsed(ReservedPenny.Id);
                BalloonStoreState.Balloons.AddOrUpdate( buyBalloonReply.Balloon );
                Success = true;
                return true;
            }
        }
    }
}

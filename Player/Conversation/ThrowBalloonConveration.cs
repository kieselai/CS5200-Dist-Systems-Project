using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Linq;
using System.Collections.Generic;

namespace Player.Conversation
{
    public class ThrowBalloonConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FillBalloonConversation));

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        
        protected int PlayerIndex         { get; set; }
        protected Balloon ReservedBalloon { get; set; }

        protected override void MessageFailure(string msg="") {
            if(ReservedBalloon != null) PlayerState.FilledBalloons.Unreserve(ReservedBalloon.Id);
            base.MessageFailure(msg);
        }

        protected List<int> PlayerIDs {
            get {
                var query = from process in PlayerState.CurrentGame.CurrentProcesses
                where process.Type == ProcessInfo.ProcessType.Player
                orderby process.LifePoints, process.HitPoints
                select process.ProcessId;
                return query.ToList();
            }
        }    

        override protected bool CreateRequest() {
            if(PlayerState.FilledBalloons.AvailableCount < 1) {
                log.Error("No balloons available to fill");
                Failure = true;
                return false;
            }
            
            if(PlayerIDs.Count > 0) {
                ReservedBalloon = PlayerState.FilledBalloons.ReserveOne();
                var PlayerId = PlayerIDs.First();
                OutgoingMessage = RouteTo( new ThrowBalloonRequest {
                    Balloon = ReservedBalloon,
                    TargetPlayerId = PlayerId
                }, PlayerState.CurrentGame.GameManagerId);
                return true;
            }
            return false;
        }
        override protected bool ProcessReply() {
            var throwBalloonReply = Cast<BalloonReply>( IncomingMessage );
            if(IncomingMessage == null) {
                MessageFailure("Throw Balloon Reply was null");
                return false;
            }
            else if (throwBalloonReply == null) {
                MessageFailure("Failed to cast throwBalloonReply");
                return false;
            }
            else if( throwBalloonReply.Success == false ) {
                log.Error("Unable to throw balloon");
                MessageFailure(throwBalloonReply.Note);
                return false;
            }
            else {
                PlayerState.FilledBalloons.MarkAsUsed(ReservedBalloon.Id);
                Success = true;
                return true;
            }
        }
    }
}

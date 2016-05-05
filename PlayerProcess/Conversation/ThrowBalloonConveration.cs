using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Linq;
using System.Collections.Generic;

namespace PlayerProcess.Conversation
{
    public class ThrowBalloonConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FillBalloonConversation));

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        
        protected int PlayerIndex         { get; set; }
        protected Balloon ReservedBalloon { get; set; }

        protected override bool MessageFailure(string msg="") {
            if(ReservedBalloon != null) PlayerState.FilledBalloons.Unreserve(ReservedBalloon.Id);
            return base.MessageFailure(msg);
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
                OutgoingMessage = SubSystem.AddressManager.RouteTo( new ThrowBalloonRequest {
                    Balloon = ReservedBalloon,
                    TargetPlayerId = PlayerId
                }, PlayerState.CurrentGame.GameManagerId);
                return true;
            }
            return false;
        }
        override protected bool ProcessReply() {
            var throwBalloonReply = IncomingMessage.Unwrap<BalloonReply>();
            if(IncomingMessage == null) {
                return MessageFailure("Throw Balloon Reply was null");
            }
            else if (throwBalloonReply == null) {
                return MessageFailure("Failed to cast throwBalloonReply");
            }
            else if( throwBalloonReply.Success == false ) {
                return MessageFailure("Unable to throw balloon, Note: " + throwBalloonReply.Note);
            }
            else {
                PlayerState.FilledBalloons.MarkAsUsed(ReservedBalloon.Id);
                Success = true;
                return true;
            }
        }
    }
}

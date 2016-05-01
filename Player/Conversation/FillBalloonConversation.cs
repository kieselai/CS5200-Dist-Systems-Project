using CommunicationLayer;
using System.Collections.Concurrent;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using Messages;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Player.Conversation
{
     public class FillBalloonConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FillBalloonConversation));

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }

        public FillBalloonConversation(){
            ReservedPennies = new Penny[2];
        }

        // Ensure a new balloon store is chosen if the last one was out.
        protected static int BalloonStoreIndex { get; set; }
        protected Penny[] ReservedPennies { get; set; }
        protected Balloon ReservedBalloon { get; set; }

        protected override void MessageFailure(string msg="") {
            if(ReservedPennies != null && ReservedPennies.Length == 2) {
                 foreach(var penny in ReservedPennies) {
                    if(penny != null ) PlayerState.Pennies.Unreserve(penny.Id);
                }
            }
            if(ReservedBalloon != null) PlayerState.Balloons.Unreserve(ReservedBalloon.Id);
            base.MessageFailure(msg);
        }

        protected List<int> BalloonStoreIDs {
            get {
                var query = from process in PlayerState.CurrentGame.CurrentProcesses
                where process.Type == ProcessInfo.ProcessType.BalloonStore
                select process.ProcessId;
                return query.ToList();
            }
        }    

        override protected bool CreateRequest() {
            if(PlayerState.Pennies.AvailableCount < 2) {
                log.Error("No pennies available to spend");
                Failure = true;
                return false;
            }
            if(PlayerState.Balloons.AvailableCount < 1) {
                log.Error("No balloons available to fill");
                Failure = true;
                return false;
            }
            
            if(BalloonStoreIDs.Count > 0) {
                ReservedPennies = PlayerState.Pennies.ReserveMany(2);
                ReservedBalloon = PlayerState.Balloons.ReserveOne();
                if(BalloonStoreIndex >= BalloonStoreIDs.Count) {
                    BalloonStoreIndex = 0;
                }
                var balloonStore = BalloonStoreIDs[BalloonStoreIndex];
                BalloonStoreIndex++;

                OutgoingMessage = RouteTo( new FillBalloonRequest {
                    Balloon = ReservedBalloon, 
                    Pennies = ReservedPennies
                }, balloonStore );
                return true;
            }
            return false;
        }
        override protected bool ProcessReply() {
            var fillBalloonReply = Cast<BalloonReply>( IncomingMessage );
            if(IncomingMessage == null) {
                MessageFailure("Fill Balloon Reply was null");
                return false;
            }
            else if (fillBalloonReply == null) {
                MessageFailure("Failed to cast fillBalloonReply");
                return false;
            }
            else if( fillBalloonReply.Success == false ) {
                log.Error("Unable to fill balloon");
                MessageFailure(fillBalloonReply.Note);
                return false;
            }
            else {
                PlayerState.Pennies.MarkAsUsed(ReservedPennies.Select(p=>p.Id).ToArray());
                PlayerState.Balloons.MarkAsUsed(ReservedBalloon.Id);
                PlayerState.FilledBalloons.AddOrUpdate( fillBalloonReply.Balloon );
                Success = true;
                return true;
            }
        }
    }
}

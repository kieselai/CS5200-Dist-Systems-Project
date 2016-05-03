using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using ProcessCommon.Conversation;
using System;
using Messages.ReplyMessages;
using Messages;

namespace BalloonStoreProcess.Conversation
{
     public class BuyBalloonConversation : ResponseConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BuyBalloonConversation));

        private BalloonStoreState BalloonStoreState {
            get { return ((BalloonStoreState)SubSystem.State); }
        }

        private bool ValidationResult   { get; set; }
        private bool BalloonsAvailable  { get; set; }
        private Balloon ReservedBalloon { get; set; }

        protected override bool ProcessRequest() {
            var request = Cast<BuyBalloonRequest>(IncomingMessage);
            BalloonsAvailable = BalloonStoreState.Balloons.AvailableCount > 0;
            ReservedBalloon = null;
            if( BalloonsAvailable ) {
                ReservedBalloon = BalloonStoreState.Balloons.ReserveOne();
                ValidationResult = SubSystem.Dispatcher.DispatchConversation<ValidatePennyConversation>(
                  (conv)=> { conv.PenniesToValidate = new Penny[] { request.Penny }; }
                );
                if(ValidationResult == false ) {
                    BalloonStoreState.Balloons.Unreserve(ReservedBalloon.Id);
                    ReservedBalloon = null;
                }
            }
            return true;
        }

        protected override bool CreateResponse() {
            OutgoingMessage = RouteTo(new BalloonReply {
                Success = BalloonsAvailable && ValidationResult,
                Balloon = ReservedBalloon
            }, IncomingMessage);
            return true;
        }
    }
}

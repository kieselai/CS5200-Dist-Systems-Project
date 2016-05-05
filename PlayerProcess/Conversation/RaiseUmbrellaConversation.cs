using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;

using System.Collections.Generic;
using MyUtilities;
using System;

namespace PlayerProcess.Conversation
{
    public class RaiseUmbrellaConversation : InitiatedConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RaiseUmbrellaConversation));
        private PlayerState PlayerState { get { return SubSystem.State as PlayerState; } }

        protected override bool MessageFailure(string failMessage = "") {
            if(ReservedUmbrella != null) {
                PlayerState.Umbrellas.Unreserve(ReservedUmbrella.Id);
            }
            return base.MessageFailure(failMessage);
        }

        public Umbrella ReservedUmbrella { get; set; }
        protected override bool CreateRequest() {
            if(PlayerState.Umbrellas.AvailableCount == 0) MessageFailure("No Umbrellas Available");
            ReservedUmbrella = PlayerState.Umbrellas.ReserveOne();
            OutgoingMessage = SubSystem.AddressManager.RouteTo(new RaiseUmbrellaRequest {
                Umbrella = ReservedUmbrella
            }, PlayerState.CurrentGame.GameManagerId);
            return true;
        }

        protected override bool ProcessReply() {
            var reply = IncomingMessage.Unwrap<Reply>();
            if(IncomingMessage == null) return MessageFailure("RaiseUmbrellaReply was null");
            else if (reply == null)
                return MessageFailure("Failed to cast RaiseUmbrellaReply");
            else if(reply.Success == false)
                return MessageFailure("Raise umbrella request was unsuccessful");
            else return true;
        }
    }
}

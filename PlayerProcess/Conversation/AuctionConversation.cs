using CommunicationLayer;
using SharedObjects;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System.Linq;
using System.Collections.Generic;
using System;
using MyUtilities;
using Messages;

namespace PlayerProcess.Conversation
{
     public class AuctionConversation : RequestReplyConversation {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AuctionConversation));

        protected bool BidOpen    { get; set; }
        protected int  Retries    { get; set; }
        protected int  UmbrellaId { get; set; }
        protected Penny[] ReservedPennies { get; set; }

        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }

        protected override void Process(object state) {
            BidOpen = true;
            while ( BidOpen && KeepGoing && Retries < RetryLimit ) {
                if(SetIncomingMessage()) {
                    if(IncomingMessage == null) MessageFailure("Auction Message was null");
                    var auction = IncomingMessage.Unwrap<AuctionAnnouncement>();
                    var bidding = IncomingMessage.Unwrap<BidAck>();
                    if ( auction != null) SendBid(auction);
                }
                else Retries++;
            }
        }

        protected void SendBid(AuctionAnnouncement auction) {
            UnreservePennies();
            UmbrellaId = IncomingMessage.Unwrap<Routing>().FromProcessId;
            var penniesLeft = PlayerState.Pennies.AvailableCount;
            var bidAmount = (auction.MinimumBid > penniesLeft)? penniesLeft : ((auction.MinimumBid > 20)? 20 : auction.MinimumBid);
            if(bidAmount > 0) {
                SendEnvelope(SubSystem.AddressManager.RouteTo(new Bid {
                    Pennies = ReservePennies(bidAmount),
                    Success = true,
                    Note = "Bidding"
                }, new int[]{ UmbrellaId }), false);
            }
        }

        protected void ProcessAck(BidAck ack) {
            if(ack.Umbrella != null && ack.Won) {
                PlayerState.Umbrellas.AddOrUpdate(ack.Umbrella);
                ReservedPennies.Tap((p)=>PlayerState.Pennies.MarkAsUsed(p.Id));
            }
            else UnreservePennies();
        }

        protected Penny[] ReservePennies(int num) {
            ReservedPennies = PlayerState.Pennies.ReserveMany(num);
            return ReservedPennies;
        }
        
        protected void UnreservePennies() {
            if(ReservedPennies.Length > 0) {
                ReservedPennies.Tap((p)=>PlayerState.Pennies.Unreserve(p.Id));
            }
        }
    }
}

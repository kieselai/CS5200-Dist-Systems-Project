using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using CommSub;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Utils;
using System;
using MyUtilities;

namespace PlayerProcess.Conversation
{
    public class AllowanceDeliveryConversation : RequestReplyWithTCP {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AllowanceDeliveryConversation));

        private PlayerState     PlayerState            { get { return SubSystem.State as PlayerState; } }
        private int             ExpectedNumberPennies  { get; set; }
        private PublicEndPoint  TCPEndPoint            { get; set; }

        protected bool TCPReceiveSuccessful { get; set; }

        protected override bool ProcessRequest() {
            var msg = IncomingMessage.Unwrap<AllowanceDeliveryRequest>();
                 if ( IncomingMessage == null ) return MessageFailure("Allowance delivery request is null");
            else if ( msg             == null ) return MessageFailure("Error while casting Allowance Delivery Request.");
            TCPEndPoint           = IncomingMessage.Destination.Clone();
            TCPEndPoint.Port      = msg.PortNumber;
            ExpectedNumberPennies = msg.NumberOfPennies;
            return true;
        }

        protected override bool OpenTCPStream() {
            var pennies = new List<Penny>();
            TCPSocket.OpenConnection( TCPEndPoint, (handler) => {
                log.Info("IN TCP Receive");
                var doContinue     = true;
                var stream         = new NetworkStream(handler);
                stream.ReadTimeout = 10000;

                while (pennies.Count < ExpectedNumberPennies && doContinue) {
                    var penny = stream.ReadStreamMessage();
                    if (penny != null) pennies.Add(penny);
                }
                log.Info("Finished reading TCP Stream, Received " + pennies.Count + " pennies.");
                if ( pennies.Count != ExpectedNumberPennies )  {
                    log.Error("Expected " + ExpectedNumberPennies + " pennies. Received " + pennies.Count);
                    MessageFailure("Something went wrong while reading TCP Message");
                }
            }, false);
            if ( pennies.Count == ExpectedNumberPennies ) {
                TCPReceiveSuccessful = true;
                pennies.Tap( (p)=> PlayerState.Pennies.AddOrUpdate(p) );
                return true;
            }
            else {
                TCPReceiveSuccessful = false;
                return true;
            }
        }
        protected override bool CreateResponse() {
            OutgoingMessage = TCPReceiveSuccessful
                ? SubSystem.AddressManager.AddressTo(new Reply{ Note = "Received Successfully", Success=true }, "PennyBank")
                : SubSystem.AddressManager.AddressTo(new Reply{ Note = "Unsuccessful Receive",  Success=false }, "PennyBank");
            if (TCPReceiveSuccessful == false ) SendEnvelope(OutgoingMessage, false);
            return TCPReceiveSuccessful;
        }
    }
}

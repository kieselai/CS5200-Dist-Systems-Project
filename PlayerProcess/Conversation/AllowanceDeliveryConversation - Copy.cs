﻿using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;

using System.Collections.Generic;
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
                handler.ReceiveTimeout = 10000;
                var doContinue     = true;
                while (pennies.Count < ExpectedNumberPennies && doContinue) {
                    var bytes = TCPSocket.ReadMessageFromSize(handler);
                    var pennySize = new Penny().DataBytes().Length;
                    if(bytes.Length == 0) doContinue = false;
                    else {
                        var penny = Penny.Decode(bytes);
                        pennies.Add(penny);
                        PlayerState.Pennies.AddOrUpdate(penny);
                    }
                }
                log.Info("Finished reading TCP Stream, Received " + pennies.Count + " pennies.");
                if ( pennies.Count != ExpectedNumberPennies ) {
                    pennies.Tap((p)=>PlayerState.Pennies.MarkAsUsed(p.Id));
                    log.Error("Expected " + ExpectedNumberPennies + " pennies. Received " + pennies.Count);
                    MessageFailure("Something went wrong while reading TCP Message");
                }
            }, false);
            if ( pennies.Count == ExpectedNumberPennies ) {
                TCPReceiveSuccessful = true;
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

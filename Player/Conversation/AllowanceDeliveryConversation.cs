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

namespace Player.Conversation
{
    public class AllowanceDeliveryConversation : RequestReplyWithTCP {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AllowanceDeliveryConversation));
        private PlayerState PlayerState {
            get { return ((PlayerState)SubSystem.State); }
        }
        private int ExpectedNumberPennies { get; set; }
        private PublicEndPoint TCPEndPoint { get; set; }
        protected override bool ProcessRequest() {
            var msg = Cast<AllowanceDeliveryRequest>(IncomingMessage);
            if(msg == null) {
                MessageFailure("Something went wrong while processing Allowance Delivery Request.");
                return false;
            }
            TCPEndPoint = IncomingMessage.Destination.Clone();
            TCPEndPoint.Port = msg.PortNumber;
            log.Debug("Received Allowance Delivery Request. ");
            ExpectedNumberPennies = msg.NumberOfPennies;
            return true;
        }

        protected override bool OpenTCPStream() {
            var pennies = new List<Penny>();

            TCPSocket.OpenConnection( TCPEndPoint, (handler) => {
                log.Info("IN TCP RECEIVE!!!!");
                var doContinue = true;
                var stream = new NetworkStream(handler);
                stream.ReadTimeout = 10000;

                while (pennies.Count < ExpectedNumberPennies && doContinue) {
                    var penny = stream.ReadStreamMessage();
                    if(penny != null) {
                        pennies.Add(penny);
                    }
                }
                log.Info("Finished reading TCP Stream");
                log.Info("Received " + pennies.Count + " pennies.");
                if(pennies.Count != ExpectedNumberPennies)  {
                    log.Error("Expected " + ExpectedNumberPennies + " pennies. Received " + pennies.Count);
                    MessageFailure("Something went wrong while reading TCP Message");
                }
            }, false);
            if(pennies.Count == ExpectedNumberPennies ) {
                var success = SendEnvelope( AddressTo(new Reply() {
                    Success = true,
                    Note = "Received Successfully"
                }, "PennyBank"), false );
                if(success) {
                    foreach(var p in pennies) {
                        PlayerState.Pennies.AddOrUpdate(p);
                    }
                    Success = true;
                    return true;
                }
            }
            return false;
        }
        protected override bool CreateResponse() {
            OutgoingMessage = AddressTo(new Reply {
                Note = "Received",
                Success = true
            }, IncomingMessage.Destination);
            return true;
        }
    }
}

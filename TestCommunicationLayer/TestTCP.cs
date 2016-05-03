using System;
using CommunicationLayer;
using SharedObjects;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerProcess;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;

namespace TestCommunicationLayer
{
    [TestClass]
    public class TestTCP {

        public TCPSocket SocketOne  { get; set; }
        public TCPSocket SocketTwo  { get; set; }

        public string MessageOne    { get; set; }
        public string MessageTwo    { get; set; }

        public byte[] RawMessageOne { get; set; }
        public byte[] RawMessageTwo { get; set; }

        public string ReceivedOne   { get; set; }
        public string ReceivedTwo   { get; set; }

        public Socket HandlerOne    { get; set; }
        public Socket HandlerTwo    { get; set; }

        public TestTCP() {
            HandlerOne = null;
            HandlerTwo = null;

            MessageOne = "Connected";
            MessageTwo = "Also Connected";

            SocketOne = TCPSocket.AcceptConnection( (handler) => {
                HandlerOne = handler;
                var messages = getMessage(handler, MessageOne);
                RawMessageOne = messages.Item1;
                ReceivedOne   = messages.Item2;
                TCPSocket.Write(handler, TCPSocket.GetBytesWithLength(MessageTwo));
            });

            SocketTwo = TCPSocket.OpenConnection( SocketOne.LocalEndPoint, (handler) => {
                HandlerTwo = handler;
                TCPSocket.Write(handler, TCPSocket.GetBytesWithLength(MessageOne));
                var messages = getMessage(handler, MessageTwo);
                RawMessageTwo = messages.Item1;
                ReceivedTwo   = messages.Item2;
            });
        }

        public Tuple<byte[], string> getMessage(Socket handler, string expectedMessage ) {
            var rawMessage = TCPSocket.ReadMessageFromSize(handler);
            string message = "";
            if( rawMessage.Length != 0) {
                message = TCPSocket.DecodeASCII(rawMessage);
            }
            Trace.WriteLine("Message Size: " + rawMessage.Length);
            Trace.WriteLine("Expected Size: "+ expectedMessage.Length);
            Trace.WriteLine("Message: " + message);
            Trace.WriteLine("Expected: " + expectedMessage);
            return Tuple.Create(rawMessage, message);
        }

        public void waitForCondition(Func<bool> testCondition, string waitMessage = "", int waitSeconds=5) {
            for( var i = 0; i < waitSeconds && testCondition() == false; i++) {
                Thread.Sleep(1000);
                if(!string.IsNullOrWhiteSpace(waitMessage)) {
                    Trace.WriteLine(waitMessage);
                }
            }
        }

        [TestMethod]
        public void TCP_Test_Accept_Connection() {
            Trace.WriteLine("Endpoint One: " + SocketOne.LocalEndPoint.ToString());
            waitForCondition(()=> HandlerOne != null, "Waiting for connection accept. ");
            Assert.IsNotNull(HandlerOne);
        }

        [TestMethod]
        public void TCP_Test_Open_Connection() {
            Trace.WriteLine("Endpoint Two: " + SocketTwo.LocalEndPoint.ToString());
            waitForCondition(()=> HandlerTwo != null, "Waiting for connection open. ");
            Assert.IsNotNull(HandlerTwo);
        }

        [TestMethod]
        public void TCP_Test_Received_Bytes_Length() {
            waitForCondition(()=> RawMessageOne != null && RawMessageTwo != null);
            Assert.IsNotNull(RawMessageOne);
            Assert.IsNotNull(RawMessageTwo);
        }

        [TestMethod]
        public void TCP_Test_Test_Message_Is_Correct() {
            waitForCondition(()=> !(string.IsNullOrEmpty(ReceivedOne) && string.IsNullOrEmpty(ReceivedTwo)));
            Assert.IsNotNull(ReceivedOne);
            Assert.IsNotNull(ReceivedTwo);
            Assert.AreEqual(ReceivedOne, MessageOne);
            Assert.AreEqual(ReceivedTwo, MessageTwo);
        }        
    }
}

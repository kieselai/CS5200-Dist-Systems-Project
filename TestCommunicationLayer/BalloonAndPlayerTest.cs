using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedObjects;
using PlayerProcess.Conversation;
using MyUtilities;
using System.Threading;
using Utils;
using CommunicationLayer;
using Messages.RequestMessages;
using Messages;
using Messages.ReplyMessages;

namespace TestCommunicationLayer
{
    [TestClass]
    public class BalloonAndPlayerTest : DummyGame {
        [TestMethod]
        public void TestBuyBalloonRequest_Start() {
            var index = init();
            TestBuyBalloonRequest_Start(index);
        }
        public void TestBuyBalloonRequest_Start(int i) {
            initBalloon(i);
            balloon[i].SignBalloons(100, 10);
            player.StartDispatcherOnly(i, this, ProcessInfo.StatusCode.PlayingGame);
            player.AddPennies(i, 3);
            Assert.IsTrue(player[i].PlayerState.Pennies.AvailableCount > 0);
            player[i].PlayerState.CurrentGame.CurrentProcesses.Add(new GameProcessData {
                ProcessId=10,
                Type = ProcessInfo.ProcessType.BalloonStore
            });
            player[i].BuyBalloon();
            var env = proxy.ReceiveMessageOfType<Routing>(i, "Proxy");
            Assert.IsNotNull(env);
            var routing = env.Unwrap<Routing>();
            Assert.IsNotNull(routing);
            var request = env.Unwrap<BuyBalloonRequest>();
            Assert.IsNotNull(request);
            Assert.AreEqual(routing.ToProcessIds[0], 10);
            ConversationId(i, request.ConvId);
        }

        [TestMethod]
        public void TestReceiveBuyBalloonRequest() {
            var index = init();
            TestReceiveBuyBalloonRequest(index);
        }
        public void TestReceiveBuyBalloonRequest(int i) {
            TestBuyBalloonRequest_Start(i);
            var convQueue = player[i].SubSystem.QueueLookup[ConversationId(i)];
            Assert.IsNotNull(convQueue);
            var request = (convQueue.Conversation as BuyBalloonConversation).OutgoingMessage;
            request.Destination = balloon.getEp(i);
            var success = proxy[i].SendWithRetries(request, ()=>balloon[i].SubSystem.QueueLookup.QueueExists(ConversationId(i)));
            Assert.IsTrue(success);
        }

        //[TestMethod]
        //public void TestValidatePennies() {
        //    var index = init();
        //    TestValidatePennies(index);
        //}
        //public void TestValidatePennies(int i) {
        //    TestReceiveBuyBalloonRequest(i);
        //    var env = pennyB[i].ReceiveMessageOfType<Message>();
        //    Assert.IsNotNull(env);
        //    Assert.IsNotNull(env.Unwrap<PennyValidation>());
        //    var reply = AddressManager.AddressTo(new Reply {
        //        Success = true
        //    }, balloon.getEp(i));
        //    reply.SetMessageIds(env.Unwrap<PennyValidation>().ConvId, false);
        //    var success = pennyB[i].SendWithRetries(reply, ()=>balloon[i].SubSystem.QueueLookup.QueueExists(env.Unwrap<PennyValidation>().ConvId));
        //    Assert.IsTrue(success);
        //}
    }
}

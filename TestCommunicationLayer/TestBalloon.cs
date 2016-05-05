using CommunicationLayer;
using SharedObjects;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerProcess;
using System;
using System.Threading;
using System.Diagnostics;
using Messages;
using Utils;
using System.Collections.Generic;
using BalloonStoreProcess;
using MyUtilities;

namespace TestCommunicationLayer
{
    [TestClass]
    public class TestBalloon : DummyGame {

        public TestBalloon() : base() {}      

        [TestMethod]
        public void TestNextIdRequest(){
            var index = init();
            TestNextIdRequest(index);
        }
        public void TestNextIdRequest(int i) {
            initBalloon(i);
            balloon[i].Start();
            var env = registry.ReceiveMessageOfType<NextIdRequest>(i, "Registry");
            Assert.IsNotNull(env);
            ConversationId(i, env.Message.ConvId);
            var msg = env.Unwrap<NextIdRequest>();
            Assert.IsNotNull(msg);
            Assert.AreEqual(msg.NumberOfIds, balloon[i].BalloonStoreState.StartingBalloons);
        }

        [TestMethod]
        public void TestNextIdReply() {
            var index = init();
            TestNextIdReply(index);
        }
        public void TestNextIdReply(int i) {
            TestNextIdRequest(i);
            var reply = AddressManager.AddressTo(new NextIdReply {
                NextId = 100, 
                NumberOfIds = 10, 
                Success = true, 
                Note = "Success!"
            }, balloon.getEp(i));
            reply.SetMessageIds(ConversationId(i), false);
            registry.SendWithRetries(i, "Registry", reply, 
                ()=> balloon[i].BalloonStoreState.Balloons.AvailableCount > 0
            );
            SyncUtils.WaitForCondition( ()=> balloon[i].BalloonStoreState.Balloons.AvailableCount == reply.Unwrap<NextIdReply>().NumberOfIds );
            Assert.AreEqual( reply.Unwrap<NextIdReply>().NumberOfIds, balloon[i].BalloonStoreState.StartingBalloons );
            Assert.AreEqual( balloon[i].BalloonStoreState.Balloons.AvailableCount, reply.Unwrap<NextIdReply>().NumberOfIds );
        }

        [TestMethod]
        public void TestBalloonSignaturesUnique() {
            var index = init();
            TestBalloonSignaturesUnique(index);
        }
        public void TestBalloonSignaturesUnique(int i) {
            initBalloon(i);
            balloon[i].SignBalloons(100, 10);
            var balloonsAvailable = balloon[i].BalloonStoreState.Balloons.ReserveMany( balloon[i].BalloonStoreState.Balloons.AvailableCount );
            for(var j = 0; j < balloonsAvailable.Length - 1; j++) {
                Assert.IsFalse( BytesAreEqual(balloonsAvailable[j].DigitalSignature, balloonsAvailable[j+1].DigitalSignature) );
            }
        }
        public bool BytesAreEqual(byte[] byteArrOne, byte[] byteArrTwo) {
            if(byteArrOne.Length != byteArrTwo.Length) return false;
            for(var i = 0; i < byteArrOne.Length; i++) {
                if( byteArrOne[i] != byteArrTwo[i] ) return false;
            }
            return true;
        }

        [TestMethod]
        public void TestBalloonSignaturesValid() {
            var index = init();
            TestBalloonSignaturesValid(index);
        }
        public void TestBalloonSignaturesValid(int i) {
            initBalloon(i);
            balloon[i].SignBalloons(100, 10);
            var balloonsAvailable = balloon[i].BalloonStoreState.Balloons.ReserveMany( balloon[i].BalloonStoreState.Balloons.AvailableCount );
            for(var j = 0; j < balloonsAvailable.Length; j++) {
                Assert.IsTrue( CryptoService.VerifySignature( CryptoService.PublicKey, balloonsAvailable[j].DataBytes(), balloonsAvailable[j].DigitalSignature) );
            }
        }
    }
}

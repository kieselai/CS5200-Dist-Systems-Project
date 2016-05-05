using CommunicationLayer;
using SharedObjects;
using System;
using log4net;
using System.Collections.Generic;
using ProcessCommon;
using MyUtilities;

using Player_Proc  = PlayerProcess.PlayerProcess;
using Balloon_Proc = BalloonStoreProcess.BalloonStoreProcess;
using Common_Proc  = ProcessCommon.CommonProcessBase;

namespace TestCommunicationLayer
{
    public class DummyGame {
        private static readonly ILog log = LogManager.GetLogger(typeof(DummyGame));
        
        private static int _count=0;
        public static int Count { get{ return _count++; } }

        public Dictionary<int, PostMan> registry;
        public Dictionary<int, PostMan> pennyB;
        public Dictionary<int, PostMan> proxy;
        public Dictionary<int, Player_Proc> player;
        public Dictionary<int, Balloon_Proc> balloon;
        public Dictionary<int, Common_Proc> common;
        private Dictionary<int, MessageNumber> _currentConvId;
        public DummyGame () {
            registry      = new Dictionary<int, PostMan>();
            pennyB        = new Dictionary<int, PostMan>();
            proxy         = new Dictionary<int, PostMan>();
            player        = new Dictionary<int, Player_Proc>();
            balloon       = new Dictionary<int, Balloon_Proc>();
            common        = new Dictionary<int, Common_Proc>();
            _currentConvId = new Dictionary<int, MessageNumber>();
        }

        public bool ValidateMessage<T>(Envelope env) {
            log.Debug( env.InvalidMessage<T>() );
            return env.IsValid<T>();
        }

        public int init() {
            var i = Count;
            Chain.Create(registry, pennyB, proxy).Tap((p)=>p.Add(i, new PostMan(12000, 20000)));
            player.addNew(i, this);
            balloon.addNew(i, this);
            common.addNew<Common_Proc, Player_Proc>(i, this);
            ConversationId(i, new MessageNumber());
            return i;
        }

        public MessageNumber ConversationId(int index) {
            if( !_currentConvId.ContainsKey(index))
                _currentConvId[index] = MessageNumber.Create();
            return _currentConvId[index];
        }

        public void ConversationId(int index, MessageNumber m) {
            _currentConvId[index] = m;
        }

        public void initBalloon(int i, ProcessInfo.StatusCode status=ProcessInfo.StatusCode.Registered) {
            balloon.StartDispatcherOnly(i, this, status);
            balloon[i].BalloonStoreState.GameManagerId      = 1;
            balloon[i].BalloonStoreState.GameId             = 1;
            balloon[i].BalloonStoreState.StartingBalloons   = 10;
            balloon[i].BalloonStoreState.StoreIndex         = 1;
            balloon[i].BalloonStoreState.IdentityInfo.Alias = "balloon Alias #1";
        }
    }
}
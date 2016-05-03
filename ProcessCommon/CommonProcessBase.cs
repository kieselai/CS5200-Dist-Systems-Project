using CommunicationLayer;
using log4net;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedObjects;
using ProcessCommon.Conversation;

namespace ProcessCommon
{
    public abstract class CommonProcessBase : CommunicationProcess {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommonProcessBase));
        public CommonProcessBase(ProcessState _state, ConversationFactory factory, int minPort, int maxPort) : base(_state, factory, minPort, maxPort) {}

        async public override void Logout(Action<bool> callback) {
            log.Debug("In Process Logout function.");
            State.CurrentMessage = "Requesting log out";
            var success = await SubSystem.Dispatcher.DispatchConversationAsync<LogoutConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.NotInitialized );
            callback(success);
        }
    }
}

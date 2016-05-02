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
    public abstract class CommonProcessBase<Conversation_Factory, Process_State> : CommunicationProcess<Conversation_Factory, Process_State> 
            where Conversation_Factory : ConversationFactory, new() where Process_State : ProcessState, new() {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommonProcessBase<Conversation_Factory, Process_State>));

        public CommonProcessBase() : base() {}

        public void initializeSubsystem(string registryEp){
            log.Debug("Initializing Subsystem.");
            initializeSubsystem();
            SubSystem.EndpointLookup.Add("Registry", new PublicEndPoint(registryEp));
        }

        async public override void Logout(Action<bool> callback) {
            log.Debug("In Process Logout function.");
            State.CurrentMessage = "Requesting log out";
            var success = await SubSystem.Dispatcher.DispatchConversationAsync<LogoutConversation>();
            if( success ) SetStatus( ProcessInfo.StatusCode.NotInitialized );
            callback(success);
        }
    }
}

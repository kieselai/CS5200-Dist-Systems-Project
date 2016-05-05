using Messages;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLayer
{
    public class AddressManager {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AddressManager));
        public EndpointLookup Lookup { get; protected set; }

        public AddressManager() {
            Lookup = new EndpointLookup();
        }

        public static Envelope AddressTo(Message m, PublicEndPoint ep) {
            return new Envelope(m, ep);
        }

        public Envelope AddressTo(Message m, string lookup) {
            return new Envelope(m, Lookup[lookup]);
        }

        public Envelope RouteTo(Message innerMessage, params int[] PIDs) {
            log.Debug("Creating Routing Envelope");
            if ( innerMessage != null ){
                return AddressTo(new Routing {
                    InnerMessage = innerMessage, 
                    ToProcessIds = PIDs
                }, "Proxy");
            }
            else {  log.Error("Inner Message is null"); }
            return null;
        }

        public Envelope RouteTo(Message innerMessage, Envelope ReceivedMessage) {
            var originalReceived = ReceivedMessage.Unwrap<Routing>();
            if(originalReceived == null) {
                log.Error("Error retrieving routing Ids");
                return null;
            }
            else return RouteTo(innerMessage, originalReceived.FromProcessId);
        }

        public static Envelope AddressTo(AddressManager am, Message m, string lookup) {
            return am.AddressTo(m, lookup);
        }

        public static Envelope RouteTo(AddressManager am, Message innerMessage, params int[] PIDs) {
            return am.RouteTo(innerMessage, PIDs);
        }
        public static Envelope RouteTo(AddressManager am, Message innerMessage, Envelope ReceivedMessage) {
            return am.RouteTo(innerMessage, ReceivedMessage);
        }
    }
}

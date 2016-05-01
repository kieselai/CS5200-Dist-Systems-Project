using System.Net;
using SharedObjects;
using Messages;


namespace CommunicationLayer
{
    public class Envelope {
        public PublicEndPoint Destination { get; set; }
        public Message        Message     { get; set; }

        public Envelope ( Message message, PublicEndPoint endpoint ) {
            Message     = message;
            Destination = endpoint;
        }
    }
}

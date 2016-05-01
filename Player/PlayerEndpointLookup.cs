using CommunicationLayer;
using System.Net;
using SharedObjects;

namespace Player
{
    public class PlayerEndpointLookup : EndpointLookup {
        public PlayerEndpointLookup( PublicEndPoint RegistryEp ) {
            Add( "Registry", RegistryEp);
        }
        public PlayerEndpointLookup( string RegistryEp ) {
            Add( "Registry", new PublicEndPoint( RegistryEp ));
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using log4net;
using Messages;
using SharedObjects;

namespace CommunicationLayer
{
    public class PostMan {
        private static readonly ILog log = LogManager.GetLogger(typeof(PostMan));

        public LocalEndPoint LocalEndPoint { get; set; }

        public PostMan(int minPort, int maxPort){
            LocalEndPoint = new LocalEndPoint(LocalEndPoint.NetworkConnectionType.UDP, minPort, maxPort);
        }

        public bool Send ( Envelope env, int timeout) {
            log.Debug("Sending UDP ("+env.Message.ToString()+", MessageID: " + env.Message.MsgId + ", ConversationID: "+env.Message.ConvId + ")"
                +"\nEndpoint: " +env.Destination.ToString() 
                +"\nJSON: "+System.Text.Encoding.Default.GetString(env.Message.Encode()));
            if( Deliver(env, timeout ) > 0) {
                log.Debug( "Message Delivered" );
                return true;
            }
            else {
                log.Debug( "Timeout during message delivery" );
                return false;
            }
        }

        public Envelope Receive ( int timeout ) {
            var result = Listen(timeout);
            if( result.Buffer.Length == 0  ) { 
                return null;
            }
            var msg = Message.Decode( result.Buffer );
            log.Debug("Received UDP (" + msg.ToString() + ", MessageID: " + msg.MsgId + ", ConversationID: "+msg.ConvId + ")\nJSON: "+System.Text.Encoding.Default.GetString(result.Buffer));
            var endPoint = new PublicEndPoint { IPEndPoint = result.RemoteEndPoint };
            return new Envelope( msg, endPoint );
        }


        public UdpReceiveResult Listen(int timeout) {
            byte[] buffer = new byte[0];
            var remoteEndPoint = new IPEndPoint( IPAddress.Any, 0 );
            UdpAction( (udpSocket) => {
                try{ buffer = udpSocket.Receive(ref remoteEndPoint); }
                catch ( Exception e ){ }// log.Debug( "Timeout during message listen" ); }
            }, timeout);
            return new UdpReceiveResult(buffer, remoteEndPoint);;
        }


        public int Deliver(Envelope envelope, int timeout) {
            var serialized = envelope.Message.Encode();
            int numBytes = 0;
            UdpAction( (udpSocket) => {
                try{ numBytes = udpSocket.Send(serialized, serialized.Length, envelope.Destination.IPEndPoint); }
                catch ( Exception e ){  log.Error( e.ToString() ); }
            });
            return numBytes;
        }

        protected void UdpAction(Action<UdpClient> action, int timeout = 0) {
            using ( var udpSocket = new UdpClient()) {
                udpSocket.Client.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpSocket.Client.Bind( LocalEndPoint );
                if(timeout > 0) udpSocket.Client.ReceiveTimeout = timeout;
                action(udpSocket);
            }
        }
    }
}

using SharedObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace CommunicationLayer {
    public class TCPSocket : BackgroundThread {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TCPSocket));
        public LocalEndPoint LocalEndPoint { get; set; }

        public TCPSocket() {
            LocalEndPoint = new LocalEndPoint();
        }

        public static TCPSocket OpenConnection(PublicEndPoint destination, Action<Socket> afterConnect, bool startNewThread = true) {
            var socket = new TCPSocket();
            log.Debug("Destination Endpoint is: "+destination.ToString());
            if(startNewThread) {
                socket.Start(new Action(()=> {
                    socket.EstablishConnection(destination, afterConnect);
                }));
            }
            else {
                socket.Process(new Action(()=> {
                    socket.EstablishConnection(destination, afterConnect);
                }));
            }
            return socket;
        }

        public static TCPSocket AcceptConnection(Action<Socket> afterConnect, bool startNewThread = true) {
            var socket = new TCPSocket();
            

            if(startNewThread) {
                socket.Start(new Action(()=> {
                    socket.StartAcceptConnection(afterConnect);
                }));
            }
            else {
                socket.Process(new Action(()=> {
                    socket.StartAcceptConnection(afterConnect);
                }));
            }
            return socket;
        }

        protected override void Process(object state) {
            var acceptOrConnect = state as Action;
            if(acceptOrConnect == null) {
                log.Error("Error while parsing state parameter in TCPSocket.");
                return;
            }
            acceptOrConnect();
            Stop();
        }

        protected void EstablishConnection(PublicEndPoint ep, Action<Socket> afterConnect) {
            log.Debug("Opening Remote TCP Connection");
            try {
                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                    client.Connect(ep.IPEndPoint);
                    log.Debug("TCP Socket connected to "+ client.RemoteEndPoint.ToString());
                    afterConnect(client);
                }
            }
            catch(Exception e) {
                log.Debug(e);
            }
        }
        protected void StartAcceptConnection(Action<Socket> afterAccept) {
            log.Debug("Opening TCP Socket");
            try {
                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                    log.Debug("Binding to local Endpoint");
                    client.Bind( LocalEndPoint );
                    client.Listen(10);
                    Socket handler = client.Accept();
                    log.Debug("Connection accepted");
                    afterAccept(handler);
                }
            }
            catch(Exception e) {
                log.Debug(e);
            }
        }

        public static void Write(Socket handler, string msg) {
            Write(handler, Encoding.ASCII.GetBytes(msg));
        }
        public static void Write(Socket handler, byte[] bytes) {
            handler.Send(bytes);
        }

        public static byte[] GetBytesWithLength(string msg){
            var msgBytes = GetBytes(msg);
            return GetBytes(msgBytes.Length).Concat(msgBytes).ToArray();
        }
        public static byte[] GetBytes(int num) {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(num));
        }
        public static byte[] GetBytes(string msg) {
            return Encoding.ASCII.GetBytes(msg);
        }

        public static int DecodeInt(IEnumerable<byte> msg) {
            if(msg.Count() != 4) return 0;
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(msg.ToArray(), 0));
        }

        public static string DecodeASCII(byte[] msg) {
            return Encoding.ASCII.GetString(msg, 0, msg.Length);
        }

        public static byte[] Read(Socket handler, int numBytes) {
            log.Debug("Reading bytes in TCP Reader");
            byte[] buffer = new byte[numBytes];
            int bytesRec = handler.Receive(buffer, numBytes, SocketFlags.None);
            log.Debug( "TCP Message (string): " + DecodeASCII(buffer));
            log.Debug( "TCP Message (int): "    + DecodeInt(buffer.Take(4).ToArray()));
            if(bytesRec != numBytes) {
                log.Error("Number of bytes received in TCP message was not the expected amount.");
                buffer = new byte[0];
            }
            return buffer;
        }

        public static List<byte[]> ReadStream(Socket handler) {
            var stream = new NetworkStream(handler);
            stream.ReadTimeout = 10000;
            
            var byteList = new List<byte[]>();
            try {
                while(stream.CanRead) {
                    var buffer = new byte[4];
                    stream.Read(buffer, 0, 4);
                    int size = DecodeInt(buffer);
                    if(size > 0 && stream.CanRead) {
                        log.Info("TCP: Received Size");
                        buffer = new byte[size];
                        stream.Read(buffer, 0, size);
                    }
                    if(buffer.Length != 0) {
                        byteList.Add(buffer);
                        log.Info("TCP: Received Message");
                    }
                }
            }
            catch(Exception e) {
                log.Error(e);
            }
            
            return byteList;
        }

        public static byte[] ReadMessageFromSize(Socket handler) {
            log.Debug("Reading a message, assuming the size of the message is contained in the first 4 bits");
            byte[] rawSize = Read(handler, 4);
            if(rawSize.Length != 0) {
                var msgSize = DecodeInt(rawSize);
                log.Debug("Message Size is: " + msgSize);
                return Read(handler, msgSize);
            }
            return new byte[0];
        }

        
    }
}

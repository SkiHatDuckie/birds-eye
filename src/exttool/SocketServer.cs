using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BirdsEye {
    public class SocketServer {
        private TcpListener _server;
        private TcpClient? _client;
        private NetworkStream? _clientStream;
        private int _port;
        private IPAddress _host;

        ///<summary>
        /// Create a socket server object that listens for 
        /// connections at the given port and address.
        ///</summary>
        public SocketServer(string host, int port) {
            _port = port;
            _host = IPAddress.Parse(host);
            _server = new TcpListener(_host, _port);
            _server.Start();
        }

        ///<summary>
        /// Accept a connection request from a client object.
        ///</summary>
        public void AcceptConnections() {
            _client = _server.AcceptTcpClient();
            _clientStream = _client.GetStream();
        }

        ///<summary>
        /// Returns true if a client object is connected to the server.
        ///</summary>
        public bool IsConnected() {
            return (_client == null) ? false : true;
        }

        ///<summary>
        /// Decode and return a message from the client object.
        /// Precondition: socket client is connected to the server.
        ///</summary>
        public string GetResponse() {
            if (_clientStream == null) {
                return "";
            }

            Byte[] bytes = new Byte[256];
            int i = _clientStream.Read(bytes, 0, bytes.Length);

            return Encoding.ASCII.GetString(bytes, 0, i);
        }

        ///<summary>
        /// Converts a string message into bytes and sends it to the client object.
        /// Precondition: socket client is connected to the server.
        ///</summary>
        public void SendMessage(string msg) {
            if (_clientStream == null) {
                return;
            }

            byte[] data = Encoding.ASCII.GetBytes(msg);
            _clientStream.Write(data, 0, data.Length);
        }

        ///<summary>
        /// End communication with the connected client object 
        /// (if one is connected!).
        ///</summary>
        public void CloseConnection() {
            if (_client != null) {
                _client.Close();
            }
        }
    }
}

using System.Net.Sockets;
using System.Net;

namespace BirdsEye {
    public class SocketServer {
        private TcpListener _server;
        private TcpClient? _client;
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
        }

        ///<summary>
        /// Returns true if a client object is connected to the server.
        ///</summary>
        public bool IsConnected() {
            return (_client == null) ? false : true;
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
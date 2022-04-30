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
        /// connections at a given port and address.
        ///</summary>
        public SocketServer(string host, int port) {
            _port = port;
            _host = IPAddress.Parse(host);
            _server = new TcpListener(_host, _port);
        }

        ///<summary>
        /// Accept a connection request from a python client.
        ///</summary>
        public void AcceptConnections() {
            _server.Start();
            _client = _server.AcceptTcpClient();
            _clientStream = _client.GetStream();
            _server.Stop();
        }

        ///<summary>
        /// Returns true if a python client is connected to the server.
        ///</summary>
        public bool IsConnected() {
            return (_client == null) ? false : true;
        }

        ///<summary>
        /// Decode and return messages from the python client.<br/>
        /// Multiple messages are seperated by an '\n'.<br/>
        /// Returns an array with "ERR" if an exception while reading from
        /// the socket stream occurred.
        ///</summary>
        public string[] GetRequests() {
            if (_clientStream == null) {
                return new string[] {"ERR"};
            }

            Byte[] bytes = new Byte[1024];
            try {
                int i = _clientStream.Read(bytes, 0, bytes.Length);

                return Encoding.ASCII.GetString(bytes, 0, i).Split('\n');
            } catch {
                return new string[] {"ERR"};
            }
        }

        ///<summary>
        /// Converts a string message into bytes and sends it to the python client.<br/>
        /// Precondition: python client is connected to the server.
        ///</summary>
        public void SendMessage(string msg) {
            if (_clientStream == null) {
                return;
            }

            byte[] data = Encoding.ASCII.GetBytes(msg);
            _clientStream.Write(data, 0, data.Length);
        }

        ///<summary>
        /// End communication with the connected python client.
        ///</summary>
        public void CloseConnection() {
            if (_client != null) {
                _client.Close();
                _client = null;
                _clientStream = null;
            }
        }

        ///<summary>
        /// Stop listening for connections if still doing so,
        /// and then close all socket objects.
        ///</summary>
        public void CloseAll() {
            _server.Stop();
            CloseConnection();
        }
    }
}
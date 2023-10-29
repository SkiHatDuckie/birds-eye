using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BirdsEye {
    public class SocketServer {
        private readonly TcpListener _server;
        private TcpClient? _client;
        private NetworkStream? _clientStream;
        private readonly int _port;
        private readonly IPAddress _host;
        private readonly Logging _log;

        /// <summary>
        /// Create a socket server object that listens for 
        /// connections at a given port and address.
        /// </summary>
        public SocketServer(Logging log, string host, int port) {
            _log = log;
            _port = port;
            _host = IPAddress.Parse(host);
            _server = new TcpListener(_host, _port);
        }

        /// <summary>
        /// Accept a connection request from a python client.
        /// </summary>
        public void AcceptConnections(object config) {
            _log.Write(1, "Accepting connection request from python client.");
            _server.Start();
            _client = _server.AcceptTcpClient();
            _clientStream = _client.GetStream();
            _client.ReceiveTimeout = ((Config) config).socketTimeout;
            _client.SendTimeout = 10000;
            _server.Stop();
        }

        /// <summary>
        /// Returns true if a python client is connected to the server.
        /// </summary>
        public bool IsConnected() {
            return _client != null;
        }

        /// <summary>
        /// Blocks until a message is received from the python client.
        /// Returns the received data as bytes.
        /// </summary>
        private (int, byte[]) ReceiveMessages() {
            byte[] bytes = new byte[2048];
            int numBytes = _clientStream!.Read(bytes, 0, bytes.Length);
            return (numBytes, bytes);
        }

        /// <summary>
        /// Decode and return messages from the python client.<br/>
        /// Multiple messages are seperated by an '\n'.<br/>
        /// Precondition: Python client is connected to server.
        /// </summary>
        public Request[] ParseRequests() {
            var data = ReceiveMessages();
            _log.Write(0,
                $@"Received the following message: {
                    Encoding.ASCII.GetString(data.Item2, 0, data.Item1)
                }"
            );
            string[] requests = Encoding.ASCII.GetString(data.Item2, 0, data.Item1).Split('\n');

            return requests.Select(s => {
                string[] request = s.Split(new char[] { ';' }, 2);
                return new Request(request[0], request[1]);
            }).ToArray();
        }

        /// <summary>
        /// Converts a string message into bytes and sends it to the python client.<br/>
        /// Precondition: Python client is connected to server.
        /// </summary>
        public void SendMessage(string msg) {
            _log.Write(0, $"Sending message: {msg}");

            byte[] data = Encoding.ASCII.GetBytes(msg);
            _clientStream!.Write(data, 0, data.Length);
        }

        /// <summary>
        /// End communication with the connected python client.
        /// </summary>
        public void CloseConnection() {
            _log.Write(1, "Closing socket connection with python client.");
            if (_client != null) {
                _client.Close();
                _client = null;
                _clientStream = null;
            }
        }

        /// <summary>
        /// Stop listening for connections if still doing so,
        /// and then close all socket objects.
        /// </summary>
        public void CloseAll() {
            _log.Write(1, "Closing all socket objects.");
            _server.Stop();
            CloseConnection();
        }
    }
}
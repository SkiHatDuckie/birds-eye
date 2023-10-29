import socket

from birdseyelib.response import Response


class Client:
    """A socket client used to communicate with the external tool 
    located at a set port and address.

    Use the external tool to get and set the port and
    address the server should listen in to.

    :param ip: The socket address.
    :type ip: str

    :param port: The socket port number.
    :type port: int"""
    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.connection_status = -1
        self.payload = ""
        self.latest_responses = []

    def connect(self):
        """(Blocking) Attempt to connect to the external tool.

        Call `Client.is_connected()` to see if the attempt was successful."""
        self.connection_status = self.client.connect_ex((self.ip, self.port))

    def close(self):
        """Close socket connection with external tool and send a final message
        notifying the external tool before closing."""
        try:
            self.client.sendall("CLOSE;\n".encode())
        except:
            pass
        self.client.close()
        self.connection_status = -1

    def is_connected(self):
        """Returns true if client is connected to the external tool."""
        return self.connection_status == 0

    def _send_requests(self):
        """(Internal) Send requests from any birdseye object to the external
        tool.

        :precondition: client is connected to a socket."""
        try:
            self.client.sendall(self.payload.encode())
            self.payload = ""
        except:
            self.close()

    def _queue_request(self, request):
        """(Internal) Append a string request to the queue to be
        sent.
        
        :param request: The request to be added.
        :type request: str"""
        self.payload += request

    def _receive_messages(self) -> str:
        """(Internal) (Blocking) Wait to receive data collected by the external
        tool.

        :precondition: client is connected to a socket."""
        try:
            return self.client.recv(2048).decode()
        except:
            self.close()

    def _parse_responses(self):
        """(Internal) (Blocking) Parse received responses into a list of
        objects.
        
        :precondition: client is connected to a socket."""
        data = self._receive_messages()
        responses = data.split("\n")
        self.latest_responses.clear()
        for response in responses:
            if response != "":
                self.latest_responses.append(Response(*response.split(";", maxsplit=1)))

    def _get_latest_response_data(self, tag) -> str:
        """(Internal) Returns the latest response received with the given
        `tag`. An empty string is returned if a response with `tag` cannot be
        found.
        
        :param tag: The tag corresponding with the received data of interest.
        :type tag: str"""
        for response in self.latest_responses:
            if tag == response.tag:
                return response.data
        
        return ""

    def advance_frame(self):
        """(Blocking) Sends all queued requests to the external tool and parses
        received responses. This will subsequently advance the emulator to the
        next frame.
        
        :precondition: client is connected to a socket."""
        self._send_requests()
        self._parse_responses()

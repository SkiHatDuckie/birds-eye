import socket


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

        self.input_request = ""

    def connect(self):
        """Attempt to connect to the external tool.

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

    def send_requests(self, objects):
        """Send requests from any birdseye object to the external tool.

        :param objects: A list of birdseye classes that interact with the external tool or \
        BizHawk emulator.
        :type objects: list

        :precondition: client is connected to a socket."""
        try:
            self.client.sendall(("".join([obj.request for obj in objects])).encode())
        except:
            self.close()

    def get_responses(self) -> str:
        """Receive data collected by the external tool, and update each object in `objects` accordingly.

        :precondition: client is connected to a socket."""
        try:
            return self.client.recv(2048).decode()
        except:
            self.close()

        return ""

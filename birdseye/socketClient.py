import socket;


class Client:
    """Instantiate a socket client object that will be used to
    communicate with the BirdsEye server located at a set
    port and address.

    Use the BirdsEye external tool to get and set the port and
    address the server should listen in to.

    `ip` is the socket address

    `port` is the socket port number"""

    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.connection_status = -1

        self.address_list = []
        self.memory_request = ""
        self.input_request = ""

        self.received_memory = None

    def connect(self):
        """Attempts to send a connection request to the BirdsEye socket server."""

        self.connection_status = self.client.connect_ex((self.ip, self.port))

    def is_connected(self):
        """Returns true if client is connected to a socket server object."""

        if self.connection_status == 0:
            return True
        else:
            return False
    
    def send_requests(self):
        """Send requests to the external tool and receive data collected by
        the external tool."""
        self.client.sendall((self.memory_request + self.input_request).encode())

        try:
            responses = self.client.recv(2048).decode()
            for response in responses.split("\n"):
                if len(response) > 6 and response[0:6] == "MEMORY":
                    self.received_memory = response[7:]
        except:
            ConnectionError("Connection with external tool was lost.")
    
    def add_address(self, addr):
        """Adds an address for the external tool to return.
        
        `addr` is a hexidecimal value representing the address to read from 
        in the BizHawk emulator's memory."""

        if not addr in self.address_list:
            self.address_list.append(str(addr))

    def add_address_range(self, start, end):
        """Precondition: `start` <= `end`.

        Adds a range of addresses from `start` to `end`, both inclusive.

        `start` is a hexidecimal value representing the first address in the range.

        `end` is a hexidecimal value representing the last address in the range."""

        for addr in range(int(start), int(end) + 1):
            self.add_address(addr)

    def get_memory(self) -> str:
        """Gets the latest memory data from the emulator. This will return
        latest value received for each address paired with the address it was read from. 
        Returns 'int(addr):-' if there has been no data received yet from an address, 
        and 'int(addr):int(data)' representing the received data otherwise.

        Precondition: client is connected to a socket."""

        self.memory_request = "MEMORY;" + ";".join(self.address_list) + "\n"
        self.address_list = []

        return self.received_memory

    def set_controller_input(self, a=False, b=False, up=False, down=False, right=False, left=False):
        """Sets the controller inputs to be executed in the emulator.
        All inputs are set to False be default.
        The inputs are executed until a new controller input is sent.
        Precondition: client is connected to a socket.

        `a` is the state of the A button

        `b` is the state of the B button

        `up` is the state of the Up button on the control pad

        `down` is the state of the Down button on the control pad

        `right` is the state of the Right button on the control pad

        `left` is the state of the Left button on the control pad"""

        bool_to_string = {False : "false", True : "true"}
        controller_input = bool_to_string[a] + ";" + bool_to_string[b] + ";" + \
                           bool_to_string[up] + ";" + bool_to_string[down] + ";" + \
                           bool_to_string[right] + ";" + bool_to_string[left]
        self.input_request = "INPUT;" + controller_input + "\n"

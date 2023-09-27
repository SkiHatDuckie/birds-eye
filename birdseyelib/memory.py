class Memory:
    """Class containing various functions for setting controller input in BizHawk."""
    def __init__(self) -> None:
        self.address_list = []
        self.request = ""
        self.received_memory = ""

    def add_address(self, addr):
        """Adds an address for the external tool to return.

        :param addr: A hexidecimal value representing the address to read from \
        in the BizHawk emulator's memory.
        :type addr: int"""
        if not str(addr) in self.address_list:
            self.address_list.append(str(addr))
            self.received_memory += str(addr) + ":-1;"

    def add_address_range(self, start, end):
        """Adds a range of addresses from `start` to `end`, both inclusive.

        :param start: A hexidecimal value representing the first address in the range.
        :type start: int

        :param end: A hexidecimal value representing the last address in the range.
        :type end: int

        :precondition: `start` <= `end`."""
        for addr in range(int(start), int(end) + 1):
            self.add_address(addr)

    def request_memory(self):
        """Requests for the latest memory data from the external tool."""
        self.request = "MEMORY;" + ";".join(self.address_list) + "\n"
        self.address_list = []

    def get_memory(self) -> list:
        """Gets the latest memory data received from the external tool. 

        This will return a list of the latest values received from each address paired 
        with the address it was read from, represented as:

        `"addr:-1"`, if there has been no data received yet from an address

        `"addr:data"`, with both `addr` and `data` in decimal form."""
        return self.received_memory.split(";")

    def process_responses(self, responses):
        """Updates fields accordingly with the given responses.

        :param responses: A message received from the external tool.
        :type responses: str"""
        for response in responses.split("\n"):
            if len(response) > 6 and response[0:6] == "MEMORY":
                    self.received_memory = response[7:]

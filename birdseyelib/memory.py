from birdseyelib import Request


class AddressList(Request):
    """Maintains a list of addresses for the external tool to return."""
    def __init__(self, client):
        super().__init__("MEM_ADDRESS", client)
        self.entries = []

    def queue(self):
        """Sends all entries to be returned by the external tool."""
        if self.entries != []:
            self.client._queue_request(self.tag + ";" + ";".join(self.entries) + "\n")

    def add(self, addr):
        """Adds an address for the external tool to return.

        :param addr: A hexidecimal value representing the address to read from \
        in the BizHawk emulator's memory.
        :type addr: int"""
        if not str(addr) in self.entries:
            self.entries.append(str(addr))

    def add_range(self, start, end):
        """Adds a range of addresses from `start` to `end`, both inclusive.

        :param start: A hexidecimal value representing the first address in the range.
        :type start: int

        :param end: A hexidecimal value representing the last address in the range.
        :type end: int

        :precondition: `start` <= `end`."""
        for addr in range(int(start), int(end) + 1):
            self.add(addr)


class Memory(Request):
    """Requests for the latest memory data from the external tool.

    Addresses to read must be sent via `memory.AddressList`"""
    def __init__(self, client):
        super().__init__("MEM_READ", client)
        self.received_memory = {}
    
    def receive(self) -> dict:
        """Gets the latest memory data received from the external tool. 

        This will return a shallow copy of the dictionary containing the latest
        data received from each requested address. Where the address
        (in hexadecimal) is the key, and the data is the value (in decimal).

        It's important to note that the returned `dict` does NOT
        add a key for a given address until it has been received from the
        external tool."""
        data = self.client._get_latest_response_data(self.tag)

        if data:
            address_value_pairs = data.strip(";").split(";")
            for addr_val_pair in address_value_pairs:
                temp = addr_val_pair.split(":")
                addr, val = temp[0], temp[1]
                self.received_memory[hex(int(addr))] = int(val)
        return self.received_memory.copy()

class Emulation:
    """Class containing various functions for controlling emulation in BizHawk."""
    def __init__(self, client) -> None:
        self.client = client

    def request_framecount(self):
        """Requests for the current framecount from the external tool."""
        self.client._queue_request("FRAME;\n")

    def get_framecount(self) -> int:
        """Returns the current framecount from the emulator, or `-1`, if no
        data has been received yet."""
        data = self.client._get_latest_response_data("FRAME")
        if data:
            return int(data)
        else:
            return -1

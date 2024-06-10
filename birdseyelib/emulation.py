class Emulation:
    """Class containing various functions for controlling emulation in BizHawk."""
    def __init__(self, client) -> None:
        self.client = client

    def request_framecount(self):
        """Requests for the current framecount from the external tool."""
        self.client._queue_request("EMU_FRAME;\n")
    
    def request_board_name(self):
        """Requests for the board name of the loaded ROM."""
        self.client._queue_request("EMU_BOARD;\n")
    
    def request_display_type(self):
        """Requests for the display type that the emulator is currently running on."""
        self.client._queue_request("EMU_DISPLAY;\n")

    def get_framecount(self) -> int:
        """Returns the current framecount from the emulator, or `-1`, if no
        data has been received yet."""
        data = self.client._get_latest_response_data("EMU_FRAME")
        return int(data) if data else -1
    
    def get_board_name(self) -> str:
        """Returns the board name of the loaded ROM, or `""`, if not available."""
        data = self.client._get_latest_response_data("EMU_BOARD")
        return data if data else ""

    def get_display_type(self) -> str:
        """Returns the display type (`"PAL"` or `"NTSC"`) that the emulator is currently running
        on.

        Returns `""`, if not avaiable."""
        data = self.client._get_latest_response_data("EMU_DISPLAY")
        return data if data else ""

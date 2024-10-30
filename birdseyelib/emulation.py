from birdseyelib import Request


class Framecount(Request):
    """Requests for the current framecount from the external tool."""
    def __init__(self, client):
        super().__init__("EMU_FRAME", client)
    
    def receive(self) -> int:
        """Returns the current framecount from the emulator, or `-1`, if no
        data has been received yet."""
        data = self.client._get_latest_response_data(self.tag)
        return int(data) if data else -1


class BoardName(Request):
    """Requests for the board name of the loaded ROM."""
    def __init__(self, client):
        super().__init__("EMU_BOARD", client)

    def receive(self) -> str:
        """Returns the board name of the loaded ROM, or `""`, if not available."""
        data = self.client._get_latest_response_data(self.tag)
        return data if data else ""


class DisplayType(Request):
    """Requests for the display type that the emulator is currently running on."""
    def __init__(self, client):
        super().__init__("EMU_DISPLAY", client)

    def receive(self) -> str:
        """Returns the display type (`"PAL"` or `"NTSC"`) that the emulator is currently running
        on.

        Returns `""`, if not avaiable."""
        data = self.client._get_latest_response_data(self.tag)
        return data if data else ""

class ExternalTool:
    """Class containing various functions for controlling the external tool."""
    def __init__(self, client) -> None:
        self.client = client

    def request_commandeer(self):
        """Requests for the current status on commandeer."""
        self.client._queue_request("COM_GET;\n")

    def get_commandeer(self) -> bool:
        """Returns the current status on commandeer: `True` if enabled, `False` otherwise."""
        data = self.client._get_latest_response_data("COM_GET")
        return eval(data)

    def set_commandeer(self, enabled):
        """Sets the communication mode of the external tool to either manual or commandeer.

        :param enabled: Determines whether a request to enable commandeer or disable it should be sent. \
        `True` = enable commandeer, `False` = disable.
        :type enabled: bool"""
        self.client._queue_request("COM_SET;" + str(enabled) + "\n")

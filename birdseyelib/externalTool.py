class ExternalTool:
    """Class containing various functions for controlling the external tool."""
    def __init__(self, client) -> None:
        self.client = client

    def set_commandeer(self, enabled):
        """Sets the communication mode of the external tool to either manual or commandeer.

        :param enabled: Determines whether a request to enable commandeer or disable it should be sent. \
        `True` = enable commandeer, `False` = disable.
        :type enabled: bool"""
        self.client._queue_request("COMMANDEER;" + str(enabled) + "\n")

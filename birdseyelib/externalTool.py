from request import Request


class GetCommandeer(Request):
    """Returns the current status on commandeer: `True` if enabled, `False` otherwise."""
    def __init__(self, client):
        super().__init__("COM_GET", client)
    
    def receive(self):
        return eval(self.client._get_latest_response_data(self.tag))


class SetCommandeer(Request):
    """Sets the communication mode of the external tool to either manual or commandeer.

    :param enabled: Determines whether a request to enable commandeer or disable it should be
    sent. `True` = enable commandeer, `False` = disable.
    :type enabled: bool"""
    def __init__(self, client, enabled):
        super().__init__("COM_SET", client)
        self.enabled = enabled

    def queue(self):
        self.client._queue_request(self.tag + ";" + str(self.enabled) + "\n")

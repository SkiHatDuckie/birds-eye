from birdseyelib import Request


class Commandeer(Request):
    """Gets and sets the commandeer mode in the external tool.
    
    :param enabled: Determines whether a request to enable commandeer or disable it should be
    sent. `True` = enable commandeer, `False` = disable.
    :type enabled: bool"""
    def __init__(self, client):
        super().__init__("COM", client)

    def queue(self, enabled=None):
        enabled_str = "" if enabled is None else str(enabled)
        self.client._queue_request(self.tag + ";" + enabled_str + "\n")

    def receive(self):
        return eval(self.client._get_latest_response_data(self.tag))

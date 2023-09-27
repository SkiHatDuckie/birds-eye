class Emulation:
    """Class containing various functions for controlling emulation in BizHawk."""
    def __init__(self) -> None:
        self.framecount = -1
        self.request = ""

    def request_framecount(self):
        """Requests for the current framecount from the external tool."""
        self.request = "FRAME;\n"

    def process_responses(self, responses):
        """Updates class fields accordingly with the given responses.

        :param responses: A message received from the external tool.
        :type responses: str"""
        for response in responses.split("\n"):
            if len(response) > 5 and response[0:5] == "FRAME":
                    self.framecount = int(response[6:])

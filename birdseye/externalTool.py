class ExternalTool:
    """Class containing various functions for controlling the external tool."""
    def __init__(self) -> None:
        self.request = ""
    
    def set_commandeer(self, enabled):
        """Sets the communication mode of the external tool to either manual or commandeer.
        
        :param enabled: Determines whether a request to enable commandeer or disable it should be sent. \
        `True` = enable commandeer, `False` = disable.
        :type enabled: bool"""
        self.request = "COMMANDEER;" + str(enabled) + "\n"
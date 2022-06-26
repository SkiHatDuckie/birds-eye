class ExternalTool:
    """Class containing various functions for controlling the external tool."""
    def __init__(self) -> None:
        self.request = ""
    
    def set_commandeer(self, enabled):
        """Sets the communication mode of the external tool to either manual or commandeer.
        
        `enabled` is a boolean that determines whether a request to enable commandeer or not should be 
        sent. `True` = enable commandeer, `False` = disable."""
        self.request = "COMMANDEER;" + str(enabled) + "\n"
class ControllerInput:
    """Class containing various functions for reading memory from BizHawk."""
    def __init__(self) -> None:
        self.request = ""

    def set_controller_input(self, a=False, b=False, up=False, down=False, right=False, left=False):
        """Sets the controller inputs to be executed in the emulator.
        All inputs are set to `False` be default.
        The inputs are executed until a new controller input is sent.

        `a` is the state of the A button

        `b` is the state of the B button

        `up` is the state of the Up button on the control pad

        `down` is the state of the Down button on the control pad

        `right` is the state of the Right button on the control pad

        `left` is the state of the Left button on the control pad"""
        bool_to_string = {False : "false", True : "true"}
        controller_input = bool_to_string[a] + ";" + bool_to_string[b] + ";" + \
                           bool_to_string[up] + ";" + bool_to_string[down] + ";" + \
                           bool_to_string[right] + ";" + bool_to_string[left]
        self.request = "INPUT;" + controller_input + "\n"

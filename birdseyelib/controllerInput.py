class ControllerInput:
    """Class containing various functions for reading memory from BizHawk."""
    def __init__(self, client) -> None:
        self.client = client

    def set_controller_input(self, a=False, b=False, up=False, down=False, right=False, left=False):
        """Sets the controller inputs to be executed in the emulator.
        All inputs are set to `False` be default.
        The inputs are executed until a new controller input is sent.

        :param a: The state of the A button.
        :type a: bool

        :param b: The state of the B button.
        :type b: bool

        :param up: The state of the Up button on the control pad.
        :type up: bool

        :param down: The state of the Down button on the control pad.
        :type down: bool

        :param right: The state of the Right button on the control pad.
        :type right: bool

        :param left: The state of the Left button on the control pad.
        :type left: bool"""
        bool_to_string = {False : "false", True : "true"}
        controller_input = bool_to_string[a] + ";" + bool_to_string[b] + ";" + \
                           bool_to_string[up] + ";" + bool_to_string[down] + ";" + \
                           bool_to_string[right] + ";" + bool_to_string[left]
        self.client._queue_request("INP_SET;" + controller_input + "\n")

class ControllerInput:
    """Class containing various functions for reading memory from BizHawk."""
    def __init__(self, client):
        self.client = client

    def set_joypad(self, joypad):
        """Sets the joypad layout to be used when setting inputs.
        
        Layout is set to the NES joypad by default.

        :param joypad: The joypad layout to send to the external tool. \
        Make sure the joypad matches the emulated system being used!
        :type joypad: Joypad"""
        self.client._queue_request("INP_JOYPAD;" + joypad._name + "\n")

    def set_controller_input(self, joypad):
        """Sets the controller inputs to be executed in the emulator.
        All inputs are set to `False` be default.
        The inputs are executed until a new controller input is sent.

        :param joypad: The joypad layout to send to the external tool. \
        Make sure the joypad matches the emulated system being used!
        :type joypad: Joypad"""
        bool_to_string = {False : "false", True : "true"}
        controller_input = ";".join(
            [bool_to_string[joypad.controls[button]] for button in joypad.controls.keys()]
        )
        controller_analog_input = ";".join(
            [str(joypad.analog_controls[analog_control]) for analog_control in joypad.analog_controls.keys()]
        )
        self.client._queue_request("INP_SET;" + controller_input + "\n")
        self.client._queue_request("INP_SET_ANALOG;" + controller_analog_input + "\n")

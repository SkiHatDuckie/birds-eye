from birdseyelib import Request


class JoypadConfig(Request):
    """Sets the joypad layout to be used when setting inputs.

    Layout is set to the NES joypad by default.
    
    :param joypad: The joypad layout to send to the external tool. \
    Make sure the joypad matches the emulated system being used!
    :type joypad: Joypad"""
    def __init__(self, client, joypad):
        super().__init__("INP_JOYPAD", client)
        self.joypad = joypad

    def queue(self):
        self.client._queue_request(self.tag + ";" + self.joypad._name + "\n")

class ControllerInputs(Request):
    """Sets the controller inputs to be executed in the emulator.
    All inputs are set to `False` be default.
    The inputs are executed until a new controller input is sent.
    
    :param joypad: The joypad layout to send to the external tool. \
    Make sure the joypad matches the emulated system being used!
    :type joypad: Joypad"""
    def __init__(self, client, joypad):
        super().__init__("INP_SET", client)
        self.joypad = joypad

    def queue(self):
        controller_input = [
            str(self.joypad.controls[button]).lower()
            for button in self.joypad.controls.keys()
        ]
        if hasattr(self.joypad, "analog_controls"):
            controller_input += [
                str(self.joypad.analog_controls[analog_control])
                for analog_control in self.joypad.analog_controls.keys()
            ]

        controller_input = ";".join(controller_input)
        self.client._queue_request(self.tag + ";" + controller_input + "\n")

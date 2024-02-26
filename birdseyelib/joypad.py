class Joypad:
    """Treated as a custom type for joypad layouts."""
    pass


class NESJoypad(Joypad):
    """Controller mapping for the NES.
    
    The \"Power\" and \"Reset\" buttons are not implemented for the time being."""
    def __init__(self):
        super().__init__()
        self._name = "NES"
        self.controls = {
            "A": False, "B": False, "Up": False, "Down": False, "Right": False, "Left": False,
            "Select": False, "Start": False,
        }

class GBAndGBCJoypad(Joypad):
    """Controller mapping for the GB and GBC.
    
    The \"Power\" button is not implemented for the time being."""
    def __init__(self):
        super().__init__()
        self._name = "GB(C)"
        self.controls = {
            "A": False, "B": False, "Up": False, "Down": False, "Right": False, "Left": False,
            "Select": False, "Start": False,
        }
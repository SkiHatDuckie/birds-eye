using System.Collections.Generic;

namespace BirdsEye {
    public class Joypad {
        public string? Name;
        public IDictionary<string, bool>? Controls;
        public IDictionary<string, int?>? ControlsAnalog;
        public int? DefaultController;
    }

    public class NESJoypad : Joypad {
        public NESJoypad() {
            Name = "NES";
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"Up", false}, {"Down", false}, {"Right", false},
                {"Left", false}, {"Select", false}, {"Start", false},
            };
            DefaultController = 1;
        }
    }

    public class GBAndGBCJoypad : Joypad {
        public GBAndGBCJoypad() {
            Name = "GB(C)";
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"Up", false}, {"Down", false}, {"Right", false},
                {"Left", false}, {"Select", false}, {"Start", false},
            };
            DefaultController = 1;
        }
    }

    public class SNESJoypad : Joypad {
        public SNESJoypad() {
            Name = "SNES";
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"L", false}, {"R", false}, {"X", false}, {"Y", false},
                {"Up", false}, {"Down", false}, {"Right", false}, {"Left", false},
                {"Select", false}, {"Start", false},
            };
            DefaultController = 1;
        }
    }

    public class NDSJoypad : Joypad {
        public NDSJoypad() {
            Name = "NDS";
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"L", false}, {"R", false}, {"X", false}, {"Y", false},
                {"Up", false}, {"Down", false}, {"Right", false}, {"Left", false},
                {"Select", false}, {"Start", false}, {"LidClose", false}, {"LidOpen", false},
                {"Touch", false},
            };
            ControlsAnalog = new Dictionary<string, int?>() {
                {"Mic Volume", 0}, {"GBA Light Sensor", 0}, {"Touch X", 142}, {"Touch Y", 0},
            };
            DefaultController = null;
        }
    }
}

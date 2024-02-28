using System.Collections.Generic;

namespace BirdsEye {
    public class Joypad {
        public IDictionary<string, bool>? Controls;
    }

    public class NESJoypad : Joypad {
        public NESJoypad() {
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"Up", false}, {"Down", false}, {"Right", false},
                {"Left", false}, {"Select", false}, {"Start", false},
            };
        }
    }

    public class GBAndGBCJoypad : Joypad {
        public GBAndGBCJoypad() {
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"Up", false}, {"Down", false}, {"Right", false},
                {"Left", false}, {"Select", false}, {"Start", false},
            };
        }
    }

    public class SNESJoypad : Joypad {
        public SNESJoypad() {
            Controls = new Dictionary<string, bool>() {
                {"A", false}, {"B", false}, {"L", false}, {"R", false}, {"X", false}, {"Y", false},
                {"Up", false}, {"Down", false}, {"Right", false}, {"Left", false},
                {"Select", false}, {"Start", false},
            };
        }
    }
}
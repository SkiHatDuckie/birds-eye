using System.Collections.Generic;

namespace BirdsEye {
    public class Joypad {
        public IDictionary<string, bool>? Controls;
    }

    public class NESJoypad : Joypad {
        public NESJoypad() {
            Controls = new Dictionary<string, bool>() {
                {"A", false},
                {"B", false},
                {"Up", false},
                {"Down", false},
                {"Right", false},
                {"Left", false},
                {"Select", false},
                {"Start", false},
            };
        }
    }
}
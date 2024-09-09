using System;
using System.Collections.Generic;
using System.Linq;

using BizHawk.Client.Common;

namespace BirdsEye {
    public class ControllerInput {
        private Joypad _joypad = new NESJoypad();
        private readonly Logging _log;

        public ControllerInput(Logging log) {
            _log = log;
        }

        ///<summary>
        /// Sets the joypad layout to be used when setting inputs. <br/>
        /// Set to the NES joypad by default. <br/>
        /// Returns nothing as a response.
        ///</summary>
        public Response SetJoypad(string newJoypad) {
            _log.Write(1, $"Changing joypad layout to {newJoypad}.");
            _joypad = newJoypad switch {
                "NES" => new NESJoypad(),
                "GB(C)" => new GBAndGBCJoypad(),
                "SNES" => new SNESJoypad(),
                "NDS" => new NDSJoypad(),
                _ => _joypad
            };
            return new Response("");
        }

        ///<summary>
        /// Execute the current input state in the emulator.
        ///</summary>
        public void ExecuteInput(ApiContainer APIs) {
            APIs.Joypad.Set((IReadOnlyDictionary<string, bool>) _joypad.Controls!, _joypad.DefaultController);
            if (_joypad.ControlsAnalog != null) {
                APIs.Joypad.SetAnalog((IReadOnlyDictionary<string, int?>) _joypad.ControlsAnalog, _joypad.DefaultController);
            }
        }

        ///<summary>
        /// Set the controller input from a string containing the state of different buttons
        /// to be executed when `ExecuteInput` is called. <br/>
        /// Be warned! This assumes that the joypad layout matches the currently
        /// emulated system. <br/>
        /// Returns nothing as a response.
        ///</summary>
        public Response SetInputFromString(string str) {
            string[] newState = str.Trim(';').Split(';');
            string key;
            for (int i = 0; i < _joypad.Controls!.Count; i++) {
                key = _joypad.Controls.ElementAt(i).Key;
                _joypad.Controls[key] = Convert.ToBoolean(newState[i]);
            }
            return new Response("");
        }

        public Response SetAnalogInputFromString(string str) {
            string[] newState = str.Trim(';').Split(';');
            string key;
            for (int i = 0; i < _joypad.ControlsAnalog!.Count; i++) {
                key = _joypad.ControlsAnalog.ElementAt(i).Key;
                _joypad.ControlsAnalog[key] = Convert.ToInt32(newState[i]);
            }
            return new Response("");
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace BirdsEye {
    [ExternalTool("BirdsEye")]
    public class CustomMainForm : ToolFormBase, IExternalToolForm {
        /// <remarks>
		/// <see cref="ApiContainer"/> can be used as a shorthand for accessing 
        /// the various APIs, more like the Lua syntax.
		/// </remarks>
		public ApiContainer? _apiContainer { get; set; }

		private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        private SocketServer _server = new SocketServer("127.0.0.1", 8080);
        private Thread _commThread;
        private System.Timers.Timer _timeoutTimer = new System.Timers.Timer(10000);

        private Memory _memory = new Memory();

        private ControllerInput _input = new ControllerInput();

        private bool _commandeer = false;

        private Label _lblRomName;
        private Label _lblMemory;
        private ListBox _lstAddress;
        private ListBox _lstMemory;
        private Label _lblCommMode;
        private Button _btnChangeCommMode;
        private Label _lblConnectionStatus;
        private Label _lblError;

        protected override string WindowTitleStatic => "BirdsEye";

        ///<summary>
        /// Main form constructor.
        /// Code is executed only once (when EmuHawk.exe is launched).
        ///</summary>
        public CustomMainForm() {
            _commThread = new Thread(new ThreadStart(_server.AcceptConnections));
            _commThread.Start();

            _timeoutTimer.Elapsed += OnTimeout;
            
            ClientSize = new Size(480, 320);
            SuspendLayout();

            // Rom Label
            _lblRomName = new Label {
                AutoSize = true,
                Location = new Point(0, 0),
            };
            // Memory Label
            _lblMemory = new Label {
                AutoSize = true,
                Location = new Point(0, 25),
                Text = "Memory"
            };
            // Memory Address ListBox
            _lstAddress = new ListBox {
                Location = new Point(100, 50),
                Size = new Size(70, 150)
            };
            // Memory Data ListBox
            _lstMemory = new ListBox {
                Location = new Point(0, 50),
                Size = new Size(100, 150)
            };
            // Communication Mode Label
            _lblCommMode = new Label {
                AutoSize = true,
                Location = new Point(240, 0),
                Text = "Communication Mode: Manual"
            };
            // Communication Mode Button
            _btnChangeCommMode = new Button {
                Location = new Point(240, 20),
                Size = new Size(100, 25),
                Text = "Change Mode"
            };
            // Connection Status Label
            _lblConnectionStatus = new Label {
                AutoSize = true,
                Location = new Point(240, 50),
                Text = "No script found",
                ForeColor = Color.Red
            };
            // Error Label
            _lblError = new Label {
                AutoSize = true,
                Location = new Point(0, 300),
                ForeColor = Color.Red
            };

            Controls.Add(_lblRomName);
            Controls.Add(_lblMemory);
            Controls.Add(_lstAddress);
            Controls.Add(_lstMemory);
            Controls.Add(_lblCommMode);
            Controls.Add(_btnChangeCommMode);
            Controls.Add(_lblConnectionStatus);
            Controls.Add(_lblError);
            ResumeLayout();

            // _btnNewAddress.Click += btnNewAddressOnClick;
            _btnChangeCommMode.Click += btnChangeCommModeOnClick;
        }

        ///<summary>
        /// Executed once after the constructor, and again every time a rom is
        /// loaded or reloaded.
        ///</summary>
        public override void Restart() {
            DisplayLoadedRom();
        }

        ///<summary>
        /// Executed before every frame.
        /// If in commandeer mode, this function will enter into a while loop,
        /// halting the emulator until input is received from the connected
        /// python script, or until connection is switched to manual mode.
        ///</summary>
        protected override void UpdateBefore() {
            UpdateMemoryListBox();
            UpdateConnectionStatus();
            if (_server.IsConnected()) {
                ProcessResponses();
            }
            if (_commandeer) {
                _input.ExecuteInput(APIs);
            }
        }

        ///<summary>
        /// Process responses received by `_server`.
        ///</summary>
        private void ProcessResponses() {
            string[] msgs = _server.GetResponses();
            if (msgs.Length == 0) {
                if (!_timeoutTimer.Enabled) {
                    _timeoutTimer.Start();
                }
            } else {
                if (_timeoutTimer.Enabled) {
                    _timeoutTimer.Stop();
                }
            }

            foreach (string msg in msgs) {
                if (msg.Length > 6 && msg.Substring(0, 6).Equals("MEMORY")) {
                    if (msg != "MEMORY;") {
                        _memory.AddAddressesFromString(msg);
                    }
                    _server.SendMessage(_memory.FormatMemory());
                } else if (msg.Length > 5 && msg.Substring(0, 5).Equals("INPUT")) {
                    _input.SetInputFromString(msg);
                    _server.SendMessage("Received");
                }
            }
        }

        ///<summary>
        /// Change the text of `_lblRomName` to display the current rom.
        ///</summary>
        private void DisplayLoadedRom() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                _lblRomName.Text = $"Currently loaded: {APIs.GameInfo.GetRomName()}";
            } else {
                _lblRomName.Text = "Currently loaded: Nothing";
            }
        }

        ///<summary>
        /// Update every item in `_lstMemory` with the new values in memory
        ///</summary>
        private void UpdateMemoryListBox() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                int[] memoryData = _memory.ReadMemory(APIs);
                for (int i = 0; i < _lstMemory.Items.Count; i++) {
                    _lstMemory.Items[i] = memoryData[i].ToString();
                }
            }
        }

        ///<summary>
        /// Determine if all characters in `str` are valid hexadecimal digits.
        /// Returns false if an invalid digit is found, otherwise this returns true.
        ///</summary>
        private bool IsHexadecimal(string str) {
            string hexadecimalChars = "0987654321ABCDEFabcdef";
            foreach (char ch in str) {
                if (!hexadecimalChars.Contains(ch)) {
                    return false;
                }
            }
            return true;
        }

        ///<summary>
        /// Update the current stutus of the socket connection and update
        /// `_lblConnectionStatus` accordingly.
        ///</summary>
        private void UpdateConnectionStatus() {
            if (_server.IsConnected()) {
                _lblConnectionStatus.Text = "Script found";
                _lblConnectionStatus.ForeColor = Color.Blue;
                if (!_commThread.IsAlive) {
                    _commThread.Join();
                }
            } else {
                _lblConnectionStatus.Text = "No script found";
                _lblConnectionStatus.ForeColor = Color.Red;
            }
        }

        ///<summary>
        /// Change the communication mode from manual -> commandeer or commandeer -> manual.
        /// Displays an error if the user attempts to switch to commandeer mode before a
        /// script is connected.
        ///</summary>
        private void btnChangeCommModeOnClick(object sender, EventArgs e) {
            if (!_server.IsConnected() && !_commandeer) {
                _lblError.Text = "ERROR: No script is connected";
                return;
            } else if (!_commandeer) {
                _commandeer = true;
                _lblCommMode.Text = "Communication Mode: Commandeer";
            } else {
                _commandeer = false;
                _lblCommMode.Text = "Communication Mode: Manual";
            }
            _lblError.Text = "";
        }

        ///<summary>
        /// Executed every time `_timeoutTimer` interval has elapsed.
        /// Displays an error message in the external tool.
        ///</summary>
        private void OnTimeout(Object source, ElapsedEventArgs e) {
            _timeoutTimer.Stop();

            _server.CloseConnection();
            UpdateConnectionStatus();
            _lblError.Text = "ERROR: No response from script after 10 seconds, connection closed";
            _commandeer = false;
            _lblCommMode.Text = "Communication Mode: Manual";

            _commThread = new Thread(new ThreadStart(_server.AcceptConnections));
            _commThread.Start();
        }
    }
}
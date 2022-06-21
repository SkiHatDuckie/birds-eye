using System;
using System.Drawing;
using System.Linq;
using System.Threading;
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

        private Config _config = new Config();
        private Logging _log;
        private SocketServer _server;
        private Memory _memory;
        private ControllerInput _input;
        private Emulation _emulation;

        private bool _commandeer = false;
        private Thread _commThread;

        private MenuStrip _mainFormMenu;
        private ToolStripMenuItem _optionSubMenu;
        private Label _lblRomName;
        private Label _lblCommMode;
        private Button _btnChangeCommMode;
        private Label _lblConnectionStatus;
        private Label _lblError;

        protected override string WindowTitleStatic => "BirdsEye";

        /// <summary>
        /// Main form constructor.<br/>
        /// Code is executed only once (when EmuHawk.exe is launched).
        /// </summary>
        public CustomMainForm() {
            _log = new Logging(_config.logLevel);
            _server = new SocketServer(_log, _config.host, _config.port);
            _memory = new Memory(_log);
            _input = new ControllerInput(_log);
            _emulation = new Emulation(_log);

            _log.Write(1, "Initializing main form.");
            this.FormClosing += OnFormClosing;

            _commThread = new Thread(new ParameterizedThreadStart(_server.AcceptConnections));
            _commThread.Start(_config);

            ClientSize = new Size(480, 320);
            SuspendLayout();

            _lblRomName = new Label {
                AutoSize = true,
                Location = new Point(0, 30),
            };
            _lblCommMode = new Label {
                AutoSize = true,
                Location = new Point(240, 30),
                Text = "Communication Mode: Manual"
            };
            _btnChangeCommMode = new Button {
                Location = new Point(240, 50),
                Size = new Size(100, 25),
                Text = "Change Mode"
            };
            _lblConnectionStatus = new Label {
                AutoSize = true,
                Location = new Point(240, 80),
                Text = "No script found",
                ForeColor = Color.Red
            };
            _lblError = new Label {
                AutoSize = true,
                Location = new Point(0, 300),
                ForeColor = Color.Red
            };
            _optionSubMenu = new ToolStripMenuItem {
                Size = new System.Drawing.Size(50, 25),
                Text = "&Options"
            };
            _mainFormMenu = new MenuStrip {
                Location = new Point(0, 0),
                Size = new System.Drawing.Size(this.Width, 25),
                TabIndex = 0,
                Text = "menuStrip1",
            };

            _mainFormMenu.Items.Add(_optionSubMenu);
            Controls.Add(_mainFormMenu);
            Controls.Add(_lblRomName);
            Controls.Add(_lblCommMode);
            Controls.Add(_btnChangeCommMode);
            Controls.Add(_lblConnectionStatus);
            Controls.Add(_lblError);
            ResumeLayout();

            _optionSubMenu.Click += optionSubMenuOnClick;
            _btnChangeCommMode.Click += btnChangeCommModeOnClick;
        }

        /// <summary>
        /// Executed once after the constructor, and again every time a rom is
        /// loaded or reloaded.
        /// </summary>
        public override void Restart() {
            DisplayLoadedRom();
        }

        /// <summary>
        /// Executed before every frame.
        /// </summary>
        protected override void UpdateBefore() {
            UpdateConnectionStatus();
            if (_server.IsConnected() && APIs.GameInfo.GetRomName() != "Null") {
                ProcessRequests();
                if (_commandeer) {
                    _input.ExecuteInput(APIs);
                }
            }
        }

        /// <summary>
        /// Process requests received by `_server`.
        /// Disconnects python client if an error occurs when receiving data, or
        /// if the python client sends a request for the connection to be closed.
        /// </summary>
        private void ProcessRequests() {
            string[] msgs = _server.GetRequests();
            if (msgs[0] == "ERR") {
                _log.Write(2, "Disconnecting from python client due to bad request.");
                HandleDisconnect();
            }

            string response = "";
            foreach (string msg in msgs) {
                if (msg.Length > 6 && msg.Substring(0, 6).Equals("MEMORY")) {
                    if (msg != "MEMORY;") {
                        _memory.AddAddressesFromString(msg);
                    }
                    response += "MEMORY;" + _memory.FormatMemory() + "\n";
                } else if (msg.Length > 5 && msg.Substring(0, 5).Equals("INPUT")) {
                    _input.SetInputFromString(msg);
                    response += "INPUT;\n";
                } else if (msg.Length > 5 && msg.Substring(0, 5).Equals("CLOSE")) {
                    HandleDisconnect();
                } else if (msg.Length > 5 && msg.Substring(0, 5).Equals("FRAME")) {
                   response += "FRAME;" + _emulation.GetFramecount(APIs) + "\n";
                }
            }

            _server.SendMessage(response);
        }

        /// <summary>
        /// Change the text of `_lblRomName` to display the current rom.
        /// </summary>
        private void DisplayLoadedRom() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                _lblRomName.Text = $"Currently loaded: {APIs.GameInfo.GetRomName()}";
            } else {
                _lblRomName.Text = "Currently loaded: Nothing";
            }
        }

        /// <summary>
        /// Determine if all characters in `str` are valid hexadecimal digits.<br/>
        /// Returns false if an invalid digit is found, otherwise this returns true.
        /// </summary>
        private bool IsHexadecimal(string str) {
            string hexadecimalChars = "0987654321ABCDEFabcdef";
            foreach (char ch in str) {
                if (!hexadecimalChars.Contains(ch)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Update the current status of the socket connection and update
        /// `_lblConnectionStatus` accordingly.
        /// </summary>
        private void UpdateConnectionStatus() {
            if (_server.IsConnected()) {
                _lblConnectionStatus.Text = "Script found";
                _lblConnectionStatus.ForeColor = Color.Blue;
                _lblError.Text = "";
                if (!_commThread.IsAlive) {
                    _commThread.Join();
                }
            } else {
                _lblConnectionStatus.Text = "No script found";
                _lblConnectionStatus.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Executed when the socket connection abrupty ends, or when requested
        /// by the python client.<br/>
        /// Displays an error message in the external tool and cleans up socket resources.
        /// </summary>
        private void HandleDisconnect() {
            _server.CloseConnection();
            UpdateConnectionStatus();
            _lblError.Text = "Connection with script has been stopped.";
            _commandeer = false;
            _lblCommMode.Text = "Communication Mode: Manual";

            _memory.ClearAddresses();

            _commThread = new Thread(new ParameterizedThreadStart(_server.AcceptConnections));
            _commThread.Start(_config);
        }

        /// <summary>
        /// Change the communication mode from manual -> commandeer, or commandeer -> manual.<br/>
        /// Displays an error if the user attempts to switch to commandeer mode before a
        /// python client is connected, or if a ROM is not loaded yet.
        /// </summary>
        private void btnChangeCommModeOnClick(object sender, EventArgs e) {
            if (!_server.IsConnected()) {
                _log.Write(2, "Cannot switch to commandeer when no script is connected.");
                _lblError.Text = "ERROR: No script is connected";
            } else if (APIs.GameInfo.GetRomName() == "Null") {
                _log.Write(2, "Cannot switch to commandeer when no ROM has been loaded.");
                _lblError.Text = "ERROR: No ROM has been loaded";
            } else {
                if (!_commandeer) {
                    _log.Write(1, "Communication mode set to commandeer.");
                    _commandeer = true;
                    _lblCommMode.Text = "Communication Mode: Commandeer";
                } else {
                    _log.Write(1, "Communication mode set to manual.");
                    _commandeer = false;
                    _lblCommMode.Text = "Communication Mode: Manual";
                }
                _lblError.Text = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void optionSubMenuOnClick(object sender, EventArgs e) {
            OptionsForm options = new OptionsForm(_config);
            options.ShowDialog();
        }

        /// <summary>
        /// Close any resources and threads before closing the application.
        /// Attempts to abort out of any thread, regardless of state.
        /// </summary>
        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            _log.Write(1, "Gracefully closing external tool.");
            try {
                _commThread.Abort();
            } catch {}

            _server.CloseAll();
        }
    }
}
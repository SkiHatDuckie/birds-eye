using System;
using System.Threading;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace BirdsEye {
    [ExternalTool("BirdsEye")]
    public sealed partial class CustomMainForm : ToolFormBase, IExternalToolForm {
        /// <remarks>
        /// <see cref="ApiContainer"/> can be used as a shorthand for accessing 
        /// the various APIs, more like the Lua syntax.
        /// </remarks>
        public ApiContainer? APIContainer { get; set; }
        private ApiContainer APIs => APIContainer ?? throw new NullReferenceException();

        private readonly Config _config = new();
        private readonly Logging _log;
        private readonly SocketServer _server;
        private readonly Memory _memory;
        private readonly ControllerInput _input;
        private readonly Emulation _emulation;

        private bool _commandeer = false;

        private Thread _commThread;

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

            _commThread = new Thread(new ParameterizedThreadStart(_server.AcceptConnections));
            _commThread.Start(_config);

            _log.Write(0, "Initializing main form.");
            InitializeControls();
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
            bool isConnected = _server.IsConnected();
            UpdateConnectionStatus(isConnected);
            if (isConnected) {
                if (!_commThread.IsAlive) {
                    _commThread.Join();
                }
                string? romName = APIs.Emulation.GetGameInfo()?.Name;
                if (romName != null || romName != "Null") {
                    ProcessRequests();
                    if (_commandeer) {
                        _input.ExecuteInput(APIs);
                    }
                }
            }
        }

        /// <summary>
        /// Process requests received by `_server`.
        /// Disconnects python client if an error occurs when receiving data, or
        /// if the python client sends a request for the connection to be closed.
        /// </summary>
        private void ProcessRequests() {
            try {
                string response = "";
                foreach (Request req in _server.ParseRequests()) {
                    if (req.Tag == "MEMORY") {
                        if (!string.IsNullOrEmpty(req.Data)) {
                            _memory.AddAddressesFromString(req.Data);
                        }
                        _memory.ReadMemory(APIs);
                        response += "MEMORY;" + _memory.FormatMemory() + "\n";
                    } else if (req.Tag == "INPUT") {
                        _input.SetInputFromString(req.Data);
                        response += "INPUT;\n";
                    } else if (req.Tag == "CLOSE") {
                        HandleDisconnect();
                        return;  // Short circuit to avoid sending an empty message.
                    } else if (req.Tag == "FRAME") {
                        response += "FRAME;" + _emulation.GetFramecount(APIs) + "\n";
                    } else if (req.Tag == "COMMANDEER") {
                        if (req.Data == "True") {
                            EnableCommandeer();
                        } else {
                            DisableCommandeer();
                        }
                    }
                }
                _server.SendMessage(response);
            } catch {
                _log.Write(3, "An unexpected exception occured when trying to read requests.");
                _log.Write(2, "Disconnecting from python client due to exception.");
                HandleDisconnect();
            }
        }

        /// <summary>
        /// Change the text of `_lblRomName` to display the current rom.
        /// </summary>
        private void DisplayLoadedRom() {
            string? romName = APIs.Emulation.GetGameInfo()?.Name;
            if (romName != null || romName != "Null") {
                _lblRomName.Text = $"Currently loaded: {romName}";
            } else {
                _lblRomName.Text = "Currently loaded: Nothing";
            }
        }

        /// <summary>
        /// Update the current status of the socket connection and update
        /// `_lblConnectionStatus` accordingly.
        /// </summary>
        private void UpdateConnectionStatus(bool isConnected) {
            if (isConnected) {
                _lblConnectionStatus.Text = "Script found";
                _lblConnectionStatus.ForeColor = ColorTheme.VistaBlue;
            } else {
                _lblConnectionStatus.Text = "No script found";
                _lblConnectionStatus.ForeColor = ColorTheme.BurntSienna;
            }
        }

        /// <summary>
        /// Executed when the socket connection abrupty ends, or when requested
        /// by the python client.<br/>
        /// Displays an error message in the external tool and cleans up socket resources.
        /// </summary>
        private void HandleDisconnect() {
            _server.CloseConnection();
            UpdateConnectionStatus(false);
            DisableCommandeer();

            _memory.ClearAddresses();

            _lstError.Items.Add("WARNING: Connection with script has been stopped.");

            _commThread = new Thread(new ParameterizedThreadStart(_server.AcceptConnections));
            _commThread.Start(_config);
        }

        /// <summary>
        /// Checks if external tool should switch to commandeer.
        /// </summary>
        private bool CanCommandeer() {
            string? romName = APIs.Emulation.GetGameInfo()?.Name;
            if (!_server.IsConnected()) {
                _log.Write(2, "Cannot switch to commandeer when no script is connected.");
                _lstError.Items.Add("ERROR: No script is connected.");
                return false;
            } else if (romName == null || romName == "Null") {
                _log.Write(2, "Cannot switch to commandeer when no ROM has been loaded.");
                _lstError.Items.Add("ERROR: No ROM has been loaded.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Switches the communication mode to commandeer.
        /// </summary>
        private void EnableCommandeer() {
            _log.Write(1, "Communication mode set to commandeer.");
            _commandeer = true;
            _lblCommMode.Text = "Communication Mode: Commandeer";
        }

        /// <summary>
        /// Switches the communication mode to manual.
        /// </summary>
        private void DisableCommandeer() {
            _log.Write(1, "Communication mode set to manual.");
            _commandeer = false;
            _lblCommMode.Text = "Communication Mode: Manual";
        }

        /// <summary>
        /// Change the communication mode from manual -> commandeer, or commandeer -> manual.<br/>
        /// Displays an error if the user attempts to switch to commandeer mode before a
        /// python client is connected, or if a ROM is not loaded yet.
        /// </summary>
        private void ChangeCommModeButtonOnClick(object sender, EventArgs e) {
            if (CanCommandeer()) {
                if (!_commandeer) {
                    EnableCommandeer();
                } else {
                    DisableCommandeer();
                }
            }
        }

        /// <summary>
        /// Disconnect the client.
        /// Displays a message if no client is already connected.
        /// </summary>
        private void DisconnectClientButtonOnClick(object sender, EventArgs e) {
            if (!_server.IsConnected()) {
                _lstError.Items.Add("ERROR: No script is connected.");
            } else {
                _server.CloseConnection();
            }
        }

        /// <summary>
        /// Opens an instance of the options form.
        /// </summary>
        private void SubMenuOptionOnClick(object sender, EventArgs e) {
            OptionsForm options = new(_config);
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
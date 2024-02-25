using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, Func<string, Response>> _requestDictionary;

        private bool _commandeer = false;

        private Thread _commThread;

        protected override string WindowTitleStatic => "BirdsEye";

        /// <summary>
        /// Main form constructor.<br/>
        /// Code is executed only once (when EmuHawk.exe is launched).
        /// </summary>
        public CustomMainForm() {
            _log = new Logging(_config.logLevel);
            _server = new SocketServer(_log, _config.host, _config.port, _config.socketTimeout,
                _config.socketBufSize);
            _memory = new Memory(_log);
            _input = new ControllerInput(_log);
            _emulation = new Emulation(_log);

            _requestDictionary = new Dictionary<string, Func<string, Response>>() {
                { "MEM_ADDRESS", (req) => _memory.AddAddressesFromString(req) },
                { "MEM_READ", (req) => _memory.MemoryOnRequest(APIs) },
                { "COM_GET", (req) => new Response(_commandeer.ToString()) },
                { "COM_SET", (req) => ChangeCommMode(req == "True") },
                { "INP_SET", (req) => _input.SetInputFromString(req) },
                { "EMU_FRAME", (req) => _emulation.GetFramecount(APIs) },
                { "EMU_BOARD", (req) => _emulation.GetBoardName(APIs) },
            };

            _commThread = new Thread(new ThreadStart(_server.AcceptConnections));
            _commThread.Start();

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
                if (romName != null && romName != "Null") {
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
                    if (req.Tag == "CLOSE") {
                        HandleDisconnect();
                        return;  // Short circuit to avoid sending an empty message.
                    } else {
                        try {
                            response += req.Tag + ";" + _requestDictionary[req.Tag](req.Data)
                                + "\n";
                        } catch (KeyNotFoundException) {
                            _log.Write(2, $"Unknown request tag: {req.Tag}.");
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
            ChangeCommMode(false);
            _memory.ClearAddresses();
            _lstError.Items.Add("WARNING: Connection with script has been stopped.");
            _commThread = new Thread(new ThreadStart(_server.AcceptConnections));
            _commThread.Start();
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

        private Response ChangeCommMode(bool enable_commandeer) {
            string modeName = enable_commandeer ? "Commandeer" : "Manual";
            _log.Write(1, $"Communication mode set to {modeName.ToLower()}.");
            _commandeer = enable_commandeer;
            _lblCommMode.Text = $"Communication Mode: {modeName}";
            return new Response("");
        }

        /// <summary>
        /// Change the communication mode from manual -> commandeer, or commandeer -> manual.<br/>
        /// Displays an error if the user attempts to switch to commandeer mode before a
        /// python client is connected, or if a ROM is not loaded yet.
        /// </summary>
        private void ChangeCommModeButtonOnClick(object sender, EventArgs e) {
            if (CanCommandeer()) {
                ChangeCommMode(!_commandeer);
            }
        }

        /// <summary>
        /// Displays a message in `_lstError` if no client is already connected.
        /// </summary>
        private void DisconnectClientButtonOnClick(object sender, EventArgs e) {
            if (!_server.IsConnected()) {
                _lstError.Items.Add("ERROR: No script is connected.");
            } else {
                HandleDisconnect();
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
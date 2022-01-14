using System;
using System.Collections.Generic;
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

        private SocketServer _server = new SocketServer("127.0.0.1", 8080);
        private Thread _commThread;

        private Label _lblRomName;

        private Label _lblMemory;
        private ListBox _lstAddress;
        private ListBox _lstMemory;
        private Button _btnNewAddress;
        private TextBox _txtAddress;
        private List<long> _memoryAddresses = new List<long>();

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
            // New Address button
            _btnNewAddress = new Button {
                Location = new Point(0, 45),
                Size = new Size(100, 25),
                Text = "Add Address:"
            };
            // New Address TextBox
            _txtAddress = new TextBox {
                Location = new Point(100, 45),
			    Size = new Size(70, 20)
            };
            // Memory Address ListBox
            _lstAddress = new ListBox {
                Location = new Point(100, 70),
                Size = new Size(70, 150)
            };
            // Memory Data ListBox
            _lstMemory = new ListBox {
                Location = new Point(0, 70),
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
            Controls.Add(_btnNewAddress);
            Controls.Add(_txtAddress);
            Controls.Add(_lstAddress);
            Controls.Add(_lstMemory);
            Controls.Add(_lblCommMode);
            Controls.Add(_btnChangeCommMode);
            Controls.Add(_lblConnectionStatus);
            Controls.Add(_lblError);

            ResumeLayout();

            _btnNewAddress.Click += btnNewAddressOnClick;
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
        /// Reads each byte (in `bytes`) in main memory.
        /// Returns an integer array of the data found.
        ///</summary>
        private uint[] ReadMemory(long[] bytes) {
            uint[] data = new uint[bytes.Length];
            for (int i = 0; i < bytes.Length; i++) {
                data[i] = APIs.Memory.ReadByte(bytes[i]);
            }
            return data;
        }

        ///<summary>
        /// Update every item in `_lstMemory` with the new values in memory
        ///</summary>
        private void UpdateMemoryListBox() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                uint[] memoryData = ReadMemory(_memoryAddresses.ToArray());
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
        /// Check on the current stutus of the socket connection and update
        /// `_lblConnectionStatus` accordingly.
        ///</summary>
        public void checkConnectionStatus() {
            if (_server.IsConnected()) {
                _lblConnectionStatus.Text = "Script found";
                _lblConnectionStatus.ForeColor = Color.Blue;
            } else {
                _lblConnectionStatus.Text = "No script found";
                _lblConnectionStatus.ForeColor = Color.Red;
            }

            if (!_commThread.IsAlive) {
                _commThread.Join();
            }
        }

        ///<summary>
        /// Executed once after the constructor, and again every time a rom is
        /// loaded or reloaded.
        ///</summary>
        public override void Restart() {
            DisplayLoadedRom();
        }

        ///<summary>
        /// Executed after every frame.
        ///</summary>
        protected override void UpdateAfter() {
            UpdateMemoryListBox();
            checkConnectionStatus();
        }

        ///<summary>
        /// Update the lstAddress, lstMemory, and memoryAddresses with the entered 
        /// address in txtAddress.
        ///</summary>
        private void btnNewAddressOnClick(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(_txtAddress.Text) && !_lstAddress.Items.Contains(_txtAddress.Text)) {
                if (IsHexadecimal(_txtAddress.Text)) {
                    _memoryAddresses.Add(Convert.ToInt64(_txtAddress.Text, 16));
                    _lstAddress.Items.Add(_txtAddress.Text);
                    _lstMemory.Items.Add("-");
                    _lblError.Text = "";
                } else {
                    _lblError.Text = "ERROR: Invalid hexidecimal number";
                }
            }
        }

        ///<summary>
        /// TODO
        ///</summary>
        private void btnChangeCommModeOnClick(object sender, EventArgs e) {
            // TODO
        }
    }
}

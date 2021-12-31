using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace BirdsEye {
    [ExternalTool("BirdsEye")]
    public partial class CustomMainForm : ToolFormBase, IExternalToolForm {
        /// <remarks>
		/// <see cref="ApiContainer"/> can be used as a shorthand for accessing 
        /// the various APIs, more like the Lua syntax.
		/// </remarks>
		public ApiContainer? _apiContainer { get; set; }

		private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        private Label _lblRomName;
        private Label _lblMemory;
        private ListBox _lstAddress;
        private ListBox _lstMemory;
        private Button _btnNewAddress;
        private TextBox _txtAddress;

        private List<long> _memoryAddresses = new List<long>();

        protected override string WindowTitleStatic => "BirdsEye";

        ///<summary>
        /// Main form constructor.
        /// Code is executed only once (when EmuHawk.exe is launched).
        ///</summary>
        public CustomMainForm() {
            ClientSize = new Size(320, 320);
            SuspendLayout();

            _lblRomName = new Label {
                AutoSize = true,
                Location = new Point(0, 0),
            };
            _lblMemory = new Label {
                AutoSize = true,
                Location = new Point(0, 25),
                Text = "Memory"
            };
            _btnNewAddress = new Button {
                Location = new Point(0, 45),
                Size = new Size(100, 25),
                Text = "Add Address:"
            };
            _txtAddress = new TextBox {
                Location = new Point(100, 45),
			    Size = new Size(70, 20)
            };
            _lstAddress = new ListBox {
                Location = new Point(100, 70),
                Size = new Size(70, 150)
            };
            _lstMemory = new ListBox {
                Location = new Point(0, 70),
                Size = new Size(100, 150)
            };

            Controls.Add(_lblRomName);
            Controls.Add(_lblMemory);
            Controls.Add(_btnNewAddress);
            Controls.Add(_txtAddress);
            Controls.Add(_lstAddress);
            Controls.Add(_lstMemory);
            ResumeLayout();

            _btnNewAddress.Click += btnNewAddress_Click;
        }

        ///<summary>
        /// Executed once after the constructor, and again every time a rom is
        /// loaded or reloaded.
        ///</summary>
        public override void Restart() {
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
        /// Determine if all characters in s are valid hexadecimal digits.
        /// Returns false if an invalid digit is found, otherwise this returns true.
        ///</summary>
        private bool IsHexadecimal(string s) {
            string hexadecimalChars = "0987654321ABCDEFabcdef";
            foreach (char ch in s) {
                if (!hexadecimalChars.Contains(ch)) {
                    return false;
                }
            }
            return true;
        }

        ///<summary>
        /// Executed after every frame.
        ///</summary>
        protected override void UpdateAfter() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                uint[] memoryData = ReadMemory(_memoryAddresses.ToArray());
                for (int i = 0; i < _lstMemory.Items.Count; i++) {
                    _lstMemory.Items[i] = memoryData[i].ToString();
                }
            }
        }

        ///<summary>
        /// Update the lstAddress, lstMemory, and memoryAddresses with the entered 
        /// address in txtAddress.
        ///</summary>
        private void btnNewAddress_Click(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(_txtAddress.Text) && !_lstAddress.Items.Contains(_txtAddress.Text)) {
                if (IsHexadecimal(_txtAddress.Text)) {
                    _memoryAddresses.Add(Convert.ToInt64(_txtAddress.Text, 16));
                    _lstAddress.Items.Add(_txtAddress.Text);
                    _lstMemory.Items.Add("-");
                }
            }
        }
    }
}

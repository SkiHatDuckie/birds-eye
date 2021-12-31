using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace BirdsEye {
    [ExternalTool("BirdsEye")]
    public partial class CustomMainForm : ToolFormBase, IExternalToolForm {
        /// <remarks>
		/// <see cref="ApiContainer"/> can be used as a shorthand for accessing the various APIs, more like 
        /// the Lua syntax.
		/// </remarks>
		public ApiContainer? _apiContainer { get; set; }

		private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        private Label lblRomName;
        private Label lblMemory;
        private ListBox lstAddress;
        private ListBox lstMemory;
        private Button btnNewAddress;
        private TextBox txtAddress;

        private List<long> memoryAddresses = new List<long>();

        protected override string WindowTitleStatic => "BirdsEye";

        ///<summary>
        /// Main form constructor.
        /// Code is executed only once (when EmuHawk.exe is launched).
        ///</summary>
        public CustomMainForm() {
            ClientSize = new Size(320, 320);
            SuspendLayout();

            lblRomName = new Label {
                AutoSize = true,
                Location = new Point(0, 0),
            };
            lblMemory = new Label {
                AutoSize = true,
                Location = new Point(0, 25),
                Text = "Memory"
            };
            btnNewAddress = new Button {
                Location = new Point(0, 45),
                Size = new Size(100, 25),
                Text = "Add Address:"
            };
            txtAddress = new TextBox {
                Location = new Point(100, 45),
			    Size = new Size(70, 20)
            };
            lstAddress = new ListBox {
                Location = new Point(100, 70),
                Size = new Size(70, 150)
            };
            lstMemory = new ListBox {
                Location = new Point(0, 70),
                Size = new Size(100, 150)
            };

            Controls.Add(lblRomName);
            Controls.Add(lblMemory);
            Controls.Add(btnNewAddress);
            Controls.Add(txtAddress);
            Controls.Add(lstAddress);
            Controls.Add(lstMemory);
            ResumeLayout();

            btnNewAddress.Click += btnNewAddress_Click;
        }

        ///<summary>
        /// Executed once after the constructor, and again every time a rom is loaded or reloaded.
        ///</summary>
        public override void Restart() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                lblRomName.Text = $"Currently loaded: {APIs.GameInfo.GetRomName()}";
            } else {
                lblRomName.Text = "Currently loaded: Nothing";
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
        /// Executed after every frame.
        ///</summary>
        protected override void UpdateAfter() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                uint[] memoryData = ReadMemory(memoryAddresses.ToArray());
                for (int i = 0; i < lstMemory.Items.Count; i++) {
                    lstMemory.Items[i] = memoryData[i].ToString();
                }
            }
        }

        ///<summary>
        /// Update the `lstAddress`, `lstMemory`, and `memoryAddresses` with the entered address in
        /// `txtAddress`. 
        ///
        /// TODO: Handle invalid addresses or addresses that don't exist
        ///</summary>
        private void btnNewAddress_Click(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(txtAddress.Text) && !lstAddress.Items.Contains(txtAddress.Text)) {
                lstAddress.Items.Add(txtAddress.Text);
                lstMemory.Items.Add("-");
                memoryAddresses.Add(Convert.ToInt64(txtAddress.Text, 16));
            }
        }
    }
}

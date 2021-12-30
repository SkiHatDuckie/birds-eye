using System;
using System.Drawing;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace BirdsEye {
    [ExternalTool("BirdsEye")]
    public partial class CustomMainForm : ToolFormBase, IExternalToolForm {
        protected override string WindowTitleStatic => "BirdsEye";

        /// <remarks>
		/// <see cref="ApiContainer"/> can be used as a shorthand for accessing the various APIs, more like the Lua syntax.
		/// </remarks>
		public ApiContainer? _apiContainer { get; set; }

		private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        private Label romNameLabel;

        ///<summary>
        /// Main form constructor.
        /// Code is executed only once (when EmuHawk.exe is launched).
        ///</summary>
        public CustomMainForm() {
            ClientSize = new Size(320, 320);
            SuspendLayout();

            romNameLabel = new Label {
                AutoSize = true,
                Location = new Point(0, 0),
                Size = new Size(35, 13),
            };

            Controls.Add(romNameLabel);
            ResumeLayout();
        }

        ///<summary>
        /// Executed once after the constructor, and again every time a rom
        /// is loaded or reloaded.
        ///</summary>
        public override void Restart() {
            if (APIs.GameInfo.GetRomName() != "Null") {
                romNameLabel.Text = $"Currently loaded: {APIs.GameInfo.GetRomName()}";
            } else {
                romNameLabel.Text = "Currently loaded: Nothing";
            }
        }
    }
}

using System.Drawing;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace Net.MyStuff.MyTool {
    [ExternalTool("BirdsEye")]
    public sealed class CustomMainForm : ToolFormBase, IExternalToolForm {
        protected override string WindowTitleStatic => "BirdsEye";

        public CustomMainForm() {
            ClientSize = new Size(240, 240);
			SuspendLayout();
			Controls.Add(new Label { AutoSize = true, Text = "Hello, world!" });
			ResumeLayout();
        }
    }
}

using System.Drawing;
using System.Windows.Forms;

namespace BirdsEye {
    public partial class CustomMainForm {
        public MenuStrip _mainFormMenu;
        public ToolStripMenuItem _optionSubMenu;

        public FlowLayoutPanel _flpToolControls;

        public GroupBoxEx _grpRomInfo;
        public FlowLayoutPanel _flpRomInfo;
        public Label _lblRomName;

        public GroupBoxEx _grpCommunications;
        public FlowLayoutPanel _flpCommunications;
        public Label _lblCommMode;
        public Button _btnChangeCommMode;
        public Label _lblConnectionStatus;
        public Button _btnDisconnectClient;

        public ListBox _lstError;

        /// <summary>
        /// Initialize all components needed in CustomMainForm.
        /// </summary>
        private void InitializeControls() {
            FormClosing += OnFormClosing;
            ClientSize = new Size(600, 480);
            SuspendLayout();

            _mainFormMenu = new MenuStrip {
                Size = new Size(Width, 25),
                Dock = DockStyle.Top,
            };
            _optionSubMenu = new ToolStripMenuItem {
                Text = "&Options",
                Size = new Size(50, 25),
            };
            _flpToolControls = new FlowLayoutPanel {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                WrapContents = false,
            };
            _grpRomInfo = new GroupBoxEx {
                Text = "ROM Info",
                Size = new Size(280, 160),
                Padding = new Padding(5),
                FlatStyle = FlatStyle.Popup,
            };
            _flpRomInfo = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
            };
            _lblRomName = new Label {
                AutoSize = true,
            };
            _grpCommunications = new GroupBoxEx {
                Text = "Communications",
                Size = new Size(280, 160),
                Padding = new Padding(8),
                FlatStyle = FlatStyle.Flat,
            };
            _flpCommunications = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
            };
            _lblCommMode = new Label {
                Text = "Communication Mode: Manual",
                AutoSize = true,
            };
            _btnChangeCommMode = new Button {
                Text = "Change Mode",
                Size = new Size(100, 25),
            };
            _lblConnectionStatus = new Label {
                Text = "No script found",
                AutoSize = true,
            };
            _btnDisconnectClient = new Button {
                Text = "Disconnect",
                Size = new Size(100, 25),
            };
            _lstError = new ListBox {
                Dock = DockStyle.Fill,
            };

            _mainFormMenu.Items.Add(_optionSubMenu);
            _flpRomInfo.Controls.Add(_lblRomName);
            _grpRomInfo.Controls.Add(_flpRomInfo);
            _flpCommunications.Controls.Add(_lblCommMode);
            _flpCommunications.Controls.Add(_btnChangeCommMode);
            _flpCommunications.Controls.Add(_lblConnectionStatus);
            _flpCommunications.Controls.Add(_btnDisconnectClient);
            _grpCommunications.Controls.Add(_flpCommunications);
            _flpToolControls.Controls.Add(_grpRomInfo);
            _flpToolControls.Controls.Add(_grpCommunications);
            Controls.Add(_lstError);  // Must be added first for DockStype.Fill to work properly
            Controls.Add(_flpToolControls);
            Controls.Add(_mainFormMenu);

            ColorTheme.ApplyColorTheme(this);

            ResumeLayout();

            _optionSubMenu.Click += SubMenuOptionOnClick;
            _btnChangeCommMode.Click += ChangeCommModeButtonOnClick;
            _btnDisconnectClient.Click += DisconnectClientButtonOnClick;
        }
    }
}
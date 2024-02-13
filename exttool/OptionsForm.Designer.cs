using System.Drawing;
using System.Windows.Forms;

namespace BirdsEye {
    public partial class OptionsForm {
        public TableLayoutPanel _tlpOptionGroups;

        public GroupBoxEx _grpSocketAddress;
        public Label _lblHost;
        public Label _lblPort;
        public TextBox _txtHost;
        public TextBox _txtPort;

        public GroupBoxEx _grpLogging;
        public Label _lblLogLevel;
        public TextBox _txtLogLevel;
        public Label _lblLogLevelRange;

        public GroupBoxEx _grpSocket;
        public Label _lblTimeout;
        public TextBox _txtTimeout;
        public Label _lblBufSize;
        public TextBox _txtBufSize;

        public Label _lblSubmit;
        public Button _btnSubmit;
        public Label _lblError;

        /// <summary>
        /// Initialize all components needed in OptionsForm.
        /// </summary>
        private void InitializeControls() {
            ClientSize = new Size(480, 320);
            SuspendLayout();

            _tlpOptionGroups = new TableLayoutPanel {
                Location = new Point(0, 0),
                AutoSize = true,
                RowCount = 2,
                ColumnCount = 2,
            };
            _grpSocketAddress = new GroupBoxEx {
                Text = "Socket Address",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
            };
            _lblHost = new Label {
                Text = "Host:",
                Location = new Point(15, 20),
                AutoSize = true,
            };
            _lblPort = new Label {
                Text = "Port:",
                Location = new Point(15, 45),
                AutoSize = true,
            };
            _txtHost = new TextBox {
                Text = _config.host,
                Location = new Point(75, 20),
                Size = new Size(75, 25),
            };
            _txtPort = new TextBox {
                Text = _config.port.ToString(),
                Location = new Point(75, 45),
                Size = new Size(75, 25),
            };
            _grpLogging = new GroupBoxEx {
                Text = "Logging",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
            };
            _lblLogLevel = new Label {
                Text = "Minimum Log Level:",
                Location = new Point(15, 20),
                AutoSize = true,
            };
            _txtLogLevel = new TextBox {
                Text = _config.logLevel.ToString(),
                Location = new Point(15, 40),
                AutoSize = true,
            };
            _lblLogLevelRange = new Label {
                Text = "0 (debug) - 4 (crash)",
                Location = new Point(15, 65),
                AutoSize = true,
            };
            _grpSocket = new GroupBoxEx {
                Text = "Sockets",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
            };
            _lblTimeout = new Label {
                Text = "Socket Timeout (ms):",
                Location = new Point(15, 20),
                AutoSize = true,
            };
            _txtTimeout = new TextBox {
                Text = _config.socketTimeout.ToString(),
                Location = new Point(15, 40),
                AutoSize = true,
            };
            _lblBufSize = new Label {
                Text = "Receiver Buffer Size (bytes):",
                Location = new Point(15, 65),
                AutoSize = true,
            };
            _txtBufSize = new TextBox {
                Text = _config.socketBufSize.ToString(),
                Location = new Point(15, 85),
                AutoSize = true,
            };
            _lblSubmit = new Label {
                Text = "",
                Location = new Point(390, 240),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                AutoSize = true,
            };
            _btnSubmit = new Button {
                Text = "Submit",
                Location = new Point(390, 270),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                AutoSize = true,
            };
            _lblError = new Label {
                Text = "Close and reopen BirdsEye after submit for changes to take effect.",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(15, 300),
                AutoSize = true,
            };

            _tlpOptionGroups.Controls.Add(_grpSocketAddress);
            _tlpOptionGroups.Controls.Add(_grpLogging);
            _tlpOptionGroups.Controls.Add(_grpSocket);
            _grpSocketAddress.Controls.Add(_lblHost);
            _grpSocketAddress.Controls.Add(_lblPort);
            _grpSocketAddress.Controls.Add(_txtHost);
            _grpSocketAddress.Controls.Add(_txtPort);
            _grpLogging.Controls.Add(_lblLogLevel);
            _grpLogging.Controls.Add(_txtLogLevel);
            _grpLogging.Controls.Add(_lblLogLevelRange);
            _grpSocket.Controls.Add(_lblTimeout);
            _grpSocket.Controls.Add(_txtTimeout);
            _grpSocket.Controls.Add(_lblBufSize);
            _grpSocket.Controls.Add(_txtBufSize);
            Controls.Add(_tlpOptionGroups);
            Controls.Add(_lblSubmit);
            Controls.Add(_btnSubmit);
            Controls.Add(_lblError);

            ColorTheme.ApplyColorTheme(this);

            ResumeLayout();

            _btnSubmit.Click += SubmitButtonOnClick;
        }
    }
}
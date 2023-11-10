using System;
using System.Drawing;
using System.Windows.Forms;

namespace BirdsEye {
    public class OptionsForm : Form {
        private readonly Config _config;

        private readonly TableLayoutPanel _tlpOptionGroups;

        private readonly GroupBox _grpSocketAddress;
        private readonly Label _lblHost;
        private readonly Label _lblPort;
        private readonly TextBox _txtHost;
        private readonly TextBox _txtPort;

        private readonly GroupBox _grpLogging;
        private readonly Label _lblLogLevel;
        private readonly TextBox _txtLogLevel;
        private readonly Label _lblLogLevelRange;

        private readonly GroupBox _grpSocket;
        private readonly Label _lblTimeout;
        private readonly TextBox _txtTimeout;

        private readonly Label _lblSubmit;
        private readonly Button _btnSubmit;
        private readonly Label _lblError;

        public OptionsForm(Config config) {
            _config = config;

            ClientSize = new Size(480, 320);
            SuspendLayout();

            _tlpOptionGroups = new TableLayoutPanel {
                Location = new Point(0, 0),
                AutoSize = true,
                RowCount = 2,
                ColumnCount = 2,
            };
            _grpSocketAddress = new GroupBox {
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
            _grpLogging = new GroupBox {
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
            _grpSocket = new GroupBox {
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
            _lblSubmit = new Label {
                Text = "",
                ForeColor = Color.Blue,
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
                ForeColor = Color.Red,
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
            Controls.Add(_tlpOptionGroups);
            Controls.Add(_lblSubmit);
            Controls.Add(_btnSubmit);
            Controls.Add(_lblError);
            ResumeLayout();

            _btnSubmit.Click += SubmitButtonOnClick;
        }

        /// <summary>
        /// Updates birdconfig.txt with the inputted values of each option.
        /// </summary>
        private void SubmitButtonOnClick(object sender, EventArgs e) {
            _config.host = _txtHost.Text;
            _config.port = Convert.ToInt32(_txtPort.Text);
            _config.logLevel = Convert.ToInt32(_txtLogLevel.Text);
            _config.socketTimeout = Convert.ToInt32(_txtTimeout.Text);
            _config.UpdateConfigs();
            _lblSubmit.Text = "Submitted!";
        }
    }
}
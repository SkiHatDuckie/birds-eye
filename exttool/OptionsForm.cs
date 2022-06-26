using System;
using System.Drawing;
using System.Windows.Forms;

namespace BirdsEye {
    public class OptionsForm : Form {
        private Config _config;

        private Label _lblHost;
        private Label _lblPort;
        private TextBox _txtHost;
        private TextBox _txtPort;
        private GroupBox _grpSocketAddress;

        private Label _lblLogLevel;
        private TextBox _txtLogLevel;
        private Label _lblLogLevelRange;
        private GroupBox _grpLogging;

        private Label _lblTimeout;
        private TextBox _txtTimeout;
        private GroupBox _grpSocket;

        private Label _lblSubmit;
        private Button _btnSubmit;
        private Label _lblError;

        public OptionsForm(Config config) {
            _config = config;

            ClientSize = new Size(480, 320);
            SuspendLayout();

            _lblHost = new Label {
                Text = "Host:",
                Location = new Point(15, 20),
                AutoSize = true
            };
            _lblPort = new Label {
                Text = "Port:",
                Location = new Point(15, 45),
                AutoSize = true
            };
            _txtHost = new TextBox {
                Text = _config.host,
                Location = new Point(75, 20),
                Size = new Size(75, 25)
            };
            _txtPort = new TextBox {
                Text = _config.port.ToString(),
                Location = new Point(75, 45),
                Size = new Size(75, 25)
            };
            _grpSocketAddress = new GroupBox {
                Text = "Socket Address",
                Location = new Point(10, 0),
                AutoSize = true
            };
            _lblLogLevel = new Label {
                Text = "Minimum Log Level:",
                Location = new Point(15, 20),
                AutoSize = true
            };
            _txtLogLevel = new TextBox {
                Text = _config.logLevel.ToString(),
                Location = new Point(15, 40),
                AutoSize = true
            };
            _lblLogLevelRange = new Label {
                Text = "0 (debug) - 4 (crash)",
                Location = new Point(15, 65),
                AutoSize = true
            };
            _grpLogging = new GroupBox {
                Text = "Logging",
                Location = new Point(10, 100),
                AutoSize = true
            };
            _lblTimeout = new Label {
                Text = "Socket Timeout (ms):",
                Location = new Point(15, 20),
                AutoSize = true
            };
            _txtTimeout = new TextBox {
                Text = _config.socketTimeout.ToString(),
                Location = new Point(15, 40),
                AutoSize = true
            };
            _grpSocket = new GroupBox {
                Text = "Sockets",
                Location = new Point(225, 0),
                AutoSize = true
            };
            _lblSubmit = new Label {
                Text = "",
                ForeColor = Color.Blue,
                Location = new Point(380, 235),
                AutoSize = true
            };
            _btnSubmit = new Button {
                Text = "Submit",
                Location = new Point(380, 270),
                AutoSize = true
            };
            _lblError = new Label {
                Text = "Close and reopen BirdsEye after submit for changes to take effect.",
                ForeColor = Color.Red,
                Location = new Point(0, this.Height - 72),
                AutoSize = true
            };

            _grpSocketAddress.FlatStyle = FlatStyle.Flat;
            _grpSocketAddress.Controls.Add(_lblHost);
            _grpSocketAddress.Controls.Add(_lblPort);
            _grpSocketAddress.Controls.Add(_txtHost);
            _grpSocketAddress.Controls.Add(_txtPort);
            _grpLogging.FlatStyle = FlatStyle.Flat;
            _grpLogging.Controls.Add(_lblLogLevel);
            _grpLogging.Controls.Add(_txtLogLevel);
            _grpLogging.Controls.Add(_lblLogLevelRange);
            _grpSocket.FlatStyle = FlatStyle.Flat;
            _grpSocket.Controls.Add(_lblTimeout);
            _grpSocket.Controls.Add(_txtTimeout);
            Controls.Add(_grpSocketAddress);
            Controls.Add(_grpLogging);
            Controls.Add(_grpSocket);
            Controls.Add(_lblSubmit);
            Controls.Add(_btnSubmit);
            Controls.Add(_lblError);
            ResumeLayout();

            _btnSubmit.Click += btnSubmitOnClick;
        }

        /// <summary>
        /// Updates birdconfig.txt with the inputted values of each option.
        /// </summary>
        private void btnSubmitOnClick(object sender, EventArgs e) {
            _config.host = _txtHost.Text;
            _config.port = Convert.ToInt32(_txtPort.Text);
            _config.logLevel = Convert.ToInt32(_txtLogLevel.Text);
            _config.socketTimeout = Convert.ToInt32(_txtTimeout.Text);
            _config.UpdateConfigs();
            _lblSubmit.Text = "Submitted!";
        }
    }
}
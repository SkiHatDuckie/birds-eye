using System;
using System.Windows.Forms;

namespace BirdsEye {
    public sealed partial class OptionsForm : Form {
        private readonly Config _config;

        public OptionsForm(Config config) {
            _config = config;
            InitializeControls();
        }

        /// <summary>
        /// Update birdconfig.txt with the inputted values of each option.
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
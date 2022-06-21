using System;
using System.IO;
using System.Text;

namespace BirdsEye {
    public class Config {
        // Default values
        public string host = "127.0.0.1";
        public int port = 8080;
        public int logLevel = 0;
        public int socketTimeout = 10000;

        public Config() {
            if (!File.Exists("birdconfig.txt")) {
                CreateNewConfigFile();
            } else {
                GetConfigs();
            }
        }

        /// <summary>
        /// Returns an array of formatted strings as bytes containing config data.
        /// </summary>
        private byte[] FormatConfigFields() {
            return new UTF8Encoding(true).GetBytes(
                "host=" + host + "\n" +
                "port=" + port.ToString() + "\n" +
                "logLevel=" + logLevel.ToString() + "\n" +
                "socketTimeout" + socketTimeout.ToString()
            );
        }

        /// <summary>
        /// Creates a new birdconfig.txt file, and fills it with the default values stored in 
        /// <see cref="Config"/>.
        /// </summary>
        private void CreateNewConfigFile() {
            byte[] data = FormatConfigFields();
            using (FileStream file = File.Create("birdconfig.txt")) {
                file.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Reads through the birdconfig.txt file and changes any of <see cref="Config"/>'s fields accordingly.
        /// </summary>
        private void GetConfigs() {
            using (FileStream file = new FileStream("birdconfig.txt", FileMode.Open, FileAccess.Read)) {
                Byte[] bytes = new Byte[1024];
                UTF8Encoding data = new UTF8Encoding(true);
                file.Read(bytes, 0, bytes.Length);
                string[] configs = data.GetString(bytes).Split('\n');
                host = configs[0].Substring(configs[0].IndexOf('=') + 1);
                port = Convert.ToInt32(configs[1].Substring(configs[1].IndexOf('=') + 1));
                logLevel = Convert.ToInt32(configs[2].Substring(configs[2].IndexOf('=') + 1));
                socketTimeout = Convert.ToInt32(configs[3].Substring(configs[3].IndexOf('=') + 1));
            }
        }

        /// <summary>
        /// Updates birdconfig.txt with the current fields of <see cref="Config"/>.
        /// </summary>
        public void UpdateConfigs() {
            byte[] data = FormatConfigFields();
            using (FileStream file = new FileStream("birdconfig.txt", FileMode.Open)) {
                file.Write(data, 0, data.Length);
            }
        }
    }
}
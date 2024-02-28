using System;
using System.IO;
using System.Text;

namespace BirdsEye {
    public class Config {
        // Default values
        public string host = "127.0.0.1";
        public int port = 8080;
        public int logLevel = 1;
        public int socketTimeout = 10000;
        public int socketBufSize = 2048;

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
                string.Join("\n", new string[] { 
                    "host=" + host,
                    "port=" + port.ToString(),
                    "logLevel=" + logLevel.ToString(),
                    "socketTimeout=" + socketTimeout.ToString(),
                    "socketBufSize=" + socketBufSize.ToString(),
                })
            );
        }

        /// <summary>
        /// Creates a new birdconfig.txt file, and fills it with the default values stored in 
        /// <see cref="Config"/>.
        /// </summary>
        private void CreateNewConfigFile() {
            byte[] data = FormatConfigFields();
            using FileStream file = File.Create("birdconfig.txt");
            file.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Reads through the birdconfig.txt file and changes any of <see cref="Config"/>'s fields accordingly.
        /// </summary>
        private void GetConfigs() {
            using FileStream file = new("birdconfig.txt", FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[1024];
            UTF8Encoding data = new(true);
            file.Read(bytes, 0, bytes.Length);
            string[] configs = data.GetString(bytes).Split('\n');
            foreach (string line in configs) {
                if (line.IndexOf('=') != -1) {
                    string config = line.Substring(0, line.IndexOf('='));
                    string value = line.Substring(line.IndexOf('=') + 1);
                    switch (config) {
                        case "host":
                            host = value;
                            break;
                        case "port":
                            port = Convert.ToInt32(value);
                            break;
                        case "logLevel":
                            logLevel = Convert.ToInt32(value);
                            break;
                        case "socketTimeout":
                            socketTimeout = Convert.ToInt32(value);
                            break;
                        case "socketBufSize":
                            socketBufSize = Convert.ToInt32(value);
                            break;
                    };
                }
            }
        }

        /// <summary>
        /// Updates birdconfig.txt with the current fields of <see cref="Config"/>.
        /// </summary>
        public void UpdateConfigs() {
            byte[] data = FormatConfigFields();
            using FileStream file = new("birdconfig.txt", FileMode.Open);
            file.Write(data, 0, data.Length);
        }
    }
}
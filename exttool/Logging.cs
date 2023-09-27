using System;
using System.Collections.Generic;

namespace BirdsEye {
    /// <summary>
    /// An object for writing external tool information to the BizHawk log window
    /// </summary>
    public class Logging {
        /// <summary>
        /// A collection all log levels, mapped to a string value.
        /// </summary>
        /// <remarks>
        /// debug (0) - Information that would only really be needed for debugging purposes.<br/>
        /// info (1) - General information about the external tool.<br/>
        /// warning (2) - Handled exceptions.<br/>
        /// error (3) - Unhandled exceptions.<br/>
        /// crash (4) - Information given out if the external tool crashes.<br/>
        /// </remarks>
        private readonly static Dictionary<int, string> LEVELS = new() {
            {0, "debug"}, {1, "info"}, {2, "warning"}, {3, "error"}, {4, "crash"}
        };
        /// <summary>
        /// The minimum log level to write messages to output for.
        /// </summary>
        private readonly int _minLevel = 1;

        public Logging(int minLevel) {
            _minLevel = minLevel;
        }

        /// <summary>
        /// Writes a log message to the BizHawk log window.
        /// </summary>
        public void Write(int level, string msg) {
            if (level >= _minLevel) {
                Console.WriteLine(":" + LEVELS[level].ToUpper() + ": " + msg);
            }
        }
    }
}
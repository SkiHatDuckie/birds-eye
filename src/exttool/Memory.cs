using System;
using System.Collections.Generic;

using BizHawk.Client.Common;

namespace BirdsEye {
    public class Memory {
        /// <remarks>
		/// <see cref="ApiContainer"/> can be used as a shorthand for accessing 
        /// the various APIs, more like the Lua syntax.
		/// </remarks>
		public ApiContainer? _apiContainer { get; set; }

		private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        private List<long> _addressList = new List<long>();
        private List<int> _memoryList = new List<int>();

        ///<summary>
        /// Adds a memory address to `_addressList`.
        /// A corresponding value (-1) is added to `_memoryList`.
        /// Precondition: `address` represents a valid hexadecimal value.
        ///</summary>
        public void AddAddress(long address) {
            _addressList.Add(address);
            _memoryList.Add(-1);
        }

        ///<summary>
        /// Reads each address in `_addressList` in main memory.
        /// Stores the gathered values in `_memoryList`.
        /// Needs the API interface in order to read memory from the emulator.
        ///</summary>
        public int[] ReadMemory(ApiContainer APIs) {
            for (int i = 0; i < _addressList.Count; i++) {
                _memoryList[i] = (int) APIs.Memory.ReadByte(_addressList[i]);
            }
            return _memoryList.ToArray();
        }

        ///<summary>
        /// Converts all memory data stored into a concatenated string seperated by a ';'.
        /// '-' is is added if data has been collected from an address 
        /// (i.e. the value is still -1).
        ///</summary>
        public string FormatMemory() {
            string result = "";
            foreach (int i in _memoryList) {
                if (i == -1) {
                    result += "-;";
                } else {
                    result += i + ";";
                }
            }
            return (result != "") ? result : ";";
        }
    }
}

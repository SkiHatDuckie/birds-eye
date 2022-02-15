using System;
using System.Collections.Generic;

using BizHawk.Client.Common;

namespace BirdsEye {
    public class Memory {
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
        /// Add memory addresses from a string.
        ///</summary>
        public void AddAddressesFromString(string str) {
            string[] addressList = str.Substring(7).Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string addr in addressList) {
                AddAddress(Convert.ToInt64(addr, 10));
            }
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
        /// Converts all addresses and the memory data stored into a 
        /// concatenated string, formatted as such:
        /// "ADDR:DATA;ADDR:DATA;..." where both ADDR and DATA are in
        /// decimal notation.
        /// '-' is is added if data has been collected from an address 
        /// (i.e. the value is still -1).
        ///</summary>
        public string FormatMemory() {
            string result = "";
            for (int i = 0; i < _memoryList.Count; i++) {
                result += _addressList[i] + ":";
                if (i == -1) {
                    result += "-;";
                } else {
                    result += _memoryList[i] + ";";
                }
            }
            return (result != "") ? result : ";";
        }
    }
}
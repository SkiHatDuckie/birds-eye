using System;
using System.Collections.Generic;

using BizHawk.Client.Common;

namespace BirdsEye {
    public class Memory {
        private readonly List<long> _addressList = new();
        private readonly List<int> _memoryList = new();
        private readonly Logging _log;

        public Memory(Logging log) {
            _log = log;
        }

        ///<summary>
        /// Add a memory address to `_addressList`.<br/>
        /// An integer `-1` is added to `_memoryList`.<br/>
        /// Precondition: `address` represents a valid hexadecimal value.
        ///</summary>
        private void AddAddress(long address) {
            _log.Write(0, $"Adding address {address} to address list.");
            _addressList.Add(address);
            _memoryList.Add(-1);
        }

        ///<summary>
        /// Add memory addresses from a request.
        /// Returns an empty string on completion.
        ///</summary>
        public string AddAddressesFromRequest(string req) {
            if (!string.IsNullOrEmpty(req)) {
                string[] addressList = req.Trim(';').Split(';');
                foreach (string addr in addressList) {
                    AddAddress(Convert.ToInt64(addr, 10));
                }
            }
            return "";
        }

        ///<summary>
        /// Read each address in `_addressList` in main memory.<br/>
        /// Store the gathered values in `_memoryList`.<br/>
        /// Needs the API interface in order to read memory from the emulator.
        ///</summary>
        public int[] ReadMemory(ApiContainer APIs) {
            for (int i = 0; i < _addressList.Count; i++) {
                _memoryList[i] = (int) APIs.Memory.ReadByte(_addressList[i]);
            }
            return _memoryList.ToArray();
        }

        ///<summary>
        /// Concatenate all addresses and memory data into one string,
        /// formatted as such:<br/>
        /// "ADDR:DATA;ADDR:DATA;..." where both ADDR and DATA are in
        /// decimal form.<br/>
        /// `-1` is added if data has not been collected from an address.
        ///</summary>
        public string FormatMemory() {
            string result = "";
            for (int i = 0; i < _memoryList.Count; i++) {
                result += _addressList[i] + ":";
                if (i == -1) {
                    result += "-1;";
                } else {
                    result += _memoryList[i] + ";";
                }
            }
            return (result != "") ? result : ";";
        }

        ///<summary>
        /// Remove any addresses that have been set and empty both
        /// `_addressList` and `_memoryList`.
        ///</summary>
        public void ClearAddresses() {
            _log.Write(0, "Clearing address and memory lists.");
            _addressList.Clear();
            _memoryList.Clear();
        }

        public string MemoryOnRequest(ApiContainer APIs) {
            ReadMemory(APIs);
            return FormatMemory();
        }
    }
}
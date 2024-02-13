using BizHawk.Client.Common;

namespace BirdsEye {
    public class Emulation {
        private readonly Logging _log;

        public Emulation(Logging log) {
            _log = log;
        }

        public string GetFramecount(ApiContainer APIs) {
            return APIs.Emulation.FrameCount().ToString();
        }

        public string GetBoardName(ApiContainer APIs) {
            return APIs.Emulation.GetBoardName();
        }
    }
}
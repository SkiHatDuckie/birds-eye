using BizHawk.Client.Common;

namespace BirdsEye {
    public class Emulation {
        private readonly Logging _log;

        public Emulation(Logging log) {
            _log = log;
        }

        public Response GetFramecount(ApiContainer APIs) {
            return new Response(APIs.Emulation.FrameCount().ToString());
        }

        public Response GetBoardName(ApiContainer APIs) {
            return new Response(APIs.Emulation.GetBoardName());
        }
    }
}
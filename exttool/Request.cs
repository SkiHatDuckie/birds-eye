namespace BirdsEye {
    public readonly struct Request {
        public Request(string tag, string data) {
            Tag = tag;
            Data = data;
        }

        public string Tag { get; }
        public string Data { get; }

        public override string ToString() => $"{Tag}, {Data}";
    }
}
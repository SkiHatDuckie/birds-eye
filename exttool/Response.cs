namespace BirdsEye {
    public readonly struct Response {
        public Response(string data) {
            Data = data;
        }
        public string Data { get; }
        public override string ToString() => $"{Data}";
    }
}
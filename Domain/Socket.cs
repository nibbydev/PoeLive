namespace Domain {
    public class Socket {
        public SocketColor? Color { get; set; }
        public bool Linked { get; set; }

        public override string ToString() {
            return $"[{Color} {Linked}]";
        }
    }
}
namespace Domain.PoeTrade.ApiDeserializer {
    public class Socket {
        public enum SocketColor {
            Red,
            Green,
            Blue,
            White,
            Abyss
        }

        public SocketColor? Color { get; set; }
        public bool Linked { get; set; }

        public override string ToString() {
            return $"[{Color} {Linked}]";
        }
    }
}
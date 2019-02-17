namespace Domain {
    public class Stat {
        public StatType Type { get; set; }
        public double? Value { get; set; }

        public override string ToString() {
            return $"[{Type} {Value}]";
        }
    }
}
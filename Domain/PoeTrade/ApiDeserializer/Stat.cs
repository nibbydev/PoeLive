namespace Domain.PoeTrade.ApiDeserializer {
    public class Stat {
        public enum StatType {
            Quality,
            Phys,
            Elem,
            Aps,
            Dps,
            PDps,
            EDps,
            Armour,
            Evasion,
            Shield,
            Block,
            Crit,
            Level
        }

        public StatType Type { get; set; }
        public double? Value { get; set; }

        public override string ToString() {
            return $"[{Type} {Value}]";
        }
    }
}
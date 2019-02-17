namespace Domain.PoeTrade {
    public class Requirement {
        public enum RequirementType {
            Level,
            ItemLevel,
            MaxSocket,
            Int,
            Str,
            Dex
        }
        
        public RequirementType? Type { get; set; }
        public int? Value { get; set; }

        public override string ToString() {
            return $"[{Type} {Value}]";
        }
    }
}
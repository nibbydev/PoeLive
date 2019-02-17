namespace Domain {
    public class Requirement {
        public RequirementType? Type { get; set; }
        public int? Value { get; set; }

        public override string ToString() {
            return $"[{Type} {Value}]";
        }
    }
}
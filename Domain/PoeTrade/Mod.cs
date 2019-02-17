using System.Text;

namespace Domain.PoeTrade {
    public class Mod {
        public string Name { get; set; }
        public double? Value { get; set; }
        public string TierShort { get; set; }
        public string TierLong { get; set; }

        public override string ToString() {
            var stringBuilder = new StringBuilder("[");

            if (Value > 0) {
                stringBuilder.Append(Value);
                stringBuilder.Append(" ");
            }

            stringBuilder.Append(Name);

            if (TierShort != null) stringBuilder.Append(" " + TierShort);
            if (TierLong != null) stringBuilder.Append(" " + TierLong);

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
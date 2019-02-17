using System.Text;

namespace Domain {
    public class Item {
        public string Seller { get; set; }
        public string Buyout { get; set; }
        public string Ign { get; set; }
        public string League { get; set; }
        public string Name { get; set; }
        public string Tab { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public string Icon { get; set; }
        public int? StackSize { get; set; }
        public Socket[] Sockets { get; set; }
        public int? LargestLink { get; set; }
        public bool Corrupted { get; set; }
        public Requirement[] Requirements { get; set; }
        public Mod[] Mods { get; set; }
        public Stat[] Stats { get; set; }
        public Influence? Influence { get; set; }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($" - Name: {Name}\n");
            stringBuilder.Append($" - Seller: {Seller}\n");
            stringBuilder.Append($" - Buyout: {Buyout}\n");
            stringBuilder.Append($" - Ign: {Ign}\n");
            stringBuilder.Append($" - League: {League}\n");
            stringBuilder.Append($" - Tab: {Tab}\n");
            stringBuilder.Append($" - X: {X}\n");
            stringBuilder.Append($" - Y: {Y}\n");
            stringBuilder.Append($" - Icon: {Icon}\n");

            if (StackSize != null) {
                stringBuilder.Append($" - StackSize: {StackSize}\n");
            }
            
            if (Influence != null) {
                stringBuilder.Append($" - Influence: {Influence}\n");
            }

            if (Sockets != null) {
                stringBuilder.Append(" - Sockets: \n");
                foreach (var socket in Sockets) {
                    stringBuilder.Append($"   - {socket}\n");
                }
            }

            if (LargestLink != null) {
                stringBuilder.Append($" - LargestLink: {LargestLink}\n");
            }

            stringBuilder.Append($" - Corrupted: {Corrupted}\n");

            if (Requirements != null) {
                stringBuilder.Append(" - Requirements: \n");
                foreach (var req in Requirements) {
                    stringBuilder.Append($"   - {req}\n");
                }
            }

            if (Mods != null) {
                stringBuilder.Append(" - Mods: \n");
                foreach (var mod in Mods) {
                    stringBuilder.Append($"   - {mod}\n");
                }
            }

            if (Stats != null) {
                stringBuilder.Append(" - Stats: \n");
                foreach (var stat in Stats) {
                    stringBuilder.Append($"   - {stat}\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
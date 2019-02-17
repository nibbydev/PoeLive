using System;
using CsQuery.ExtensionMethods.Internal;
using Domain;

namespace Program {
    public static class Program {
        static Program() {
            BaseConnection.DispatchItem = DispatchItem;
            BaseConnection.RemoveActive = RemoveActive;
            BaseConnection.PoeSessionId = "";
        }

        private static void Main(string[] args = null) {
            var urls = new[] {
                "osikarikinoami"
            };
            
            var urls2 = new[] {
                "NV6ofp"
            };

            // Append CLI input
            if (args != null && args.Length > 0) {
                urls.AddRange(args);
            }

            try {
                Service.PoeTrade.Controller.Run(urls);
                Service.PathOfExile.Controller.Run(urls2);
                Console.ReadKey(true);
            } finally {
                Service.PoeTrade.Controller.Stop();
                Service.PathOfExile.Controller.Stop();
            }
        }

        private static void DispatchItem(Item[] items) {
            throw new NotImplementedException();
        }

        private static void RemoveActive(string uniq) {
            throw new NotImplementedException();
        }
    }
}
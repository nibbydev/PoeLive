using System;
using CsQuery.ExtensionMethods.Internal;
using Domain;

namespace Program {
    public static class Program {
        static Program() {
            Service.PoeTrade.Connection.DispatchItem = DispatchItem;
            Service.PoeTrade.Connection.RemoveActive = RemoveActive;
        }

        private static void Main(string[] args = null) {
            var urls = new[] {
                "osikarikinoami",
                "urinotamotasan"
            };

            // Append CLI input
            if (args != null && args.Length > 0) {
                urls.AddRange(args);
            }

            try {
                Service.PoeTrade.Controller.Run(urls);
                Console.ReadKey(true);
            } finally {
                Service.PoeTrade.Controller.Stop();
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
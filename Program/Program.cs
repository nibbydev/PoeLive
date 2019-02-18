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
                "http://poe.trade/search/osikarikinoami",
                "https://www.pathofexile.com/trade/search/Betrayal/NV6ofp",
                "test"
            };

            // Append CLI input
            urls.AddRange(args);

            // Add connection urls
            foreach (var url in urls) {
                try {
                    Service.Controller.AddConnection(url);
                } catch (Exception e) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }

            try {
                Service.Controller.Run();
                Console.ReadKey(true);
            } finally {
                Service.Controller.Stop();
            }
        }

        private static void DispatchItem(Item[] items) {
            //throw new NotImplementedException();
        }

        private static void RemoveActive(string uniq) {
            //throw new NotImplementedException();
        }
    }
}
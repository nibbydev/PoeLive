using System;
using CsQuery.ExtensionMethods.Internal;
using Domain;
using Service;
using Service.Connection;

namespace Program {
    public static class Program {
        static Program() {
            BaseConnection.DispatchNewItem = DispatchNewItem;
            BaseConnection.DispatchDelItem = DispatchDelItem;
            BaseConnection.PoeSessionId = "";
        }

        private static void Main(string[] args = null) {
            var urls = new[] {
                //"http://poe.trade/search/osikarikinoami", // exalted orb
                "https://www.pathofexile.com/trade/search/Betrayal/NV6ofp", // exalted orb
                //"https://www.pathofexile.com/trade/search/Betrayal/d8OvUJ", // non-unique body armour
                "test"
            };

            // Append CLI input
            urls.AddRange(args);

            // Add connection urls
            foreach (var url in urls) {
                try {
                    Controller.AddConnection(url);
                } catch (Exception e) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }

            try {
                Controller.Run();
                Console.ReadKey(true);
            } finally {
                Controller.Stop();
            }
        }

        private static void DispatchNewItem(Item item) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private static void DispatchDelItem(string identifier) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
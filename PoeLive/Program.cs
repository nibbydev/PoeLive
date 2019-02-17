using System;
using System.Collections.Generic;

namespace PoeLive {
    public static class Program {
        private static readonly List<Connection> _connections = new List<Connection>();
        

        private static void Main(string[] args) {
            var urls = new[] {
                //"osikarikinoami",
                "iatahidosaniwo",
                //"urinotamotasan",
            };

            try {
                foreach (var url in urls) {
                    _connections.Add(new Connection(url));
                }
            
                Console.ReadKey(true);
            } finally {
                foreach (var connection in _connections) {
                    connection.DeleteSocket();
                }

                var allDone = false;
                while (!allDone) {
                    foreach (var connection in _connections) {
                        if (connection.IsConnected()) {
                            allDone = false;
                            break;
                        }
                        
                        allDone = true;
                    }
                }
            }
        }
    }
}
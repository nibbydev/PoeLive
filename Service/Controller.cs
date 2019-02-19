using System;
using System.Collections.Generic;
using Domain;
using Service.Connection;


namespace Service {
    public static class Controller {
        private static readonly List<BaseConnection> Connections = new List<BaseConnection>();

        public static void AddConnection(string url) {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentException("Null/empty url");
            }

            if (PtConnection.UrlRegex.IsMatch(url)) {
                Connections.Add(new PtConnection(url));
                return;
            }

            if (PoeConnection.UrlRegex.IsMatch(url)) {
                Connections.Add(new PoeConnection(url));
                return;
            }

            if (url.Contains("poe.app")) {
                throw new NotImplementedException();
            }

            throw new ArgumentException($"Could not parse url '{url}'");
        }


        public static void Run() {
            foreach (var connection in Connections) {
                connection.Connect();
            }
        }

        public static void Stop() {
            foreach (var connection in Connections) {
                connection.Disconnect();
            }

            var allDone = false;
            while (!allDone) {
                foreach (var connection in Connections) {
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
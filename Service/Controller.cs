using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using Domain;
using Service.Connection;


namespace Service {
    public static class Controller {
        private static readonly List<BaseConnection> Connections = new List<BaseConnection>();
        private static readonly string ExecutionPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private static void AddConnections(ConnectionInfo[] connections) {
            RemoveConnections();
            
            foreach (var connection in connections) {
                switch (connection.Type) {
                    case ConnectionType.PoeTrade:
                        Connections.Add(new PtConnection(connection));
                        break;
                    case ConnectionType.PathOfExile:
                        Connections.Add(new PoeConnection(connection));
                        break;
                    case ConnectionType.PoeApp:
                        Connections.Add(new PaConnection(connection));
                        break;
                }
            }
        }

        public static void Run() {
            if (Connections.Count == 0) {
                Console.WriteLine("No connections defined");
                return;
            }

            if (Connections.Any(c => c.Info.Type.Equals(ConnectionType.PathOfExile))) {
                if (string.IsNullOrEmpty(BaseConnection.PoeSessionId)) {
                    Console.WriteLine("No POESESSID set");
                    return;
                }
            }

            try {
                foreach (var connection in Connections) {
                    connection.Connect();
                }
                
                Console.ReadKey(true);
            } finally {
                foreach (var connection in Connections) {
                    connection.DeleteSocket();
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

        public static void OpenInExplorer() {
            Process.Start("explorer.exe", ExecutionPath);
        }

        public static void Load() {
            var path = $@"{ExecutionPath}\poelive.txt";

            if (!File.Exists(path)) {
                Console.WriteLine($"Cannot find file:\n  - {path}");
                return;
            }

            var connections = LoadInfoFile(path);
            if (connections == null) {
                return;
            }
            
            Console.WriteLine($"Loaded {connections.Length} connections");
            AddConnections(connections);
        }

        private static ConnectionInfo[] LoadInfoFile(string path) {
            var regex = new Regex(@"(^.+$)?\n?(^https?:\/\/(.*?)\/.+$)", RegexOptions.Multiline);
            var text = File.ReadAllText(path, Encoding.ASCII)?.Trim();

            if (string.IsNullOrEmpty(text)) {
                Console.WriteLine("File is empty");
                return null;
            }

            var match = regex.Match(text);

            if (!match.Success) {
                Console.WriteLine("No urls found in file");
                return null;
            }

            var connections = new List<ConnectionInfo>();

            while (match.Success) {
                var info = new ConnectionInfo {
                    Title = string.IsNullOrEmpty(match.Groups[1].Value) ? null : match.Groups[1].Value,
                    Url = match.Groups[2].Value,
                    Type = null
                };

                if (PoeConnection.UrlRegex.IsMatch(match.Groups[2].Value)) {
                    info.Type = ConnectionType.PathOfExile;
                    connections.Add(info);
                } else if (PtConnection.UrlRegex.IsMatch(match.Groups[2].Value)) {
                    info.Type = ConnectionType.PoeTrade;
                    connections.Add(info);
                }

                match = match.NextMatch();
            }

            return connections.ToArray();
        }

        public static void PrintConnections() {
            if (Connections.Count == 0) {
                Console.WriteLine("Loaded: None");
                return;
            }

            foreach (var connection in Connections) {
                Console.WriteLine($"{connection.Info.Url}: ");
                Console.WriteLine($"  - Type: {connection.Info.Type.ToString() ?? "<Unknown>"}");
                Console.WriteLine($"  - Description: {connection.Info.Title ?? "<Unknown>"}");
            }
        }

        public static void RemoveConnections() {
            Connections.Clear();
        }
        
        public static void SetPoeSessId() {
            Console.WriteLine("Enter POESESSID:");
            Console.Write("> ");
            var id = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(id) || id.Length != 32) {
                Console.WriteLine("SessID not set");
                return;
            }
            
            Console.WriteLine("SessID set");
            BaseConnection.PoeSessionId = id;
        }
    }
}
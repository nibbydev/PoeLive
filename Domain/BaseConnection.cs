using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Domain {
    public abstract class BaseConnection {
        protected enum ConnectionType {
            PoeTrade,
            PathOfExile,
            PoeApp
        }

        protected static readonly HttpClient WebClient = new HttpClient();
        public static Action<string> RemoveActive { protected get; set; }
        public static Action<Item[]> DispatchItem { protected get; set; }

        protected WebSocket WebSocket;
        protected string WsUrl, Identifier;
        protected readonly ConnectionType Type;
        public static string PoeSessionId { private get; set; }

        static BaseConnection() {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "PoeLive 1.0");
            WebClient.DefaultRequestHeaders.Add("Accept", "*/*");
            WebClient.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        protected BaseConnection(ConnectionType type, string url) {
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentException("Empty or null search url passed");
            }

            Type = type;

            if (ConnectionType.PathOfExile.Equals(type) && PoeSessionId.IsNullOrEmpty()) {
                throw new ArgumentException("No POESESSID found");
            }
        }


        public void Connect() {
            if (WebSocket == null) {
                throw new Exception("Websocket has not been created");
            }

            WebSocket.Connect();
        }

        public void Disconnect() {
            DeleteSocket();
        }


        protected void CreateSocket(object e = null) {
            if (WsUrl == null) {
                throw new ArgumentException("Web socket url has not been created");
            }

            DeleteSocket();

            WebSocket = new WebSocket(WsUrl);
            WebSocket.OnMessage += SocketOnMessage;
            WebSocket.OnOpen += SocketOnOpen;
            WebSocket.OnClose += SocketOnClose;
            WebSocket.OnError += SocketOnClose;

            if (Type.Equals(ConnectionType.PathOfExile)) {
                PrintColorMsg(ConsoleColor.Magenta, "Connection", "Using POESESSID");
                WebSocket.SetCookie(new Cookie("POESESSID", PoeSessionId));
            }

            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket created");
        }

        public void DeleteSocket() {
            if (WebSocket == null) {
                return;
            }

            WebSocket.OnOpen -= SocketOnOpen;
            WebSocket.OnClose -= SocketOnClose;
            WebSocket.OnError -= SocketOnClose;
            WebSocket.OnMessage -= SocketOnMessage;

            WebSocket.Close();
            WebSocket = null;

            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket deleted");
        }

        public bool IsConnected() {
            return WebSocket != null && WebSocket.IsAlive;
        }


        protected virtual void SocketOnOpen(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket connected");
        }

        private void SocketOnClose(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket disconnected");

            // Wait a bit before recreating
            Task.Delay(1000).ContinueWith(t => CreateSocket());
        }


        public virtual void DispatchSearchAsync(string[] ids = null) {
            throw new NotImplementedException();
        }

        public virtual Task<string> AsyncRequest(string value) {
            throw new NotImplementedException();
        }

        protected abstract void SocketOnMessage(object sender, MessageEventArgs e);

        public virtual string BuildWebSocketUrl(string url) {
            throw new NotImplementedException();
        }


        protected static void DispatchDelete(string value) {
            RemoveActive?.Invoke(value);
        }

        protected void PrintColorMsg(ConsoleColor color, string a, string b = null) {
            if (string.IsNullOrEmpty(Identifier) || string.IsNullOrEmpty(a)) {
                throw new ArgumentException();
            }

            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write(Type);
            Console.ResetColor();
            Console.Write("][");
            Console.ForegroundColor = color;
            Console.Write(Identifier);
            Console.ResetColor();
            Console.Write("] ");

            Console.ResetColor();
            Console.Write(a);

            if (b == null) {
                Console.WriteLine();
                return;
            }

            Console.Write(": ");

            switch (color) {
                case ConsoleColor.Black:
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case ConsoleColor.Blue:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case ConsoleColor.Cyan:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case ConsoleColor.Gray:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ConsoleColor.Green:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case ConsoleColor.Magenta:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case ConsoleColor.Red:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case ConsoleColor.White:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ConsoleColor.Yellow:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    Console.ForegroundColor = color;
                    break;
            }

            Console.WriteLine(b);
            Console.ResetColor();
        }
    }
}
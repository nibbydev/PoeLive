using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Service.Connection {
    public abstract class BaseConnection {
        protected static readonly HttpClient WebClient = new HttpClient();
        public static Action<string> DispatchDelItem { protected get; set; }
        public static Action<Item> DispatchNewItem { protected get; set; }

        protected WebSocket WebSocket;
        protected string WsUrl, Identifier;
        public readonly ConnectionInfo Info;
        public static string PoeSessionId { get; set; }
        public static bool Verbose { private get; set; } = true;
        private bool _run;

        static BaseConnection() {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "PoeLive 1.0");
            WebClient.DefaultRequestHeaders.Add("Accept", "*/*");
            WebClient.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        protected BaseConnection(ConnectionInfo info) {
            if (string.IsNullOrEmpty(info.Url) || string.IsNullOrWhiteSpace(info.Url)) {
                throw new ArgumentException("Empty or null search url passed");
            }

            Info = info;

            /*if (ConnectionType.PathOfExile.Equals(info.Type) && PoeSessionId.IsNullOrEmpty()) {
                throw new ArgumentException("Cannot connect to pathofexile.com due to missing POESESSID cookie");
            }*/
        }


        public virtual void Connect() {
            if (WebSocket == null) {
                throw new Exception("Websocket has not been created");
            }

            _run = true;

            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Connecting");
            WebSocket.Connect();
        }


        protected void CreateSocket(object e = null) {
            if (WsUrl == null) {
                throw new ArgumentException("Web socket url has not been created");
            }

            WebSocket = new WebSocket(WsUrl);
            WebSocket.OnMessage += SocketOnMessage;
            WebSocket.OnOpen += SocketOnOpen;
            WebSocket.OnClose += SocketOnClose;
            WebSocket.OnError += SocketOnClose;
            
            // If url points to pathofexile.com, use provided POESESSID cookie
            if (ConnectionType.PathOfExile.Equals(Info.Type)) {
                WebSocket.SetCookie(new Cookie("POESESSID", PoeSessionId));
            }
        }

        public void DeleteSocket() {
            _run = false;
            
            if (WebSocket == null) {
                return;
            }

            WebSocket.OnOpen -= SocketOnOpen;
            WebSocket.OnClose -= SocketOnClose;
            WebSocket.OnError -= SocketOnClose;
            WebSocket.OnMessage -= SocketOnMessage;

            WebSocket.Close();
            WebSocket = null;
        }

        public bool IsConnected() {
            return WebSocket != null && WebSocket.IsAlive;
        }


        protected virtual void SocketOnOpen(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket opened");
        }

        private void SocketOnClose(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket closed/error");
            DeleteSocket();
            
            // Wait a bit before recreating
            Task.Delay(1000).ContinueWith(task => {
                if (!_run) {
                    return;
                }
                
                CreateSocket();
                Connect();
            });
        }


        public virtual void DispatchSearchAsync(string[] ids = null) {
            throw new NotImplementedException();
        }

        public virtual Task<string> AsyncRequest(string value) {
            throw new NotImplementedException();
        }

        protected abstract void SocketOnMessage(object sender, MessageEventArgs e);

        public virtual string BuildWebSocketUrl() {
            throw new NotImplementedException();
        }

        protected void PrintColorMsg(ConsoleColor color, string a, string b = null) {
            if (!Verbose) {
                return;
            }
            
            if (string.IsNullOrEmpty(Identifier) || string.IsNullOrEmpty(a)) {
                throw new ArgumentException();
            }

            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write(Info.Type);
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
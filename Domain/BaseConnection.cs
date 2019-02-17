using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Domain {
    public abstract class BaseConnection : IConnection {
        protected static readonly HttpClient WebClient = new HttpClient();
        public static Action<string> RemoveActive { protected get; set; }
        public static Action<Item[]> DispatchItem { protected get; set; }
        
        protected WebSocket WebSocket;
        protected string SearchUrl, WsUrl, Identifier;
        public static string PoeSessionId { private get; set; }

        static BaseConnection() {
            WebClient.DefaultRequestHeaders.Add("User-Agent", "PoeLive 1.0");
            WebClient.DefaultRequestHeaders.Add("Accept", "*/*");
            WebClient.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        protected BaseConnection(string searchUrl) {
            if (searchUrl.IsNullOrEmpty()) {
                throw new ArgumentException();
            }
            
            SearchUrl = searchUrl;
        }

        
        public void CreateSocket(object e = null) {
            if (WsUrl == null) {
                throw new NotImplementedException();
            }

            DeleteSocket();
            
            WebSocket = new WebSocket(WsUrl);
            WebSocket.OnMessage += SocketOnMessage;
            WebSocket.OnOpen += SocketOnOpen;
            WebSocket.OnClose += SocketOnClose;
            WebSocket.OnError += SocketOnClose;

            if (!string.IsNullOrEmpty(PoeSessionId)) {
                WebSocket.SetCookie(new Cookie("POESESSID", PoeSessionId));
            }
            
            WebSocket.Connect();
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
            
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket disposed");
        }

        public bool IsConnected() {
            return WebSocket != null && WebSocket.IsAlive;
        }



        public virtual void SocketOnOpen(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket connected");
        }

        public void SocketOnClose(object sender, EventArgs e) {
            PrintColorMsg(ConsoleColor.Magenta, "Connection", "Socket disconnected");
            
            // Wait a bit before recreating
            Task.Delay(1000).ContinueWith(t=> CreateSocket());
        }
        

        

        public abstract void DispatchSearchAsync(string[] ids = null);

        public abstract Task<string> AsyncRequest(string value);

        public abstract void SocketOnMessage(object sender, MessageEventArgs e);
        
        
        
        
        protected static void DispatchDelete(string value) {  
            RemoveActive?.Invoke(value);
        }

        protected void PrintColorMsg(ConsoleColor color, string a, string b = null) {
            if (string.IsNullOrEmpty(Identifier) || string.IsNullOrEmpty(a)) {
                throw new ArgumentException();
            }
            
            Console.Write("[");
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
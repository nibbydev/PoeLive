using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods;
using Domain;
using WebSocketSharp;

namespace Service.PoeTrade {
    public class Connection {
        private static readonly HttpClient WebClient = new HttpClient();
        public static Action<string> RemoveActive { private get; set; }
        public static Action<Item[]> DispatchItem { private get; set; }

        private const string VersionString = "{\"type\": \"version\", \"value\": 3}";
        private const string UserAgent = "Mozilla/5.0 (SMART-FRIDGE; TempleOS; Tizen 2.3) AppleWebkit/538.1 (KHTML, like Gecko) SamsungBrowser/1.0 Fridge Safari/538.1";
        
        private readonly string _search;
        private int _lastId;
        private WebSocket _webSocket;
        private Timer _heartBeat;
        private bool _gotInitialHeartBeat;

        static Connection() {
            WebClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            WebClient.DefaultRequestHeaders.Add("Accept", "*/*");
            WebClient.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        public Connection(string search) {
            _search = search;
            
            // Get initial id state
            _lastId = PostAsync(-1, _search).Result.ParseJSON<ApiDeserializer>().NewId;
            
            PrintSearch(ConsoleColor.Magenta);
            Console.Write("Got initial id: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(_lastId);
            Console.ResetColor();
            
            CreateSocket();
        }

        private void CreateSocket(object e = null) {
            DeleteSocket();
            
            _webSocket = new WebSocket($"ws://live.poe.trade/{_search}");
            _webSocket.OnMessage += SocketOnMessage;
            _webSocket.OnOpen += SocketOnOpen;
            _webSocket.OnClose += SocketOnClose;
            _webSocket.OnError += SocketOnClose;
            _webSocket.Connect();
            
            PrintSearch(ConsoleColor.Magenta);
            Console.Write("Connection: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Socket created");
            Console.ResetColor();
        }

        public void DeleteSocket() {
            _heartBeat?.Dispose();
            
            if (_webSocket == null) {
                return;
            }
            
            _webSocket.OnOpen -= SocketOnOpen;
            _webSocket.OnClose -= SocketOnClose;
            _webSocket.OnError -= SocketOnClose;
            _webSocket.OnMessage -= SocketOnMessage;
            
            _webSocket.Close();
            _webSocket = null;
            
            PrintSearch(ConsoleColor.Red);
            Console.Write("Connection: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Socket deleted");
            Console.ResetColor();
        }
        
        public bool IsConnected() {
            return _webSocket != null && _webSocket.IsAlive;
        }


        
        
        private void HeartBeatSend(object e = null) {
            // Only run heartbeat if connection has been established
            if (_gotInitialHeartBeat && _webSocket.IsAlive) {
                _webSocket?.Send("ping");
            }
        }
        
        private void HeartBeatReceive() {
            PrintSearch(ConsoleColor.Magenta);
            Console.Write("Heartbeat: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            
            if (_gotInitialHeartBeat) {
                Console.WriteLine("Recurring");
                Console.ResetColor();
                return;
            }
            
            Console.WriteLine("Initial");
            Console.ResetColor();
            
            _gotInitialHeartBeat = true;
            _heartBeat = new Timer(HeartBeatSend, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }
        
        private async void DispatchSearchAsync() {
            PrintSearch();
            Console.Write("Notify: ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(_lastId);
            Console.ResetColor();
            
            // Make POST request
            var jsonString = await PostAsync(_lastId, _search);
            var data = jsonString?.ParseJSON<ApiDeserializer>();

            if (jsonString == null) {
                PrintSearch(ConsoleColor.Red);
                Console.Write("Invalid reply for: ");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(_lastId);
                Console.ResetColor();
                return;
            }
            
            _lastId = data.NewId;
            
            // No data
            if (data.Data == null) {
                PrintSearch();
                Console.Write("POST status: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No data");
                Console.ResetColor();
                return;
            }
            
            // Subscribe
            foreach (var uniq in data.Uniqs) {
                var payload = new {type = "subscribe", value = uniq};
                _webSocket.Send(payload.ToJSON());
            }
            
            // Parse item data
            var items = HtmlParser.ParsePoeTrade(data.Data);

            // Give each item its uniq
            for (var i = 0; i < data.Count; i++) {
                items[i].Uniq = data.Uniqs[i];
            }
            
            DispatchItem?.Invoke(items);
        }

        private void DispatchDelete(string value) {
            PrintSearch(ConsoleColor.DarkGray);
            Console.Write("Delete: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(_lastId);
            Console.ResetColor();
            
            RemoveActive?.Invoke(value);
        }


        
        
        
        private void SocketOnMessage(object sender, MessageEventArgs e) {
            /*PrintSearch();
            Console.Write("Got WS: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(e.Data);
            Console.ResetColor();*/

            // Reply was just the id
            if (int.TryParse(e.Data, out var id)) {
                _lastId = id;
                return;
            }

            var msg = e.Data.ParseJSON<WsDeserializer>();
            switch (msg.Type) {
                case "pong":
                    HeartBeatReceive();
                    break;

                case "notify":
                    DispatchSearchAsync();
                    break;
                
                case "del":
                    DispatchDelete(msg.Value);
                    break;
                
                default:
                    PrintSearch();
                    Console.WriteLine("Unknown type: {0}", e.Data);
                    break;
            }
        }

        private void SocketOnOpen(object sender, EventArgs e) {
            _webSocket.Send(VersionString);
            _webSocket?.Send("ping");
        }

        private void SocketOnClose(object sender, EventArgs e) {
            PrintSearch(ConsoleColor.Red);
            Console.Write("Connection: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Socket disconnected");
            Console.ResetColor();

            // Wait a bit before recreating
            Task.Delay(1000).ContinueWith(t=> CreateSocket());
        }



        private static async Task<string> PostAsync(int id, string search) {
            var payload = new Dictionary<string, string> {{"id", id.ToString()}};
            var content = new FormUrlEncodedContent(payload);
            HttpResponseMessage response;
            
            try {
                response = await WebClient.PostAsync($"http://poe.trade/search/{search}/live", content);
            } catch {
                return null;
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        private void PrintSearch(ConsoleColor color = ConsoleColor.Blue) {
            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write(_search);
            Console.ResetColor();
            Console.Write("] ");
        }
    }
}
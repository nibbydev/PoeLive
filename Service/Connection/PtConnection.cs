using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods;
using Domain;
using Domain.PoeTrade;
using Domain.PoeTrade.ApiDeserializer;
using WebSocketSharp;

namespace Service.Connection {
    public sealed class PtConnection : BaseConnection {
        public static readonly Regex UrlRegex = new Regex(@"^https?:\/\/poe\.trade\/search\/([a-zA-Z]+)\/?$");
        private int _lastId;

        public PtConnection(ConnectionInfo info) : base(info) {
            WsUrl = BuildWebSocketUrl();
        }

        public override void Connect() {
            CreateSocket();
            _lastId = AsyncRequest("-1").Result.ParseJSON<ApiDeserializer>().NewId;
            base.Connect();
        }

        public override string BuildWebSocketUrl() {
            // http://poe.trade/search/omisokonausiha
            var match = UrlRegex.Match(Info.Url);

            if (!match.Success || match.Groups.Count != 2) {
                throw new ArgumentException("Could not parse url");
            }

            Identifier = match.Groups[1].Value;
            return $"ws://live.poe.trade/{Identifier}";
        }

        protected override void SocketOnOpen(object sender, EventArgs e) {
            base.SocketOnOpen(sender, e);
            WebSocket?.Send("{\"type\": \"version\", \"value\": 3}");
        }

        public override async void DispatchSearchAsync(string[] ids = null) {
            if (ids != null) {
                throw new ArgumentException();
            }

            PrintColorMsg(ConsoleColor.Blue, "Notify", _lastId.ToString());

            // Make POST request
            var jsonString = await AsyncRequest(_lastId.ToString());
            var data = jsonString?.ParseJSON<ApiDeserializer>();

            if (jsonString == null) {
                PrintColorMsg(ConsoleColor.Blue, "Invalid reply", _lastId.ToString());
                return;
            }

            _lastId = data.NewId;

            // Subscribe
            foreach (var uniq in data.Uniqs) {
                var payload = new {type = "subscribe", value = uniq};
                WebSocket.Send(payload.ToJSON());
            }

            // Parse item data
            var items = HtmlParser.ParsePoeTrade(data.Data);

            // Give each item its uniq
            for (var i = 0; i < data.Count; i++) {
                items[i].Uniq = data.Uniqs[i];
            }
            
            items.ForEach(t => DispatchNewItem?.Invoke(Converter.ConvertPt(t)));
        }

        protected override void SocketOnMessage(object sender, MessageEventArgs e) {
            // Reply was just the id
            if (int.TryParse(e.Data, out var id)) {
                _lastId = id;
                return;
            }

            var msg = e.Data.ParseJSON<WsDeserializer>();
            switch (msg.Type) {
                case "notify":
                    DispatchSearchAsync();
                    break;

                case "del":
                    DispatchDelItem?.Invoke(msg.Value);
                    break;

                default:
                    PrintColorMsg(ConsoleColor.Red, "Unknown type", e.Data);
                    break;
            }
        }

        public override async Task<string> AsyncRequest(string value) {
            var payload = new Dictionary<string, string> {{"id", value}};
            var content = new FormUrlEncodedContent(payload);

            try {
                var response = await WebClient.PostAsync($"http://poe.trade/search/{Identifier}/live", content);
                return await response.Content.ReadAsStringAsync();
            } catch {
                return null;
            }
        }
    }
}
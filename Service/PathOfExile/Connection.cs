using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using Domain;
using Domain.PathOfExile;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Service.PathOfExile {
    public class Connection : BaseConnection {
        private string _league;

        public Connection(string search) : base(search) {
            SearchUrl = search;
            WsUrl = BuildWebSocketUrl(SearchUrl);

            CreateSocket();
        }

        private string BuildWebSocketUrl(string data = null) {
            //throw new NotImplementedException();

            // todo this

            _league = "Betrayal";
            Identifier = data;

            return $"wss://www.pathofexile.com/api/trade/live/{_league}/{Identifier}";
        }


        public override async void DispatchSearchAsync(string[] ids = null) {
            if (ids == null) {
                throw new ArgumentException();
            }

            for (var i = 0; i <= ids.Length / 10; i++) {
                var subIds = ids.Skip(i * 10).Take(10);
                var concatIds = string.Join(",", subIds);

                PrintColorMsg(ConsoleColor.Blue, "New", concatIds);

                // Make POST request
                var jsonString = await AsyncRequest(concatIds);
                var data = jsonString?.ParseJSON<ApiDeserializer>();

                // todo: convert to domain object
                var domainItems = new Domain.Item[data?.result.Length ?? 0];
                DispatchItem?.Invoke(domainItems);

                throw new NotImplementedException();
            }
        }

        public override void SocketOnMessage(object sender, MessageEventArgs e) {
            var msg = e.Data.ParseJSON<WsDeserializer>();

            if (!msg.@new.IsNullOrEmpty()) {
                msg.@new.ForEach(t => RemoveActive?.Invoke(t));
                DispatchSearchAsync(msg.@new);
            }
        }

        public override async Task<string> AsyncRequest(string concatIds) {
            var url = $"https://www.pathofexile.com/api/trade/fetch/{concatIds}?query={Identifier}";

            try {
                var response = await WebClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            } catch {
                return null;
            }
        }
    }
}
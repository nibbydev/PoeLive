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
    public sealed class PaConnection : BaseConnection {
        public static readonly Regex UrlRegex = new Regex(@"^https?:\/\/poeapp\.com\/#\/search\/([^\/]+)\/?$");
        private const bool HashApiImplemented = false;
        public static string HashApiUrl;
        private const string WsVersion = "2";
        
        private string _urlParam, _query, _hash;

        public PaConnection(ConnectionInfo info) : base(info) {
        }

        public override void Connect() {
            if (string.IsNullOrEmpty(HashApiUrl) || !HashApiImplemented) {
                throw new NotImplementedException("No valid hash api url found");
            }
            
            WsUrl = BuildWebSocketUrl();
            
            CreateSocket();
            base.Connect();
        }

        public override string BuildWebSocketUrl() {
            // https://poeapp.com/#/search/N4IghgLhBOCWBGBXCBTAzgOgDYrAc0RRAC4R4UYwBPMLEAGhFgDsAzAewwnYGsVnYALxQATDK0RYsAOTABbIqQCiAD1qoRAAgDy0eCAC+QA/
            var match = UrlRegex.Match(Info.Url);

            if (!match.Success || match.Groups.Count != 2) {
                throw new ArgumentException("Could not parse url");
            }

            _urlParam = match.Groups[1].Value;
            _query = LZStringCSharp.LZString.DecompressFromEncodedURIComponent(_urlParam);
            _hash = Identifier = AsyncRequestHash(_query).Result;

            return $"wss://poeapp.com/ws?{WsVersion}-{_hash}";
        }
        
        private static async Task<string> AsyncRequestHash(string json) {
            try {
                var response = await WebClient.GetAsync(HashApiUrl + json);
                return await response.Content.ReadAsStringAsync();
            } catch {
                return null;
            }
        }

        protected override void SocketOnOpen(object sender, EventArgs e) {
            base.SocketOnOpen(sender, e);
            
            var encodedQuery = LZStringCSharp.LZString.CompressToUTF16(_query);
            WebSocket?.Send(encodedQuery);
        }
        
        protected override void SocketOnMessage(object sender, MessageEventArgs e) {
            var decodedMsg = LZStringCSharp.LZString.DecompressFromUTF16(e.Data);
            PrintColorMsg(ConsoleColor.Blue, "Msg", decodedMsg);

            try {
                throw new NotImplementedException();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Domain {
    public interface IConnection {
        void CreateSocket(object e = null);

        void DeleteSocket();

        bool IsConnected();
        
        
        void SocketOnMessage(object sender, MessageEventArgs e);

        void SocketOnOpen(object sender, EventArgs e);

        void SocketOnClose(object sender, EventArgs e);
        
        
        void DispatchSearchAsync(string[] ids = null);

        Task<string> AsyncRequest(string value);
    }
}
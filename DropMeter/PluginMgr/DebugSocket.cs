using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropMeter.PluginInterface;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DropMeter.PluginMgr
{

    public struct DebugMessageCapsule
    {

        public string pluginId;
        public PluginMessage message;

    }
    class DebugSocket : IDisposable
    {
        public static DebugSocket instance = new DebugSocket();
        public WebSocketServer wssv;
        public class DebugSocketWS : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                var msg = e.Data == "BALUS"
                    ? "I've been balused already..."
                    : "I'm not available now.";

                Send(msg);
            }
        }
        public DebugSocket()
        {
            wssv = new WebSocketServer("ws://localhost:9007");
            wssv.AddWebSocketService<DebugSocketWS>("/pluginchannel");
            
            wssv.Start();
            
        }

        public void Dispose()
        {
            wssv.Stop();
        }
    }
}

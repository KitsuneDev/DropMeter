using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropMeter.PluginInterface;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DropMeter.WebNowPlaying
{
    class WebNowPlayingReceiver : WebSocketBehavior
    {
        #region Operations
        public async void PLAYPAUSE()
        {
            this.Send("PLAYPAUSE");

        }
        public async void NEXT()
        {
            this.Send("NEXT");

        }
        public async void PREVIOUS()
        {
            this.Send("PREVIOUS");

        }
        public async void SETPOSITION(int Position)
        {
            this.Send("SETPOSITION " + Position);

        }
        public async void SETVOLUME(int Volume)
        {
            this.Send("SETVOLUME "+ Volume);

        }
        public async void REPEAT()
        {
            this.Send("REPEAT");

        }
        public async void SHUFFLE()
        {
            this.Send("SHUFFLE");

        }
        public async void RATING(int Stars)
        {
            this.Send("RATING "+Stars);

        }



        #endregion
        protected override void OnMessage(MessageEventArgs e)
        {


            var msg = e.Data;
            Console.WriteLine(e.Data);
            var datas = msg.Split(':');
            WebNowPlaying.helper.EmitMessage(datas[0], datas.Skip(1).ToArray());
            Send(msg);
        
    }
    }
public class WebNowPlaying : DMPlugin
{


        WebNowPlayingReceiver LastPlayer;
        internal static IPluginHelper helper;
        public string DisplayName
        {
            get => "WebNowPlaying";

        }

        public string Slug
        {
            get => "webnowplaying";

        }

        private WebSocketServer wssv;
        public bool Initialize(IPluginHelper _helper)
        {
            helper = _helper; 
            wssv = new WebSocketServer("ws://127.0.0.1:8974/");
            wssv.AddWebSocketService<WebNowPlayingReceiver>("/");
            wssv.Start();
            
            helper.OnMessageReceived += (message, data) =>
            {

            };
            return true;
        }

        public void Terminate()
        {
            if (wssv != null) wssv.Stop();
        }
    }
}

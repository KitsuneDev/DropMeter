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
        public void PLAYPAUSE()
        {
            this.Send("PLAYPAUSE");

        }
        public void NEXT()
        {
            this.Send("NEXT");

        }
        public void PREVIOUS()
        {
            this.Send("PREVIOUS");

        }
        public void SETPOSITION(int Position)
        {
            this.Send("SETPOSITION " + Position);

        }
        public void SETVOLUME(int Volume)
        {
            this.Send("SETVOLUME "+ Volume);

        }
        public void REPEAT()
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
            WebNowPlaying.helper.BroadcastMessage(datas[0], string.Join(":", datas.Skip(1)));
            if(WebNowPlaying.RainmeterConnector.ReadyState == WebSocketState.Open)
                try
                {
                    WebNowPlaying.RainmeterConnector.Send(e.Data);
                }
                catch
                {
                    //TODO: Log Rainmeter Send Error
                
                }

            Send(msg);
        
    }
    }
public class WebNowPlaying : DMPlugin
{


        WebNowPlayingReceiver LastPlayer;
        internal static WebSocket RainmeterConnector = new WebSocket("ws://127.0.0.1:8976/");
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
                //Pass it to the Player
                wssv.WebSocketServices["/"].Sessions.Broadcast(message);
            };
            Task task = Task.Run(ConnectRainmeterCompat);
            
            return true;
        }

        public async Task<bool> ConnectRainmeterCompat()
        {
            int RainMeterTries = 0;
            while (RainMeterTries <= 10)
            {
                try
                {
                    RainmeterConnector.Connect();

                    return true;
                }
                catch
                {
                    await Task.Delay(2000);
                    RainMeterTries = RainMeterTries + 1;
                }
            }
            WebNowPlaying.helper.logger.Warn("Unable to detect Rainmeter Compat.");
            return false;
        }
        public void Terminate()
        {
            RainmeterConnector.Close();
            wssv?.Stop();
        }
    }
}

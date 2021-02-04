using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using DropMeter.PluginInterface;

namespace DropMeter.CEF
{
    class JSComContextHelper
    {
        public static ObjectRepository<string, JSComContext> instances = new ObjectRepository<string, JSComContext>();

    }
    class JSComContext
    {
        
        
        public void SendToPlugin()
        {
            // Do something really cool here.
        }

        internal ObjectRepository<string, Dictionary<string, IJavascriptCallback>> EventHandlers = new ObjectRepository<string, Dictionary<string, IJavascriptCallback>>();

        internal async void TransmitEvent(string pluginSlug, string messageID, params object[] data)
        {
            try
            {
                await EventHandlers[pluginSlug][messageID].ExecuteAsync(data);
            }
            catch (KeyNotFoundException)
            {

            }
        }

        public void RegisterCallback(string pluginSlug, string messageID, IJavascriptCallback javascriptCallback)
        {
            EventHandlers[pluginSlug][messageID] = javascriptCallback;
            /*const int taskDelay = 1500;

            Task.Run(async () =>
            {
                await Task.Delay(taskDelay);

                using (javascriptCallback)
                {
                    //NOTE: Classes are not supported, simple structs are
                    var response = new CallbackResponseStruct("This callback from C# was delayed " + taskDelay + "ms");
                    await javascriptCallback.ExecuteAsync(response);
                }
            });*/
        }

    }
}

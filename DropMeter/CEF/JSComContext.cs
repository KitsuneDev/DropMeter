using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace DropMeter.CEF
{
    class JSComContext
    {
        public string MyProperty { get; set; }
        public void InvokeCallbackOnBrowser()
        {
            // Do something really cool here.
        }

        private Dictionary<string, Dictionary<string, IJavascriptCallback>> EventHandlers = new Dictionary<string, Dictionary<string, IJavascriptCallback>>();

        internal async void TransmitEvent(string pluginSlug, string messageID, params object[] data)
        {
            await EventHandlers[pluginSlug][messageID].ExecuteAsync(data);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using DropMeter.CEF;
using DropMeter.PluginInterface;
using DropMeter.PluginMgr;
using Newtonsoft.Json;

namespace DropMeter
{
    public class PluginHelper : IPluginHelper
    {
        private DMPlugin Plugin;
        internal PluginHelper(DMPlugin Plugin)
        {
            this.Plugin = Plugin;
        }
        public void BroadcastMessage(string id, object parameters)
        {
            var data = new PluginMessage()
            {
                messageID = id,
                data = parameters
            };

            var sessions = DebugSocket.instance.wssv.WebSocketServices["/pluginchannel"].Sessions;
            sessions.Broadcast(JsonConvert.SerializeObject(new DebugMessageCapsule()
            {
                message = data,
                pluginId = Plugin.Slug
            }));
            //TODO
            foreach(var widget in JSComContextHelper.instances.keyValuePairs)
            {
                widget.Value.TransmitEvent(Plugin.Slug, data);
            }
        }

        public void SendToView(string viewId, string id, object parameters)
        {
            JSComContextHelper.instances[viewId].TransmitEvent(Plugin.Slug, new PluginMessage()
            {
                messageID = id,
                data = parameters
            });
        }

        public event MessageHandler<IMessageReceivedData> OnMessageReceived;
    }

    class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.FullName == typeof(DMPlugin).Assembly.FullName)
            {
                return null;
            }
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }

    public class PluginLoader
    {
        public static List<DMPlugin> Plugins { get; set; }
        public static List<Assembly> PluginASM { get; private set; }
        static IEnumerable<DMPlugin> InitializePlugin(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                var pluginFQDN = type.Module.FullyQualifiedName;
                var modelFQDN = typeof(DMPlugin).Module.FullyQualifiedName;
                Console.WriteLine($"Plugin Injector: {pluginFQDN}/{modelFQDN}");
                if (typeof(DMPlugin).IsAssignableFrom(type))
                {
                    DMPlugin result = Activator.CreateInstance(type) as DMPlugin;
                    if (result != null)
                    {
                        result.Initialize(new PluginHelper(result));
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                /*string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements DMPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");*/
            }
        }

        public static void LoadPlugins()
        {
            Plugins = new List<DMPlugin>();
            PluginASM = new List<Assembly>();
            //Load the DLLs from the Plugins directory
            if (Directory.Exists(App.PluginBase))
            {
                var loadContext = new PluginLoadContext(App.PluginBase);
                string[] files = Directory.GetFiles(App.PluginBase);
                foreach (string file in files)
                {
                    if (file.EndsWith(".dll"))
                    {
                        PluginASM.Add(loadContext.LoadFromAssemblyPath(file));
                        

                    }
                }
            }

            
        }

        public static void InitializePlugins()
        {
            foreach (var asm in PluginASM)
            {
                Plugins.AddRange(InitializePlugin(asm));
            }
        }
    }
}

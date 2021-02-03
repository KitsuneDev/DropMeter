﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DropMeter.PluginInterface;

namespace DropMeter
{
    public class PluginHelper : IPluginHelper
    {
        private DMPlugin Plugin;
        internal PluginHelper(DMPlugin Plugin)
        {
            this.Plugin = Plugin;
        }
        public void EmitMessage(string id, object parameters)
        {
            //TODO
        }
        
        public event MessageHandler<IMessageReceivedData> OnMessageReceived;
    }
    public class PluginLoader
    {
        public static List<DMPlugin> Plugins { get; set; }

        public static void LoadPlugins()
        {
            Plugins = new List<DMPlugin>();

            //Load the DLLs from the Plugins directory
            if (Directory.Exists("plugins"))
            {
                string[] files = Directory.GetFiles("plugins");
                foreach (string file in files)
                {
                    if (file.EndsWith(".dll"))
                    {
                        Assembly.LoadFile(Path.GetFullPath(file));
                    }
                }
            }

            Type interfaceType = typeof(DMPlugin);
            //Fetch all types that implement the interface IPlugin and are a class
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
                .ToArray();
            foreach (Type type in types)
            {
                //Create a new instance of all found types
                Plugins.Add((DMPlugin)Activator.CreateInstance(type));
            }
        }

        public static void InitializePlugins()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Initialize(new PluginHelper(plugin));
            }
            Console.WriteLine("Loaded all plugins.");
        }
    }
}

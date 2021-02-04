using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using DropMeter.CEF;

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();
            PluginLoader.LoadPlugins();
            PluginLoader.InitializePlugins();

            var settings = new CefSettings
            {
                //BrowserSubprocessPath = GetCefExecutablePath()
            };
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = LocalFileHandlerFactory.SchemeName,
                SchemeHandlerFactory = new LocalFileHandlerFactory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "widgets")) //TODO
            });

            Cef.Initialize(settings);

            application.Run();
            
        }
    }
}

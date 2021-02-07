using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using DropMeter.CEF;
using DropMeter.PluginMgr;

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Dictionary<string, HTMLWidget> OpenWidgets = new Dictionary<string, HTMLWidget>();
        internal static List<string> AvailableWidgets = new List<string>();
        [STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();
            if (!Directory.Exists(HTMLWidget.DATAPATH)) Directory.CreateDirectory(HTMLWidget.DATAPATH);
            PluginLoader.LoadPlugins();
            PluginLoader.InitializePlugins();
            if(DebugSocket.instance == null) DebugSocket.instance = new DebugSocket();
            var settings = new CefSettings
            {
                //BrowserSubprocessPath = GetCefExecutablePath()
                CachePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"),
                CefCommandLineArgs =
                {
                    {"disable-web-security", "true"}
                }
            };
            settings.RemoteDebuggingPort = 8088;
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = LocalFileHandlerFactory.SchemeName,
                SchemeHandlerFactory = new LocalFileHandlerFactory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "widgets")), //TODO
                IsCorsEnabled = true,
                IsSecure = true,
                IsCSPBypassing = true,
                
                
            });

            Cef.Initialize(settings);
            
            application.Run();
            
        }


        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            var widgetDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "widgets");
            foreach (var widget in Directory.EnumerateDirectories(widgetDir))
            {
                if (File.Exists(Path.Combine(widget, "index.html")))
                {
                    var wn = new DirectoryInfo(widget).Name;
                    AvailableWidgets.Add(wn);
                    var view = new HTMLWidget(wn, true);
                    view.Show();
                    OpenWidgets.Add(wn, view);
                }
                else
                {
                    Console.WriteLine($"Widget at {widget} has no Entrypoint! Skipping.");
                }
            }

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = DropMeter.Properties.Resources.Icon;
            _notifyIcon.Visible = true;

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
                new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Manage DropMeter").Click += (s, e) => ShowMainWindow();
            foreach (var widget in OpenWidgets)
            {
                _notifyIcon.ContextMenuStrip.Items.Add("Manage: " + widget.Key).Click +=
                    (s, e) => widget.Value.EnterMoveMode();
            }

            _notifyIcon.ContextMenuStrip.Items.Add("Shutdown").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
            Current.Shutdown(0);
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
    }
}


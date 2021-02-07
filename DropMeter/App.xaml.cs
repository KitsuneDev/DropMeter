using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using CefSharp;
using CefSharp.Wpf;
using DropMeter.CEF;
using DropMeter.FileHandler;
using DropMeter.PluginMgr;
using Westwind.Utilities;
using Application = System.Windows.Application;

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Dictionary<string, HTMLWidget> OpenWidgets = new Dictionary<string, HTMLWidget>();
        internal static List<string> AvailableWidgets = new List<string>();
        internal DMFileHandler fileHandler;
        internal static string WidgetsBase = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "widgets");

        public Mutex Mutex;
        [STAThread]
        public static void Main()
        {

            var application = new App();
            application.InitializeComponent();
            application.Run();
            
        }


        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;
        public NamedPipeManager namedPipe = new NamedPipeManager("DropMeter");


        public App()
        {
            SingleInstanceCheck();
            
            if (!Directory.Exists(HTMLWidget.DATAPATH)) Directory.CreateDirectory(HTMLWidget.DATAPATH);
            PluginLoader.LoadPlugins();
            PluginLoader.InitializePlugins();
            if (DebugSocket.instance == null) DebugSocket.instance = new DebugSocket();
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
                SchemeHandlerFactory = new LocalFileHandlerFactory(WidgetsBase), //TODO
                IsCorsEnabled = true,
                IsSecure = true,
                IsCSPBypassing = true,


            });

            Cef.Initialize(settings);
        }

        public void LoadWidgets()
        {
            foreach (var widget in Directory.EnumerateDirectories(WidgetsBase))
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
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            
            LoadWidgets();

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
            _notifyIcon.ContextMenuStrip.Items.Add("-");
            if (OpenWidgets.Count == 0)
            {
                _notifyIcon.ContextMenuStrip.Items.Add("No Widgets Loaded");
            }
            foreach (var widget in OpenWidgets)
            {
                ToolStripMenuItem menu;
                //_notifyIcon.ContextMenuStrip.Items.Add("Manage: " + widget.Key).Click += (s, e) => widget.Value.EnterMoveMode();
                var iconPath = Path.Combine(WidgetsBase, widget.Key, "favicon.ico");
                var menuTitle = $"Widget: {widget.Key}";
                if (File.Exists(iconPath))
                {
                    Icon icoFile = new Icon(iconPath);
                    Image favicon = icoFile.ToBitmap();

                    menu = new ToolStripMenuItem(menuTitle, favicon);
                }
                else menu = new ToolStripMenuItem(menuTitle);
                menu.DropDown = new ToolStripDropDown();

                menu.DropDown.Items.Add("Enter Management Mode").Click += (s, e) => widget.Value.EnterMoveMode();
                menu.DropDown.Items.Add("Open DevTools").Click += (sender, args) => widget.Value.WebView.ShowDevTools();
                menu.DropDown.Items.Add("Unload").Click += (s, e) => widget.Value.Close();


                _notifyIcon.ContextMenuStrip.Items.Add(menu);


            }
            _notifyIcon.ContextMenuStrip.Items.Add("-");
            _notifyIcon.ContextMenuStrip.Items.Add("Reload Widgets").Click += (s, e) =>
            {
                foreach (var widget in OpenWidgets)
                {
                    widget.Value.Close();
                    
                }
                OpenWidgets = new Dictionary<string, HTMLWidget>();
                LoadWidgets();
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Shutdown").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            
            _isExit = true;
            MainWindow?.Close();
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
                MainWindow?.Hide(); // A hidden window can be shown again, a closed one not
            }
        }

        public void SingleInstanceCheck()
        {

            bool isOnlyInstance = false;
            this.Mutex = new Mutex(true, @"DropMeter", out isOnlyInstance);
            if (!isOnlyInstance)
            {
                string filesToOpen = " ";
                var args = Environment.GetCommandLineArgs();
                if (args != null && args.Length > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 1; i < args.Length; i++)
                    {
                        sb.AppendLine(args[i]);
                    }
                    filesToOpen = sb.ToString();
                }

                
                namedPipe.Write(filesToOpen);

                // this exits the application                    
                Environment.Exit(0);

            }
            else
            {
                namedPipe.StartServer();
                namedPipe.ReceiveString += OpenFileFromInstance;
            }
        }

        private void OpenFileFromInstance(string filesToOpen)
        {
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(filesToOpen))
                {
                    
                    foreach (var file in filesToOpen.GetLines())
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            if(fileHandler == null) fileHandler = new DMFileHandler(this);
                            fileHandler.OpenFile(file);
                        }
                            
                    }
                    //if (lastTab != null)
                    //    Dispatcher.InvokeAsync(() => TabControl.SelectedItem = lastTab);
                }

                /*
                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;

                this.Topmost = true;
                this.Activate();
                Dispatcher.BeginInvoke(new Action(() => { this.Topmost = false; }));
            */
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            namedPipe.StopServer();
            base.OnExit(e);
        }
    }

    
    
    

}


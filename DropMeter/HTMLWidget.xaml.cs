using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Internals;
using CefSharp.JavascriptBinding;
using CefSharp.Wpf;
using DropMeter.CEF;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DropMeter
{
    internal class WidgetDataStore
    {
        [JsonProperty]
        internal double TopPosition;
        [JsonProperty]
        internal double LeftPosition;
        [JsonProperty]
        internal double Height;
        [JsonProperty]
        internal double Width;
    }
    /// <summary>
    /// Interaction logic for HTMLWidget.xaml
    /// </summary>
    public partial class HTMLWidget : Window
    {
        public string WidgetName;
        /************ win32 interop stuff ****************/
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);


        private bool attachToDesktop = true;

        const int GWL_HWNDPARENT = -8;
        
        internal static string DATAPATH = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ldata");
        internal string LDataPath;
        public HTMLWidget(string widgetName, bool attachToDesktop = true)
        {
            

            this.attachToDesktop = attachToDesktop;
            
            InitializeComponent();
            LDataPath = Path.Combine(DATAPATH, widgetName + ".json");
            if (File.Exists(LDataPath))
            {

                WidgetDataStore data = JsonConvert.DeserializeObject<WidgetDataStore>(File.ReadAllText(LDataPath));
                this.Top = data.TopPosition;
                this.Left = data.LeftPosition;
                this.Width = data.Width;
                this.Height = data.Height;

            }
            //BrowserSettings settings = new BrowserSettings();
            //settings.WebSecurity = CefState.Disabled;
            //settings.FileAccessFromFileUrls = CefState.Enabled;
            //settings.UniversalAccessFromFileUrls = CefState.Enabled;
            //WebView.BrowserSettings = settings;
            var mmx = Cef.AddCrossOriginWhitelistEntry("widgets://test", "https", "musixmatch.com", true);
            WebView.MenuHandler = new CloseMenuHandler(this);
            WebView.DragHandler = new DragDropHandler();
            this.WidgetName = widgetName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Desktop Widget Inner Gears
            if (attachToDesktop)
            {
                

                

                
                //Init();

                // The window is set to the parent, and the parent window of the background window is set to the Program Manager window.
                IntPtr hwnd2 = new WindowInteropHelper(this).Handle;
                //W32.SetParent(hwnd2, programIntPtr);


                //IntPtr pWnd = FindWindow("Progman", null);
                //pWnd = FindWindowEx(pWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                //pWnd = FindWindowEx(pWnd, IntPtr.Zero, "SysListView32", null);
                //IntPtr tWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                //SetParent(tWnd, pWnd);
                W32.EnumWindows((hwnd, lParam) =>
                {
                    var shell = W32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", (IntPtr) null);
                    // Find the WorkerW that contains the SHELLDLL_DefView window handle
                    if (shell != IntPtr.Zero)
                    {
                        //hwnd is WorkerW
                        W32.SetParent(hwnd2, shell);
                        // Find the current WorkerW window, the next WorkerW window. 
                        //IntPtr tempHwnd = W32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", (IntPtr)null);

                        // hide this window
                        //W32.ShowWindow(tempHwnd, 0);
                    }

                    return true;
                }, IntPtr.Zero);
            }
            #endregion

            
            EnterWidgetMode(null, null);
            
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void WebView_IsBrowserInitializedChanged(object _, DependencyPropertyChangedEventArgs dep)
        {
            
            this.WebView.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            {
                var repo = e.ObjectRepository;
                if (e.ObjectName == "DropMeter")
                {
                    BindingOptions bindingOptions = null; //Binding options is an optional param, defaults to null
                    bindingOptions = BindingOptions.DefaultBinder; //Use the default binder to serialize values into complex objects

                    //bindingOptions = new BindingOptions { Binder = new MyCustomBinder() }); //Specify a custom binder
                    repo.NameConverter = null; //No CamelCase of Javascript Names
                    //For backwards compatability reasons the default NameConverter doesn't change the case of the objects returned from methods calls.
                    //https://github.com/cefsharp/CefSharp/issues/2442
                    //Use the new name converter to bound object method names and property names of returned objects converted to camelCase
                    repo.NameConverter = new CamelCaseJavascriptNameConverter();
                    repo.Register("DropMeter", JSComContextHelper.instances[WidgetName], isAsync: true, options: bindingOptions);
                }
            };
            this.WebView.Address = $"{LocalFileHandlerFactory.SchemeName}://{WidgetName}/index.html";
            /*this.WebView.LoadHtml(@"<!DOCTYPE html>
<html>
<body>

<h1>My First Heading</h1>
<p>My first paragraph.</p>

<style>
    html, body {
    background: transparent;
    color: white;
    -webkit-app-region: drag;
    
}
</style>
</body>
</html>", "http://demoplugin.int/");*/

            //this.WebView.ShowDevTools();

        }

        private void EnterWidgetMode(object sender, RoutedEventArgs e)
        {
            // Sender is null if not invoked by user
            if (sender != null)
            {
                Console.WriteLine("Saving Widget Data...");
                var conf = new WidgetDataStore()
                {
                    Height = this.Height,
                    Width = this.Width,
                    TopPosition = this.Top,
                    LeftPosition = this.Left,
                };
                var encoded = JsonConvert.SerializeObject(conf);
                File.WriteAllText(LDataPath, encoded);
            }

            
            WidgetMove.Visibility = Visibility.Hidden;
            WebView.Visibility = Visibility.Visible;
        }

        public void EnterMoveMode()
        {
            Dispatcher.Invoke(() =>
            {
                WidgetMove.Visibility = Visibility.Visible;
                WebView.Visibility = Visibility.Hidden;
            });
        }

        private void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            if (args.Frame.IsMain)
            {
                args.Frame.ExecuteJavaScriptAsync("CefSharp.BindObjectAsync(\"DropMeter\");");
            }
        }
    }
}

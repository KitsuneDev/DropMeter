using System;
using System.Collections.Generic;
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

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for HTMLWidget.xaml
    /// </summary>
    public partial class HTMLWidget : Window
    {
        public string PluginName;
        /************ win32 interop stuff ****************/
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpWindowClass, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);


        private bool attachToDesktop = true;

        const int GWL_HWNDPARENT = -8;

        public HTMLWidget(string PluginName, bool attachToDesktop = true)
        {
            this.attachToDesktop = attachToDesktop;
            InitializeComponent();
            WebView.MenuHandler = new CloseMenuHandler(this);
            WebView.DragHandler = new DragDropHandler();
            this.PluginName = PluginName;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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
            var settings = new CefSettings
            {
                //BrowserSubprocessPath = GetCefExecutablePath()
            };
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = LocalFileHandlerFactory.SchemeName,
                SchemeHandlerFactory = new LocalFileHandlerFactory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./widgets/")) //TODO
            });
            
            Cef.Initialize(settings);


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
                    bindingOptions = BindingOptions.DefaultBinder; //Use the default binder to serialize values into complex objects,

                   // bindingOptions = new BindingOptions { Binder = new MyCustomBinder() }); //No camelcase of names and specify a custom binder
                    //For backwards compatability reasons the default NameConverter doesn't change the case of the objects returned from methods calls.
                    //https://github.com/cefsharp/CefSharp/issues/2442
                    //Use the new name converter to bound object method names and property names of returned objects converted to camelCase
                    repo.NameConverter = new CamelCaseJavascriptNameConverter();
                    repo.Register("DropMeter", new JSComContext(), isAsync: false, options: bindingOptions);
                }
            };
            this.WebView.Address = $"{LocalFileHandlerFactory.SchemeName}://{PluginName}/index.html";
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
    }
}

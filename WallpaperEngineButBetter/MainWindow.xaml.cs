using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DropMeter;

namespace WallpaperEngineButBetter
{
    class WindowInfo
    {
        public IntPtr Handle;
        public string Title;
        public override string ToString()
        {
            return Title + " (" + Handle + ")";
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")] private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(
            IntPtr hWnd
        );


        List<WindowInfo> AllWindows = new List<WindowInfo>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private WindowInfo currentInfo;
        private IntPtr OldParent;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            W32.EnumWindows((hwnd, lParam) =>
            {
                //var shell = W32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", (IntPtr)null);
                // Find the WorkerW that contains the SHELLDLL_DefView window handle
                try
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hwnd, title, 256);
                    var final = title.ToString();
                    if (final != null && final != "")
                    {
                        ProcessChooser.Items.Add(new WindowInfo()
                        {
                            Handle = hwnd,
                            Title = final
                        });
                    }
                } catch {}


                return true;
            }, IntPtr.Zero);
            AllWindows = ProcessChooser.Items.Cast<WindowInfo>().ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentInfo == null)
            {
                currentInfo = (WindowInfo)ProcessChooser.SelectedItem;
                OldParent = GetParent(currentInfo.Handle);
                W32.EnumWindows((hwnd, lParam) =>
                {
                    var shell = W32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", (IntPtr)null);
                    // Find the WorkerW that contains the SHELLDLL_DefView window handle
                    if (shell != IntPtr.Zero)
                    {
                        //hwnd is WorkerW
                        if (AboveIcons.IsChecked == true)
                            W32.SetParent(currentInfo.Handle, shell);
                        else
                        {
                            W32.FindWindowEx(shell, IntPtr.Zero, "SysListView32", IntPtr.Zero);
                        }
                        // Find the current WorkerW window, the next WorkerW window. 
                        //IntPtr tempHwnd = W32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", (IntPtr)null);

                        // hide this window
                        //W32.ShowWindow(tempHwnd, 0);
                    }

                    return true;
                }, IntPtr.Zero);
            }
            else
            {
                W32.SetParent(currentInfo.Handle, OldParent);
                currentInfo = null;
                //OldParent = ;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

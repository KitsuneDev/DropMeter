using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DropMeter.Win32
{
    class WallpaperOverwrite
    {
        private IntPtr progman, workerw, handle, originalParent;
        public bool IsAttached { get; private set; } = false;
        
        private Window target;

        public WallpaperOverwrite(Window target)
        {
            this.target = target;
        }
        public void AttachToDesktop()
        {
            handle = new WindowInteropHelper(target).Handle;
            originalParent = W32.GetParent(handle);
            // Fetch the Progman window
            progman = W32.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            W32.SendMessageTimeout(progman,
                0x052C,
                new IntPtr(0xD),
                new IntPtr(0x1),
                W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out result);
            // Spy++ output
            // .....
            // 0x00010190 "" WorkerW
            //   ...
            //   0x000100EE "" SHELLDLL_DefView
            //     0x000100F0 "FolderView" SysListView32
            // 0x00100B8A "" WorkerW       <-- This is the WorkerW instance we are after!
            // 0x000100EC "Program Manager" Progman
            workerw = IntPtr.Zero;

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle,
                    IntPtr.Zero,
                    "SHELLDLL_DefView",
                    IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                        tophandle,
                        "WorkerW",
                        IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            SetParentWorkerW(handle);
            W32.SystemParametersInfo(W32.SPI_SETDESKWALLPAPER, 0, null, W32.SPIF_UPDATEINIFILE);

        }
        /// <summary>
        /// Adds the wp as child of spawned desktop-workerw window.
        /// </summary>
        /// <param name="windowHandle">handle of window</param>
        private void SetParentWorkerW(IntPtr windowHandle)
        {
            //Legacy, Windows 7
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
            {
                if (!workerw.Equals(progman)) //this should fix the win7 wallpaper disappearing issue.
                    W32.ShowWindow(workerw, 0);

                IntPtr ret = W32.SetParent(windowHandle, progman);
                if (ret.Equals(IntPtr.Zero))
                {
                    //LogUtil.LogWin32Error("Failed to set window parent");
                    throw new Exception("Failed to set window parent.");
                }
                //workerw is assumed as progman in win7, this is untested with all fn's: addwallpaper(), wp pause, resize events.. 
                workerw = progman;
            }
            else
            {
                IntPtr ret = W32.SetParent(windowHandle, workerw);
                if (ret.Equals(IntPtr.Zero))
                {
                    //LogUtil.LogWin32Error("Failed to set window parent");
                    throw new Exception("Failed to set window parent.");
                }
            }
            IsAttached = true;
        }

        public void DetachFromDesktop()
        {
            handle = new WindowInteropHelper(target).Handle;
            W32.SetParent(handle, originalParent);
            IsAttached = false;
        }
    }
}

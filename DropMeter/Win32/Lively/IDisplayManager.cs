using System.Collections.ObjectModel;
using System;
using System.Drawing;

using Lively.Models;

public interface IDisplayManager
{
    ObservableCollection<DisplayMonitor> DisplayMonitors { get; }
    DisplayMonitor PrimaryDisplayMonitor { get; }
    Rectangle VirtualScreenBounds { get; }

    event EventHandler DisplayUpdated;

    DisplayMonitor GetDisplayMonitorFromHWnd(IntPtr hWnd);
    DisplayMonitor GetDisplayMonitorFromPoint(Point point);
    bool IsMultiScreen();
    uint OnHwndCreated(IntPtr hWnd, out bool register);
    IntPtr OnWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
    bool ScreenExists(IDisplayMonitor display);
}
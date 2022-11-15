
using Lively.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DropMeter.FileHandler;

namespace Lively.Core
{
    public interface IDesktopCore : IDisposable
    {
        public event EventHandler WallpaperReset;
        public List<IWidget> LoadedWidgets { get; }
        public void AddWidget(IWidget widget);
        void RemoveWidget(ExtensionManifest widget);
        void ForceReload();
    }

    public interface IWidget
    {
        IntPtr InputHandle { get; set; }
        ExtensionManifest manifest { get; set; }
    }

    public class WidgetInfo : IWidget
    {
        public IntPtr InputHandle { get; set; }
        public ExtensionManifest manifest { get; set; }
    }
    public class DesktopCore : IDesktopCore
    {
        public event EventHandler WallpaperReset;
        public List<IWidget> LoadedWidgets { get; } = new List<IWidget>();
        public void AddWidget(IWidget widget)
        {
            LoadedWidgets.Add(widget);
        }

        public void RemoveWidget(ExtensionManifest widget)
        {
            LoadedWidgets.RemoveAll(x => x.manifest.Slug == widget.Slug);
        }

        public void ForceReload()
        {
            WallpaperReset?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            
        }
    }
}

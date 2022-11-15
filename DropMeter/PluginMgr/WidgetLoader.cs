using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using DropMeter.Annotations;
using DropMeter.FileHandler;
using Lively.Core;
using Newtonsoft.Json;


namespace DropMeter.PluginMgr
{
    public class WidgetLoader : INotifyPropertyChanged
    {
        public IDesktopCore DesktopCore;

        public WidgetLoader(IDesktopCore core)
        {
            DesktopCore = core;
        }
        public ObservableDictionary<string, HTMLWidget> OpenWidgets { get; set; } = new ObservableDictionary<string, HTMLWidget>();
        public ObservableDictionary<string, ExtensionManifest> Widgets { get; set; } = new ObservableDictionary<string, ExtensionManifest>();
        internal string LOADED_PATH = System.IO.Path.Combine(App.BASE, "loaded.json");

        public void SaveWidgetConfig()
        {
            string code = JsonConvert.SerializeObject(OpenWidgets.Keys.ToArray());
            File.WriteAllText(LOADED_PATH, code);
        }

        public void LoadWidgetConfig()
        {
            if (!File.Exists(LOADED_PATH)) return;
            string[] widgetsToLoad = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(LOADED_PATH));
            foreach (var wn in widgetsToLoad)
            {
                LoadWidget(wn, false);
            }

            ((App) Application.Current).CreateContextMenu();
        }

        public void LoadWidget(string widgetName, bool AutoRebuildContext = true)
        {
            if(OpenWidgets.ContainsKey(widgetName)) return;
            if (!Widgets.ContainsKey(widgetName))
            {
                Console.WriteLine($"Widget {widgetName} has not been found.");
                return;
            }

            var entry = Widgets[widgetName];
            var view = new HTMLWidget(widgetName, true);
            view.Show();
            OpenWidgets.Add(widgetName, view);
            if(AutoRebuildContext) ((App)Application.Current).CreateContextMenu();
            WidgetInfo info = new WidgetInfo()
            {
                InputHandle = new WindowInteropHelper(view).Handle,
                manifest = Widgets[widgetName]
            };
            DesktopCore.AddWidget(info);

            ChangedLoadedWidgets();

        }

        public void CloseWidget(string widgetName, bool AutoRebuildContext = true)
        {
            if (!OpenWidgets.ContainsKey(widgetName)) return;
            var data = OpenWidgets[widgetName];
            data.Close();
            OpenWidgets.Remove(widgetName);
            if (AutoRebuildContext) ((App)Application.Current).CreateContextMenu();
            ChangedLoadedWidgets();
            DesktopCore.RemoveWidget(Widgets[widgetName]);

        }

        private void ChangedLoadedWidgets()
        {
            Widgets.DependencyChanged();
            OpenWidgets.DependencyChanged();
            DesktopCore.ForceReload();
            OnPropertyChanged(null);
        }

        public void LoadAvailableWidgets()
        {
            Widgets.Clear();
            foreach (var widget in Directory.EnumerateDirectories(App.WidgetsBase))
            {
                var mfPath = Path.Combine(widget, "manifest.json");
                if (File.Exists(Path.Combine(widget, "index.html")) && File.Exists(mfPath))
                {
                    var wn = new DirectoryInfo(widget).Name;
                    var manifest = JsonConvert.DeserializeObject<ExtensionManifest>(File.ReadAllText(mfPath));
                    Widgets.Add(wn, manifest);
                    
                }
                else
                {
                    Console.WriteLine($"Widget at {widget} has no Entrypoint! Skipping.");
                }
            }
#if DEBUG
            if (!Widgets.ContainsKey("DebugWidget"))
            {
                Widgets.Add("DebugWidget", new ExtensionManifest()
                {
                    Name = "Debug Widget",
                    ExtensionType = ManifestType.Widget,
                    ManifestVersion = 1,
                    Slug = "DebugWidget",
                });
            }
#endif
            //Widgets.DependencyChanged();
        }
        public void LoadWidgets()
        {
            LoadAvailableWidgets();
            LoadWidgetConfig();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

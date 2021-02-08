using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DropMeter.Annotations;
using DropMeter.FileHandler;
using Newtonsoft.Json;


namespace DropMeter.PluginMgr
{
    public class WidgetLoader : INotifyPropertyChanged
    {
        public ObservableDictionary<string, HTMLWidget> OpenWidgets { get; set; } = new ObservableDictionary<string, HTMLWidget>();
        public ObservableDictionary<string, ExtensionManifest> Widgets { get; set; } = new ObservableDictionary<string, ExtensionManifest>();
        internal string LOADED_PATH = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loaded.json");

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

            ChangedLoadedWidgets();

        }

        public void CloseWidget(string widgetName)
        {
            if (!OpenWidgets.ContainsKey(widgetName)) return;
            var data = OpenWidgets[widgetName];
            data.Close();
            OpenWidgets.Remove(widgetName);
            ChangedLoadedWidgets();
        }

        private void ChangedLoadedWidgets()
        {
            Widgets.DependencyChanged();
            OpenWidgets.DependencyChanged();
            OnPropertyChanged(null);
        }

        public void LoadAvailableWidgets()
        {

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

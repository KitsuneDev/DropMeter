using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DropMeter.FileHandler.UIs;
using Newtonsoft.Json;

namespace DropMeter.FileHandler
{
    class DMFileHandler
    {
        public App App;
        internal DMFileHandler(App app)
        {
            this.App = app;
        }

        

        internal void OpenFile(string path)
        {
            if (path.EndsWith(".dmx"))
            {
                try {
                    Stream filestream = new FileStream(path, FileMode.Open);
                    ZipArchive file = new ZipArchive(filestream);
                    Stream manifestStream = file.GetEntry("manifest.json").Open();
                    var manifestReader = new StreamReader(manifestStream);
                
                
                    var manifest = JsonConvert.DeserializeObject<ExtensionManifest>(manifestReader.ReadToEnd());
                    filestream.Close();
                    var permAsk = new InstallExtension(manifest);
                    var hasPerm = permAsk.ShowDialog();
                    if (hasPerm == true)
                    {
                        var isPlugin = (manifest.ExtensionType == ManifestType.Plugin);
                        var baseDir = isPlugin ? App.PluginBase : App.WidgetsBase;
                        
                        var target = isPlugin ? baseDir : Path.Combine(baseDir, manifest.Slug);
                        var isUpdate = Directory.Exists(target);
                        if (isUpdate&& !isPlugin)
                        {
                            var shouldReplace = MessageBox.Show(
                                $"The {manifest.ExtensionType.ToString()} appears to be already installed. Would you like to replace it?", "Duplicate Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (shouldReplace == MessageBoxResult.Yes)
                            {
                                Directory.Delete(target, true);
                            }
                            else return;

                        }
                        ZipFile.ExtractToDirectory(path, target);
                        if(!isPlugin) App.widgetLoader.LoadAvailableWidgets();
                        var extraAction = (manifest.ExtensionType == ManifestType.Widget)
                            ? (isUpdate ? "It will be reloaded." : "Enable it at the Management Center.")
                            : "Please restart DropMeter."; //TODO: Reload plugins?
                        MessageBox.Show($"{manifest.Name} has been installed. {extraAction}");
                        //TODO: Reload widgets
                    }
               
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        $"An error was detected while handling the plugin package: {e.Message}",
                        "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                
                }
            }

            else
            {
                MessageBox.Show(
                    "An unknown file has been sent to DropMeter. We'll just pretend we did not see that.",
                    "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

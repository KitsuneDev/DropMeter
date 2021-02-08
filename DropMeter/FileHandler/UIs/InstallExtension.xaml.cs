using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DropMeter.FileHandler.UIs
{
    public class ColoredText
    {
        public string Text { get; set; }
        public SolidColorBrush Color { get; set; } = Brushes.Black;
    }
    /// <summary>
    /// Interaction logic for InstallExtension.xaml
    /// </summary>
    public partial class InstallExtension : Window
    {
        public ObservableCollection<ColoredText> RequiredPlugins { get; set; } = new ObservableCollection<ColoredText>();
        public ObservableCollection<ColoredText> RequiredCORS { get; set; } = new ObservableCollection<ColoredText>();
        public InstallExtension(ExtensionManifest manifest)
        {
            InitializeComponent();
            PluginType.Content = manifest.ExtensionType.ToString();
            PluginName.Content = manifest.Name;
            foreach (var requiredPlugin in manifest.RequiredPlugins)
            {
                RequiredPlugins.Add(new ColoredText()
                {
                    Text = requiredPlugin,
                    Color = (PluginLoader.Plugins.Find(x=>x.Slug == requiredPlugin) != null) ? Brushes.Black : Brushes.Red
                });
            }
            foreach (var requiredPlugin in manifest.RequiredCors)
            {
                RequiredCORS.Add(new ColoredText()
                {
                    Text = requiredPlugin,
                    Color = Brushes.Black
                });
            }
            //TODO: Show favicon
        }

        private void InstallBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void AbortBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

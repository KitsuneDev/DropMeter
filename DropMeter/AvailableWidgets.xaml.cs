using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DropMeter.FileHandler;
using DropMeter.PluginMgr;

namespace DropMeter
{
    /// <summary>
    /// Interaction logic for AvailableWidgets.xaml
    /// </summary>
    public partial class AvailableWidgets : Page
    {

        
        
        public AvailableWidgets()
        {
            InitializeComponent();
        }

        private void LoadWidget_OnClick(object sender, RoutedEventArgs e)
        {
            if (availableList.SelectedItem == null)
            {
                MessageBox.Show("Please select a widget.", "Oops", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var selected = (KeyValuePair<string, ExtensionManifest>)availableList.SelectedItem;
            
            App.widgetLoader.LoadWidget(selected.Value.Slug);
            //((CollectionViewSource)this.Resources["availableWidgets"]).View.Refresh();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            App.widgetLoader.SaveWidgetConfig();
        }

        private void FilterCollection(object sender, FilterEventArgs e)
        {
            var wn = ((KeyValuePair<string, ExtensionManifest>) e.Item).Key;
            var pass = !App.widgetLoader.OpenWidgets.ContainsKey(wn);
            e.Accepted = pass;
            
        }
    }
}

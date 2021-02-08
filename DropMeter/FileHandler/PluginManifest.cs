using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace DropMeter.FileHandler
{

    public enum ManifestType
    {
        Plugin,
        Widget
    }
    public class ExtensionManifest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ManifestType ExtensionType { get; set; }
        public int ManifestVersion { get; set; } = 1;
        public string Name { get; set; }
        public string Slug { get; set; }
        public string[] RequiredPlugins { get; set; }
        public string[] RequiredCors { get; set; }
    }
}

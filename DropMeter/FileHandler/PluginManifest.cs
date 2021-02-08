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
        public ManifestType ExtensionType;
        public int ManifestVersion = 1;
        public string Name;
        public string Slug;
        public string[] RequiredPlugins;
        public string[] RequiredCors;
    }
}

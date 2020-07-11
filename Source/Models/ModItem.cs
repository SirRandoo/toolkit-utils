using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ModItem
    {
        public string Author;
        public string Name;
        public string SteamId;
        public string Version;
    }
}

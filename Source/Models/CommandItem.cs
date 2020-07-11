using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CommandItem
    {
        [CanBeNull] public CommandData Data;
        public string Description;
        public string Name;
        public string Usage;

        [JsonConverter(typeof(StringEnumConverter))]
        public UserLevels UserLevel;

        public bool Shortcut => !Data?.IsShortcut ?? false;
    }
}

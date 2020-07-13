using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TraitData
    {
        public bool CanBypassLimit;
        [DefaultValue(new string[] { })] public string[] Conflicts;
        public bool CustomName;

        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(null)]
        public KarmaType? KarmaTypeForAdding;

        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(null)]
        public KarmaType? KarmaTypeForRemoving;

        [DefaultValue(new string[] { })] public string[] Stats;
    }
}

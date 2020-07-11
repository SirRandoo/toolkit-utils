using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
        [DefaultValue(new string[] { })] public string[] Stats;
    }
}

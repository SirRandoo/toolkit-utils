using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShopLegacy
    {
        public List<TraitItem> Traits { get; set; }
        public List<PawnKindItem> Races { get; set; }
    }
}

using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemData
    {
        public bool? IsMelee;
        public bool? IsRanged;
        public bool? IsWeapon;

        [JsonConverter(typeof(StringEnumConverter))]
        public KarmaType? KarmaType;

        public string Mod;

        public int? QuantityLimit;
    }
}

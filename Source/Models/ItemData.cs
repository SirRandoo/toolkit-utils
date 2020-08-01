using System.ComponentModel;
using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemData
    {
        [DefaultValue(null)] public bool CustomName;
        [DefaultValue(null)] public bool? IsMelee;
        [DefaultValue(null)] public bool? IsRanged;
        [DefaultValue(null)] public bool? IsWeapon;
        [DefaultValue(null)] public KarmaType? KarmaType;

        public string Mod;

        [DefaultValue(null)] public int? QuantityLimit;
    }
}

using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemData
    {
        public bool? IsMelee;
        public bool? IsRanged;
        public bool? IsWeapon;
        public KarmaType? KarmaType;

        public string Mod;

        public int? QuantityLimit;
    }
}

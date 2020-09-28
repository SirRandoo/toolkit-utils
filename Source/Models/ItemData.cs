using System.ComponentModel;
using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemData
    {
        [DefaultValue(null)] public string CustomName;

        public bool HasQuantityLimit;
        public bool IsMelee;
        public bool IsRanged;
        [DefaultValue(true)] public bool IsStuffAllowed;
        public bool IsWeapon;
        [DefaultValue(null)] public KarmaType? KarmaType;

        public string Mod;
        [DefaultValue(-1)] public int QuantityLimit;
    }
}

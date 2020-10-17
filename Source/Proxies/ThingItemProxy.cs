using JetBrains.Annotations;
using MoonSharp.Interpreter;
using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class ThingItemProxy
    {
        private readonly ThingItem thingItem;

        [MoonSharpHidden]
        public ThingItemProxy(ThingItem thingItem)
        {
            this.thingItem = thingItem;
        }

        public bool IsEnabled => thingItem.IsEnabled;
        public string DefName => thingItem.DefName;
        public string Name => thingItem.Name;
        public int Price => thingItem.Price;
        public string Abr => thingItem.Item?.abr;
        public string Abbreviation => thingItem.Item?.abr;
        public string Mod => thingItem.Mod;
        public string Category => thingItem.Category;

        public string Label => thingItem.Thing.label;
        public bool IsStuff => thingItem.Thing.IsStuff;
        public int StackLimit => thingItem.Thing.stackLimit;
        public bool Stackable => thingItem.Thing.stackLimit > 1;
        public float BaseMarketValue => thingItem.Thing.BaseMarketValue;
        public int BaseHitPoints => thingItem.Thing.BaseMaxHitPoints;
        public bool UsesHitPoints => thingItem.Thing.useHitPoints;
        public bool IsAddictiveDrug => thingItem.Thing.IsAddictiveDrug;
        public bool IsIngestible => thingItem.Thing.IsIngestible;
        public bool IsMedicine => thingItem.Thing.IsMedicine;
        public bool IsDrug => thingItem.Thing.IsDrug;
        public bool IsMeleeWeapon => thingItem.Thing.IsMeleeWeapon;
        public bool IsRangedWeapon => thingItem.Thing.IsRangedWeapon;
        public bool IsWeapon => thingItem.Thing.IsWeapon;
        public float BaseMass => thingItem.Thing.BaseMass;
        public bool IsNonMedicalDrug => thingItem.Thing.IsNonMedicalDrug;
        public bool IsPleasureDrug => thingItem.Thing.IsPleasureDrug;

        public bool HasQuantityLimit => thingItem.Data.HasQuantityLimit;
        public int QuantityLimit => thingItem.Data.QuantityLimit;
        public string KarmaType => thingItem.Data.KarmaType.ToString();
        public bool CanBeStuff => thingItem.Data.IsStuffAllowed;
    }
}

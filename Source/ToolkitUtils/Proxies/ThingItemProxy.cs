// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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

        public bool IsEnabled => thingItem.Enabled;
        public string DefName => thingItem.DefName;
        public string Name => thingItem.Name;
        public int Cost => thingItem.Cost;
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

        public bool HasQuantityLimit => thingItem.ItemData.HasQuantityLimit;
        public int QuantityLimit => thingItem.ItemData.QuantityLimit;
        public string KarmaType => thingItem.Data.KarmaType.ToString();
        public bool CanBeStuff => thingItem.ItemData.IsStuffAllowed;
    }
}

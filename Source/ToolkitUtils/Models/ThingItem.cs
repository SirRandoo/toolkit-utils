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

using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItem : IShopItemBase
    {
        private string _categoryCached;
        private ItemData _data;
        private Item _item;
        private ThingDef _producedAt;
        private bool _productionIndexed;
        private bool _usabilityIndexed;
        private bool _usable;

        [CanBeNull]
        [IgnoreDataMember]
        public Item Item
        {
            get
            {
                if (_item != null)
                {
                    return _item;
                }

                if (Thing == null)
                {
                    return null;
                }

                _item = StoreInventory.items.Find(i => i?.defname?.Equals(Thing.defName) == true);

                if (_item == null)
                {
                    _item = new Item(Thing.CalculateStorePrice(), Thing.label.ToToolkit(), Thing.defName);
                    StoreInventory.items.Add(_item);
                }
                else
                {
                    Enabled = _item.price > 0;
                }

                return _item;
            }
            set => _item = value;
        }

        [NotNull] [DataMember(Name = "mod")] public string Mod => Data?.Mod ?? Thing.TryGetModName();

        [IgnoreDataMember] public ThingDef Thing { get; set; }

        [CanBeNull]
        [IgnoreDataMember]
        public ThingDef ProducedAt
        {
            get
            {
                if (!_productionIndexed)
                {
                    _producedAt = DefDatabase<RecipeDef>.AllDefs.Where(i => i.recipeUsers != null && i.products.Count == 1 && i.products.Any(p => p.thingDef == Thing))
                       .SelectMany(i => i.recipeUsers)
                       .Distinct()
                       .OrderBy(i => (int)i.techLevel)
                       .FirstOrDefault();

                    _productionIndexed = true;
                }

                return _producedAt;
            }
        }

        [CanBeNull]
        [DataMember(Name = "data")]
        public ItemData ItemData
        {
            get => _data ??= (ItemData)Data;
            set => Data = _data = value;
        }

        [DataMember(Name = "category")]
        public string Category
        {
            get
            {
                if (!_categoryCached.NullOrEmpty())
                {
                    return _categoryCached;
                }

                string category = Thing?.FirstThingCategory?.LabelCap ?? string.Empty;

                if (category.NullOrEmpty() && Thing?.race != null)
                {
                    category = "TechLevel_Animal".Localize().CapitalizeFirst();
                }

                _categoryCached = category;

                return _categoryCached;
            }
        }

        [IgnoreDataMember]
        public bool IsUsable
        {
            get
            {
                if (_usabilityIndexed || Thing == null)
                {
                    return _usable;
                }

                _usable = CompatRegistry.AllUsabilityHandlers.Any(h => h.IsUsable(Thing));
                _usabilityIndexed = true;

                return _usable;
            }
        }

        [CanBeNull]
        [DataMember(Name = "defName")]
        public string DefName
        {
            get => Item?.defname ?? Thing?.defName;
            set
            {
                if (Item != null)
                {
                    Item.defname = value;
                }

                Thing = DefDatabase<ThingDef>.GetNamed(value, false);
            }
        }

        [DataMember(Name = "enabled")] public bool Enabled { get; set; }

        [NotNull]
        [DataMember(Name = "name")]
        public string Name
        {
            get => ItemData?.CustomName ?? Item?.abr ?? "Fetching...";
            set
            {
                if (ItemData != null)
                {
                    ItemData.CustomName = value;
                }
            }
        }

        [DataMember(Name = "price")]
        public int Cost
        {
            get => Item?.price ?? -10;
            set
            {
                if (Item != null)
                {
                    Item.price = value;
                }
            }
        }

        [CanBeNull]
        [IgnoreDataMember]
        public IShopDataBase Data
        {
            get
            {
                if (_data == null && ToolkitUtils.Data.ItemData.TryGetValue(Thing.defName, out ItemData result))
                {
                    _data = result;
                }

                return _data;
            }
            set
            {
                _data = (ItemData)value;

                if (Item?.defname != null)
                {
                    ToolkitUtils.Data.ItemData[Item!.defname] = _data;
                }
            }
        }

        public void ResetName()
        {
            if (ItemData != null)
            {
                ItemData.CustomName = null;
            }
        }

        public void ResetPrice()
        {
            if (Thing == null)
            {
                return;
            }

            Cost = Thing.CalculateStorePrice();

            if (!Enabled)
            {
                Enabled = true;
            }
        }

        public void ResetData()
        {
            ItemData?.Reset();
        }

        public void Update()
        {
            if (Item == null)
            {
                return;
            }

            if (Item.price == 0)
            {
                Enabled = false;
                Item.price = -10;

                return;
            }

            switch (Enabled)
            {
                case true when Item.price < 0:
                    Item.price = Thing.CalculateStorePrice();

                    break;
                case false when Item.price > 0:
                    Item.price = -10;

                    break;
            }

            Enabled = Item.price > 0;
        }

        public override string ToString()
        {
            var container = "ThingItem(\n";

            container += "  Item(\n";
            container += $"    defName={Item?.defname?.ToStringSafe()}\n";
            container += $"    abr={Item?.abr?.ToStringSafe()}\n";
            container += $"    price={Item?.price.ToStringSafe()}\n";
            container += "  ),\n";

            container += "  Thing(\n";
            container += $"    defName={Thing?.defName?.ToStringSafe()}\n";
            container += "  ),\n";

            container += $"  Mod={Mod.ToStringSafe()},\n";
            container += $"  Enabled={Enabled}\n";
            container += ")";

            return container;
        }
    }
}

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

using System.Data;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItem : IShopItemBase
    {
        private string categoryCached;
        private ItemData data;
        private Item item;

        [CanBeNull]
        public Item Item
        {
            get
            {
                if (item != null)
                {
                    return item;
                }

                if (Thing == null)
                {
                    return null;
                }

                item = StoreInventory.items.Find(i => i?.defname?.Equals(Thing.defName) ?? false);

                if (item == null)
                {
                    item = new Item(Thing.CalculateStorePrice(), Thing.label.ToToolkit(), Thing.defName);
                    StoreInventory.items.Add(item);
                }
                else
                {
                    Enabled = item.price > 0;
                }

                return item;
            }
            set => item = value;
        }

        [NotNull] public string Mod => Data?.Mod ?? Thing.modContentPack?.Name ?? "Unknown";
        public ThingDef Thing { get; set; }

        [CanBeNull]
        [DataMember(Name = "data")]
        public ItemData ItemData
        {
            get => data ??= (ItemData) Data;
            set => Data = data = value;
        }

        public string Category
        {
            get
            {
                if (!categoryCached.NullOrEmpty())
                {
                    return categoryCached;
                }

                string category = Thing?.FirstThingCategory?.LabelCap ?? string.Empty;

                if (category.NullOrEmpty() && Thing?.race != null)
                {
                    category = "TechLevel_Animal".Localize().CapitalizeFirst();
                }

                categoryCached = category;
                return categoryCached;
            }
        }

        public string DefName
        {
            get => Item?.defname ?? Thing.defName;
            set => throw new ReadOnlyException();
        }

        public bool Enabled { get; set; }

        [NotNull]
        public string Name
        {
            get => ItemData?.CustomName ?? Item?.abr ?? "Fetching...";
            set => throw new ReadOnlyException();
        }

        [DataMember(Name = "price")]
        public int Cost
        {
            get => Item?.price ?? -10;
            set => throw new ReadOnlyException();
        }

        [CanBeNull]
        [IgnoreDataMember]
        public IShopDataBase Data
        {
            get
            {
                if (data == null && ToolkitUtils.Data.ItemData.TryGetValue(Thing.defName, out ItemData result))
                {
                    data = result;
                }

                return data;
            }
            set => ToolkitUtils.Data.ItemData[Item!.defname] = data = (ItemData) value;
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
            container += $"    defName={Item?.defname}\n";
            container += $"    abr={Item?.abr.ToStringSafe()}\n";
            container += $"    price={Item?.price.ToStringSafe()}\n";
            container += "  ),\n";

            container += "  Thing(\n";
            container += $"    defName={Thing.defName.ToStringSafe()}\n";
            container += "  ),\n";

            container += $"  Mod={Mod},\n";
            container += $"  Enabled={Enabled}\n";
            container += ")";

            return container;
        }

        [NotNull]
        public string GetDefaultName()
        {
            return Thing.label.ToToolkit();
        }

        [NotNull]
        public static ThingItem FromData([NotNull] Item item, ThingDef thing)
        {
            var thingItem = new ThingItem {Item = item, Thing = thing, Enabled = item.price > 0};
            ItemData _ = thingItem.ItemData;
            thingItem.Update();

            return thingItem;
        }
    }
}

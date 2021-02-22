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

using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItem
    {
        private string categoryCached;
        private ItemData data;

        public bool IsEnabled;
        private Item item;
        public string DefName => Item?.defname ?? Thing.defName;
        public string Name => Data?.CustomName ?? Item?.abr ?? "Fetching...";
        public int Price => Item?.price ?? -10;

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
                    IsEnabled = item.price > 0;
                }

                return item;
            }
            set => item = value;
        }

        public string Mod => Data?.Mod ?? Thing.modContentPack?.Name ?? "Unknown";
        public ThingDef Thing { get; set; }

        public ItemData Data
        {
            get
            {
                if (data == null && ToolkitUtils.Data.ItemData.TryGetValue(Thing.defName, out ItemData result))
                {
                    data = result;
                }

                return data;
            }
            set
            {
                ToolkitUtils.Data.ItemData[Item.defname] = value;
                data = value;
            }
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

        public void Update()
        {
            if (Item == null)
            {
                return;
            }

            if (Item.price == 0)
            {
                IsEnabled = false;
                Item.price = -10;
                return;
            }

            if (IsEnabled && Item.price < 0)
            {
                Item.price = Thing.CalculateStorePrice();
            }
            else if (!IsEnabled && Item.price > 0)
            {
                Item.price = -10;
            }

            IsEnabled = Item.price > 0;
        }

        public override string ToString()
        {
            var container = "ThingItem(\n";

            container += "  Item(\n";
            container += $"    defName={Item.defname}\n";
            container += $"    abr={Item.abr.ToStringSafe()}\n";
            container += $"    price={Item.price.ToStringSafe()}\n";
            container += "  ),\n";

            container += "  Thing(\n";
            container += $"    defName={Thing.defName.ToStringSafe()}\n";
            container += "  ),\n";

            container += $"  Mod={Mod},\n";
            container += $"  Enabled={IsEnabled}\n";
            container += ")";

            return container;
        }

        public string GetDefaultName()
        {
            return Thing.label.ToToolkit();
        }

        public static ThingItem FromData(Item item, ThingDef thing)
        {
            var thingItem = new ThingItem {Item = item, Thing = thing, IsEnabled = item.price > 0};
            ItemData _ = thingItem.Data;
            thingItem.Update();

            return thingItem;
        }
    }
}

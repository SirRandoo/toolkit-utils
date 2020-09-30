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

                item = StoreInventory.items.Find(i => i.defname.Equals(Thing.defName));

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

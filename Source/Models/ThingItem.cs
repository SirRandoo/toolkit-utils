using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItem
    {
        private string categoryCached;
        public List<FloatMenuOption> CategoryContextOptions;
        private ItemData data;
        public List<FloatMenuOption> InfoContextOptions;

        public bool IsEnabled;
        public List<FloatMenuOption> PriceContextOptions;
        public string DefName => Item.defname;
        public string Name => Data?.CustomName ?? Item.abr;
        public int Price => Item.price;

        public Item Item { get; set; }

        public string Mod => Data?.Mod ?? Thing.modContentPack?.Name ?? "Unknown";
        public ThingDef Thing { get; set; }

        public ItemData Data
        {
            get
            {
                if (data == null && ToolkitUtils.Data.ItemData.TryGetValue(Item.defname, out ItemData result))
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
            thingItem.Update();

            return thingItem;
        }
    }
}

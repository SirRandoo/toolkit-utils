using System.Collections.Generic;
using Newtonsoft.Json;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItem
    {
        private string categoryCached;
        public List<FloatMenuOption> CategoryContextOptions;
        [JsonIgnore] private ItemData data;
        public bool Enabled;

        public List<FloatMenuOption> InfoContextOptions;
        public List<FloatMenuOption> PriceContextOptions;

        public Item Item { get; set; }
        public string Mod => Thing.modContentPack?.Name ?? "";
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
                Enabled = false;
                Item.price = -10;
                return;
            }

            if (Enabled && Item.price < 0)
            {
                Item.price = Thing.CalculateStorePrice();
            }
            else if (!Enabled && Item.price > 0)
            {
                Item.price = -10;
            }

            Enabled = Item.price > 0;
        }

        public void DrawItemInfo(Rect canvas)
        {
            var iconRegion = new Rect(27f, canvas.y, 27f, canvas.height);
            var labelRegion = new Rect(iconRegion.width + 5f + 27f, canvas.y, canvas.width - 30f, canvas.height);

            Widgets.Checkbox(0f, canvas.y, ref Enabled, paintable: true);

            TextAnchor cache = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRegion, Thing?.LabelCap ?? Item.abr);
            Text.Anchor = cache;

            if (Thing != null)
            {
                Widgets.ThingIcon(iconRegion, Thing);
            }

            if (Widgets.ButtonInvisible(canvas, false))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(Thing));
            }

            Widgets.DrawHighlightIfMouseover(canvas);
        }

        public override string ToString()
        {
            var container = "Container(\n";

            container += "  Item(\n";
            container += $"    defName={Item.defname}\n";
            container += $"    abr={Item.abr.ToStringSafe()}\n";
            container += $"    price={Item.price.ToStringSafe()}\n";
            container += "  ),\n";

            container += "  Thing(\n";
            container += $"    defName={Thing.defName.ToStringSafe()}\n";
            container += "  ),\n";

            container += $"  Mod={Mod},\n";
            container += $"  Enabled={Enabled}\n";
            container += ")";

            return container;
        }
    }
}

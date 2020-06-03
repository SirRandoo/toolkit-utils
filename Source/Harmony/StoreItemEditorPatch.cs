using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_ItemEditor), "UpdateStoreItemList")]
    public class StoreItemEditorPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            List<Item> inventory = StoreInventory.items;
            HashSet<ThingDef> tradeables = StoreDialog.GetTradeables().ToHashSet();

            if (TkUtils.ShopExpansion.Races != null)
            {
                foreach (Item item in inventory
                    .Where(item => !item.defname.NullOrEmpty())
                    .Where(item => item.price >= 0)
                    .Where(item => TkUtils.ShopExpansion.Races.Any(r => r.DefName.Equals(item.defname))))
                {
                    item.price = -10;
                }
            }

            StoreInventory.items = inventory
                .Where(i => !i.defname.NullOrEmpty())
                .Where(i => tradeables.Any(t => t.defName.Equals(i.defname)))
                .ToList();

            for (int index = StoreDialog.Containers.Count - 1; index >= 0; index--)
            {
                StoreDialog.Container container = StoreDialog.Containers[index];

                if (StoreInventory.items.Contains(container.Item))
                {
                    continue;
                }

                try
                {
                    StoreDialog.Containers.RemoveAt(index);
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            foreach (Item item in StoreInventory.items.Where(i => i.abr.NullOrEmpty()))
            {
                ThingDef thing = tradeables.FirstOrDefault(i => i.defName.Equals(item.defname));

                if (thing == null)
                {
                    continue;
                }

                item.abr = thing.label.ToToolkit();
            }
        }
    }
}

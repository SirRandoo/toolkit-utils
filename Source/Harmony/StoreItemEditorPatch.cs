using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_ItemEditor), "UpdateStoreItemList")]
    public static class StoreItemEditorPatch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            List<Item> inventory = StoreInventory.items;
            HashSet<ThingDef> tradeables = StoreDialog.GetTradeables().ToHashSet();

            if (Data.PawnKinds != null)
            {
                DisableKinds(inventory);
            }

            StoreInventory.items = inventory.Where(i => !i.defname.NullOrEmpty())
               .Where(i => tradeables.Any(t => t.defName.Equals(i.defname)))
               .ToList();

            RemoveDanglingItems();
            FixNullItemNames(tradeables);

            List<ToolkitItem> items = PrepareItems(tradeables).ToList();

            if (TkSettings.Offload)
            {
                Task.Run(
                    () =>
                    {
                        Data.SaveJson(new ItemList {Items = items}, Paths.ToolkitItemFilePath);
                        Data.SaveItemData(Paths.ItemDataFilePath);
                    }
                );
            }
            else
            {
                Data.SaveJson(new ItemList {Items = items}, Paths.ToolkitItemFilePath);
                Data.SaveItemData(Paths.ItemDataFilePath);
            }

            return true;
        }

        private static IEnumerable<ToolkitItem> PrepareItems(HashSet<ThingDef> tradeables)
        {
            var items = new List<ToolkitItem>();

            foreach (Item item in StoreInventory.items)
            {
                ThingDef thingDef = tradeables.FirstOrDefault(t => t.defName.Equals(item.defname));
                string category = thingDef?.FirstThingCategory?.LabelCap;

                if (category.NullOrEmpty() && thingDef?.race != null)
                {
                    category = "Animal";
                }

                items.Add(
                    new ToolkitItem {Abr = item.abr, DefName = item.defname, Price = item.price, Category = category}
                );
            }

            return items;
        }

        private static void FixNullItemNames(HashSet<ThingDef> tradeables)
        {
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

        private static void RemoveDanglingItems()
        {
            for (int index = Data.Items.Count - 1; index >= 0; index--)
            {
                ThingItem thingItem = Data.Items[index];

                if (StoreInventory.items.Contains(thingItem.Item))
                {
                    continue;
                }

                try
                {
                    Data.Items.RemoveAt(index);
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        private static void DisableKinds(List<Item> inventory)
        {
            foreach (Item item in inventory.Where(item => !item.defname.NullOrEmpty())
               .Where(item => item.price >= 0)
               .Where(item => Data.PawnKinds.Any(r => r.DefName.Equals(item.defname))))
            {
                item.price = -10;
            }
        }
    }
}

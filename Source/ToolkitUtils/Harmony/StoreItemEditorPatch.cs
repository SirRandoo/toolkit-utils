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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class StoreItemEditorPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Store_ItemEditor), "UpdateStoreItemList");
        }

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
                        async () =>
                        {
                            await Data.SaveJsonAsync(new ItemList { Items = items }, Paths.ToolkitItemFilePath);
                            await Data.SaveItemDataAsync(Paths.ItemDataFilePath);
                        }
                    )
                   .ConfigureAwait(false);
            }
            else
            {
                Data.SaveJson(new ItemList { Items = items }, Paths.ToolkitItemFilePath);
                Data.SaveItemData(Paths.ItemDataFilePath);
            }

            return false;
        }

        [NotNull]
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
                    new ToolkitItem { Abr = item.abr, DefName = item.defname, Price = item.price, Category = category }
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

        private static void DisableKinds([NotNull] List<Item> inventory)
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

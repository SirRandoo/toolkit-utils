﻿// ToolkitUtils
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

namespace SirRandoo.ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch to clean the item list before Twitch Toolkit
///     saves it to disk.
/// </summary>
/// <remarks>
///     This patch is responsible for removing items that don't have a
///     proper name or def name, as well as mismatching items between
///     Utils and Twitch Toolkit's internal item lists, then transforms
///     the Utils containers into a form compatible with Twitch Toolkit's
///     item list file.
/// </remarks>
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class StoreItemEditorPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Store_ItemEditor), nameof(Store_ItemEditor.UpdateStoreItemList));
    }

    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null)
        {
            return null;
        }

        TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

        return null;
    }

    private static bool Prefix()
    {
        List<Item> inventory = StoreInventory.items;
        var tradeables = new HashSet<ThingDef>(StoreDialog.GetTradeables());
        List<string> tradeableIds = tradeables.Select(t => t.defName).ToList();

        if (Data.PawnKinds is not null)
        {
            DisableKinds(inventory);
        }

        StoreInventory.items = inventory.Where(i => !string.IsNullOrEmpty(i.defname)).Where(i => tradeableIds.Contains(i.defname)).ToList();

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

    private static IEnumerable<ToolkitItem> PrepareItems(HashSet<ThingDef> tradeables)
    {
        var items = new List<ToolkitItem>();

        foreach (Item item in StoreInventory.items)
        {
            ThingDef? thingDef = tradeables.FirstOrDefault(t => t.defName.Equals(item.defname, StringComparison.Ordinal));
            string? category = thingDef?.FirstThingCategory?.LabelCap;

            if (string.IsNullOrEmpty(category) && thingDef?.race is not null)
            {
                category = "Animal";
            }

            items.Add(new ToolkitItem { Abr = item.abr, DefName = item.defname, Price = item.price, Category = category! });
        }

        return items;
    }

    private static void FixNullItemNames(HashSet<ThingDef> tradeables)
    {
        foreach (Item item in StoreInventory.items.Where(i => i.abr.NullOrEmpty()))
        {
            ThingDef? thing = tradeables.FirstOrDefault(i => i.defName.Equals(item.defname, StringComparison.Ordinal));

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
            catch (IndexOutOfRangeException)
            {
                // While it's unlikely this will throw an exception,
                // we'll still swallow the exception in the event it
                // does.
            }
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

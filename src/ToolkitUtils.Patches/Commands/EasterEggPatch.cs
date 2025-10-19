// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Patches. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Mod.Domain.Products;
using TwitchToolkit.Utilities;
using Verse;

namespace ToolkitUtils.Patches;

/// <summary>A Harmony patch to fix SirRandoo's Easter egg erroring out when it selects an animal instead of an item.</summary>
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class EasterEggPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(EasterEggPatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(EasterEgg), nameof(EasterEgg.ExecuteSirRandooEasterEgg));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not patch {Method} :: Your game may crash when SirRandoo's easter egg is executed", original.FullDescription());

        return null;
    }

    [UsedImplicitly]
    private static bool Prefix()
    {
        Task.Run(async () =>
            {
                Thing? item = await CreateItem();

                if (item is null) return;

                await SpawnItem(item);
            }
        );

        return false;
    }

    private static async Task<Thing?> CreateItem()
    {
        List<ItemProduct> items = [..Registries.Items.AllRegistrants,];

        for (int i = items.Count - 1; i >= 0; i--)
        {
            ItemProduct? item = items[i];

            if (!item.Purchasable || item.Price < 200 || item.Price > 2000) items.RemoveAt(i);
            if (item.Metadata is ItemProductMetadata metadata && metadata.Properties.HasFlagFast(ItemProperties.Wildlife)) items.RemoveAt(i);
        }

        ItemProduct? product = await MainThreadExtensions.OnMainAsync(items.RandomElement);
        ThingDef? stuff = null;

        if (product.Metadata is ItemProductMetadata productMetadata && productMetadata.Properties.HasFlag(ItemProperties.Material))
            stuff = await MainThreadExtensions.OnMainAsync(GenStuff.RandomStuffByCommonalityFor, product.Def, TechLevel.Undefined);

        Thing? result = await MainThreadExtensions.OnMainAsync(ThingMaker.MakeThing, product.Def, stuff);
        var qualityComp = await result.GetCompAsync<CompQuality>();

        if (qualityComp is null) return result;

        QualityCategory quality = await MainThreadExtensions.OnMainAsync(QualityUtility.GenerateQualityTraderItem);
        await MainThreadExtensions.OnMainAsync(qualityComp.SetQuality, quality, (ArtGenerationContext?)ArtGenerationContext.Outsider);

        return result;
    }

    private static async Task SpawnItem(Thing item)
    {
        Map? map = Current.Game.AnyPlayerHomeMap;

        if (map == null) return;

        Thing container = item;
        IntVec3 position = DropCellFinder.TradeDropSpot(map);

        if (item.def.Minifiable)
        {
            ThingDef? minifiedDef = item.def.minifiedDef;
            var minifiedThing = (MinifiedThing)ThingMaker.MakeThing(minifiedDef);
            minifiedThing.InnerThing = item;
            minifiedThing.stackCount = 1;

            container = minifiedThing;
        }
        else
            item.stackCount = 1;

        if (await IsRoofed(map, position))
            await MainThreadExtensions.OnMainAsync(GenSpawn.Spawn, container, position, map, WipeMode.VanishOrMoveAside);
        else
        {
            ActiveTransporterInfo info = new()
            {
                SingleContainedThing = container, leaveSlag = false, moveItemsAsideBeforeSpawning = true,
            };

            await MainThreadExtensions.OnMainAsync(DropPodUtility.MakeDropPodAt, position, map, info, (Faction)null!);
        }

        await Find.LetterStack.ReceiveLetterAsync(label: "SirRandoo is here", text: @"SirRandoo has sent you a rare item! Enjoy!", LetterDefOf.PositiveEvent, container);
    }

    private static async Task<bool> IsRoofed(Map map, IntVec3 position)
    {
        RoofDef? roof = await MainThreadExtensions.OnMainAsync(map.roofGrid.RoofAt, position);

        return roof is not null;
    }
}

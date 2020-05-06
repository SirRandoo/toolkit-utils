﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(StoreItemsWindow), "PostClose")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ItemWindowPatch
    {
        [HarmonyPrefix]
        public static void PostClose(List<ThingDef> ___cachedTradeables, List<int> ___tradeablesPrices)
        {
            for (var i = 0; i < ___cachedTradeables.Count; i++)
            {
                var t = ___cachedTradeables[i];

                if (!(t.race?.Humanlike ?? false))
                {
                    continue;
                }

                var price = ___tradeablesPrices[i];

                if (price < 0)
                {
                    continue;
                }

                ___tradeablesPrices[i] = -10;

                ShopExpansionHelper.SaveData(TkUtils.ShopExpansion, ShopExpansionHelper.ExpansionFile);
            }
        }
    }
}
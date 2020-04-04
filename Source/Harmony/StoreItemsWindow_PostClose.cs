using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(StoreItemsWindow), "PostClose")]
    public class StoreItemsWindow_PostClose
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

                foreach (var race in TkUtils.ShopExpansion.Races.Where(race => race.DefName.Equals(t.defName)))
                {
                    race.Price = price;
                    ___tradeablesPrices[i] = -10;
                }
                
                ShopExpansionHelper.SaveData(TkUtils.ShopExpansion, ShopExpansionHelper.ExpansionFile);
            }
        }
    }
}

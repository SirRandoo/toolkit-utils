using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(StoreItemsWindow), "PostClose")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class ItemWindowPatch
    {
        public static void Prefix(List<ThingDef> ___cachedTradeables, List<int> ___tradeablesPrices)
        {
            for (var i = 0; i < ___cachedTradeables.Count; i++)
            {
                ThingDef t = ___cachedTradeables[i];

                if (!(t.race?.Humanlike ?? false))
                {
                    continue;
                }

                int price = ___tradeablesPrices[i];

                if (price < 0)
                {
                    continue;
                }

                ___tradeablesPrices[i] = -10;
            }

            SaveKinds();
        }

        private static void SaveKinds()
        {
            if (TkSettings.Offload)
            {
                Task.Run(
                    () =>
                    {
                        switch (TkSettings.DumpStyle)
                        {
                            case "MultiFile":
                                Data.SavePawnKinds(Paths.PawnKindFilePath);
                                return;
                            case "SingleFile":
                                Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                                return;
                        }
                    }
                );
            }
            else
            {
                switch (TkSettings.DumpStyle)
                {
                    case "MultiFile":
                        Data.SavePawnKinds(Paths.PawnKindFilePath);
                        return;
                    case "SingleFile":
                        Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                        return;
                }
            }
        }
    }
}

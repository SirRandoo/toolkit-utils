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
        public static void Prefix([NotNull] List<ThingDef> ___cachedTradeables, List<int> ___tradeablesPrices)
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

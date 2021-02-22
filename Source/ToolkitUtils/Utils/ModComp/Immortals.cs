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
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class Immortals
    {
        public static readonly bool Active;
        internal static readonly HediffDef ImmortalHediffDef;

        static Immortals()
        {
            foreach (Mod _ in LoadedModManager.ModHandles.Where(
                h => h.Content.PackageId.EqualsIgnoreCase("fridgeBaron.Immortals")
            ))
            {
                try
                {
                    ImmortalHediffDef =
                        DefDatabase<HediffDef>.AllDefs.FirstOrDefault(h => h.defName.Equals("IH_Immortal"));

                    Active = ImmortalHediffDef != null;
                }
                catch (Exception e)
                {
                    LogHelper.Error("Compatibility class for Immortals failed!", e);
                }
            }
        }

        public static bool TryGrantImmortality(Pawn pawn)
        {
            try
            {
                pawn.health.AddHediff(ImmortalHediffDef);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error($"Could not grant immortality to {pawn.LabelCap}", e);
                return false;
            }
        }
    }
}

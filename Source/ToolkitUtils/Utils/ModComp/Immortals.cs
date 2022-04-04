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
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class Immortals
    {
        public static readonly bool Active = ModLister.GetActiveModWithIdentifier("fridgeBaron.Immortals") != null;
        internal static readonly HediffDef ImmortalHediffDef = DefDatabase<HediffDef>.GetNamed("IH_Immortal", false);

        public static bool TryGrantImmortality([NotNull] Pawn pawn)
        {
            if (!Active)
            {
                return false;
            }

            try
            {
                pawn.health.AddHediff(ImmortalHediffDef);

                return true;
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error($"Could not grant immortality to {pawn.LabelCap}", e);

                return false;
            }
        }
    }
}

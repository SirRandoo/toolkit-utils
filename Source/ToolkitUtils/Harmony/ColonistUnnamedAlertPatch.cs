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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ColonistUnnamedAlertPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Alert_UnnamedColonist), "GetReport");
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public static bool Prefix(ref AlertReport __result)
        {
            __result = false;

            if (!ToolkitSettings.ViewerNamedColonistQueue)
            {
                return false;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (component == null)
            {
                return false;
            }

            Dictionary<string, Pawn> pawnHistory = component.pawnHistory;
            List<Pawn> colonistsSpawned = Find.CurrentMap?.mapPawns.FreeColonistsSpawned;

            if (colonistsSpawned == null || colonistsSpawned.Count == pawnHistory.Count)
            {
                return false;
            }

            var container = new List<Pawn>();

            for (var i = 0; i < colonistsSpawned.Count; i++)
            {
                Pawn pawn = colonistsSpawned[i];

                if (pawnHistory.ContainsKey(pawn.LabelShort) || pawn.IsBorrowedByAnyFaction() || CompatRegistry.Magic?.IsUndead(pawn) == true)
                {
                    continue;
                }

                container.Add(pawn);
            }

            if (container.Count > 0)
            {
                __result = AlertReport.CulpritsAre(container);
            }

            return false;
        }
    }
}

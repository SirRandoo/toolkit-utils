using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Alert_UnnamedColonist), "GetReport")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class AlertUnnamedColonistPatch
    {
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
            List<Pawn> colonistsSpawned = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned;

            if (colonistsSpawned.Count == pawnHistory.Count)
            {
                return false;
            }

            List<Pawn> container = colonistsSpawned.Where(c => !component.pawnHistory.ContainsKey(c.Name.ToStringShort))
                // Questing
               .Where(pawn => !pawn.IsBorrowedByAnyFaction())
                // RimWorld of Magic
               .Where(pawn => !pawn.IsUndead())
               .ToList();

            if (container.Count > 0)
            {
                __result = AlertReport.CulpritsAre(container);
            }

            return false;
        }
    }
}

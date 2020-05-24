using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Alert_UnnamedColonist), "GetReport")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class AlertUnnamedColonistPatch
    {
        [HarmonyPrefix]
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

            var pawnHistory = component.pawnHistory;
            var colonistsSpawned = Helper.AnyPlayerMap.mapPawns.FreeColonistsSpawned;

            if (colonistsSpawned.Count == pawnHistory.Count)
            {
                return false;
            }

            var container = colonistsSpawned
                .Where(c => !component.HasPawnBeenNamed(c))
                // Questing
                .Where(pawn => !pawn.IsBorrowedByAnyFaction())
                // RimWorld of Magic
                .Where(pawn => !pawn.story.traits.allTraits.Any(t => t.def.defName.EqualsIgnoreCase("Undead")))
                .ToList();

            if (container.Any())
            {
                __result = AlertReport.CulpritsAre(container);
            }

            return false;
        }
    }
}

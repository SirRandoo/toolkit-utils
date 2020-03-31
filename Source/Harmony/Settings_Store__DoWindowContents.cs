using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Settings_Store), "DoWindowContents")]
    public static class Settings_Store__DoWindowContents
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DoWindowContents(IEnumerable<CodeInstruction> instructions)
        {
            var method = AccessTools.Method(typeof(Settings_Store__DoWindowContents), nameof(DrawUtilsContents));
            var methodMarker = AccessTools.Method(typeof(Listing), nameof(Listing.Gap));
            var markerFound = false;

            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand as string == "Events Edit")
                {
                    markerFound = true;
                }

                if (markerFound && instruction.opcode == OpCodes.Callvirt && ReferenceEquals(instruction.operand, methodMarker))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, method);
                }

                yield return instruction;
            }
        }

        private static void DrawUtilsContents(Listing_Standard optionsListing)
        {
            optionsListing.Gap();
            optionsListing.GapLine();
            if (optionsListing.ButtonTextLabeled("Traits".Translate(), "Open"))
            {
                Find.WindowStack.Add(new TraitConfigDialog());
            }

            optionsListing.Gap();
            optionsListing.GapLine();
            if (optionsListing.ButtonTextLabeled(Find.ActiveLanguageWorker.Pluralize("Race".Translate()), "Open"))
            {
                Find.WindowStack.Add(new RaceConfigDialog());
            }
        }
    }
}

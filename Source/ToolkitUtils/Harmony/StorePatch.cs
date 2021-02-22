﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Settings_Store), "DoWindowContents")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class StorePatch
    {
        private static readonly MethodInfo UtilsInjectorMethod;
        private static readonly MethodInfo InjectionSiteMarkerMethod;

        static StorePatch()
        {
            UtilsInjectorMethod = AccessTools.Method(typeof(StorePatch), nameof(DrawUtilsContents));
            InjectionSiteMarkerMethod = AccessTools.Method(typeof(Listing), nameof(Listing.Gap));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var markerFound = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldstr && instruction.OperandIs("Events Edit"))
                {
                    markerFound = true;
                }

                if (markerFound
                    && instruction.opcode == OpCodes.Callvirt
                    && ReferenceEquals(instruction.operand, InjectionSiteMarkerMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, UtilsInjectorMethod);
                }

                yield return instruction;
            }
        }

        private static void DrawUtilsContents(Listing_Standard optionsListing)
        {
            if (optionsListing == null)
            {
                LogHelper.Warn("Could not inject Utils' shop buttons into Toolkit's store.  You should report this.");
                return;
            }

            optionsListing.Gap();
            optionsListing.GapLine();
            if (optionsListing.ButtonTextLabeled(
                $"[ToolkitUtils] {"Traits".Localize()}",
                "TKUtils.Buttons.Open".Localize()
            ))
            {
                Find.WindowStack.Add(new TraitConfigDialog());
            }

            optionsListing.Gap();
            optionsListing.GapLine();
            if (optionsListing.ButtonTextLabeled(
                Find.ActiveLanguageWorker.Pluralize($"[ToolkitUtils] {"Race".Localize().Pluralize()}"),
                "TKUtils.Buttons.Open".Localize()
            ))
            {
                Find.WindowStack.Add(new PawnKindConfigDialog());
            }

        #if DEBUG
            optionsListing.Gap();
            optionsListing.GapLine();
            if (optionsListing.ButtonTextLabeled("Editor", "TKUtils.Buttons.Open".Localize()))
            {
                // Find.WindowStack.Add(new LuaEditorWindow("Untitled.lua"));
            }
        #endif
        }
    }
}
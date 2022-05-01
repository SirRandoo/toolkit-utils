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
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class StorePatch
    {
        private static MethodInfo _utilsInjectorMethod;
        private static MethodInfo _injectionSiteMarkerMethod;

        private static bool Prepare()
        {
            _utilsInjectorMethod = AccessTools.Method(typeof(StorePatch), nameof(DrawUtilsContents));
            _injectionSiteMarkerMethod = AccessTools.Method(typeof(Listing), nameof(Listing.Gap));

            return true;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Settings_Store), nameof(Settings_Store.DoWindowContents));
        }

        [ItemNotNull]
        private static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var markerFound = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Ldstr, "Events Edit"))
                {
                    markerFound = true;
                }

                if (markerFound && instruction.opcode == OpCodes.Callvirt && ReferenceEquals(instruction.operand, _injectionSiteMarkerMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, _utilsInjectorMethod);
                }

                yield return instruction;
            }
        }

        [UsedImplicitly]
        private static void DrawUtilsContents([CanBeNull] Listing_Standard optionsListing)
        {
            if (optionsListing == null)
            {
                TkUtils.Logger.Warn("Could not inject Utils' shop buttons into Toolkit's store.  You should report this.");

                return;
            }

            string openText = "TKUtils.Buttons.Open".TranslateSimple();

            DoButtonSeparator(optionsListing);

            if (optionsListing.ButtonTextLabeled($"[ToolkitUtils] {"Traits".TranslateSimple()}", openText))
            {
                Find.WindowStack.Add(new TraitConfigDialog());
            }

            DoButtonSeparator(optionsListing);

            if (optionsListing.ButtonTextLabeled($"[ToolkitUtils] {"Race".TranslateSimple().Pluralize()}", openText))
            {
                Find.WindowStack.Add(new PawnKindConfigDialog());
            }

            DoButtonSeparator(optionsListing);

            if (optionsListing.ButtonTextLabeled($"[ToolkitUtils] {"TKUtils.Editor.Title".TranslateSimple()}", openText))
            {
                Find.WindowStack.Add(new Editor());
            }
        }

        private static void DoButtonSeparator([NotNull] Listing_Standard optionsListing)
        {
            optionsListing.Gap();
            optionsListing.GapLine();
        }
    }
}

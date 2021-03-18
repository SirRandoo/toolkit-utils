using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
using TwitchToolkit.IncidentHelpers.Raids;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

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


namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class IncidentPatches
    {
        private static readonly MethodInfo PurchaseMessageMethod;
        private static readonly MethodInfo ViewerProperty;
        private static readonly FieldInfo UsernameField;
        private static readonly MethodInfo FormatMessageMethod;
        private static readonly MethodInfo FormatMessageMethodAlt;
        private static readonly Type[] AltPatchTypes;

        static IncidentPatches()
        {
            AltPatchTypes = new[] {typeof(DropRaid)};
            FormatMessageMethodAlt = AccessTools.Method(
                typeof(IncidentPatches),
                "FormatMessage",
                new[] {typeof(string), typeof(bool), typeof(string)}
            );
            FormatMessageMethod = AccessTools.Method(
                typeof(IncidentPatches),
                "FormatMessage",
                new[] {typeof(string), typeof(string)}
            );
            UsernameField = AccessTools.Field(typeof(Viewer), "username");
            ViewerProperty = AccessTools.PropertyGetter(typeof(IncidentHelperVariables), "Viewer");
            PurchaseMessageMethod = AccessTools.Method(typeof(VariablesHelpers), "SendPurchaseMessage");
        }

        public static bool Prepare()
        {
            return ModLister.GetActiveModWithIdentifier("brrainz.puppeteer") != null;
        }

        [NotNull]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return DefDatabase<StoreIncidentVariables>.AllDefs
               .Select(incident => incident.incidentHelper.GetMethod("TryExecute"))
               .Where(method => method != null);
        }

        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler(
            [NotNull] IEnumerable<CodeInstruction> instructions,
            MethodBase original
        )
        {
            var transpiler = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (!instruction.Calls(PurchaseMessageMethod))
                {
                    transpiler.Add(instruction);
                    continue;
                }

                transpiler.Remove(transpiler.FindLast(i => i.opcode == OpCodes.Ldc_I4_0));
                transpiler.Add(new CodeInstruction(OpCodes.Ldarg_0));
                transpiler.Add(new CodeInstruction(OpCodes.Call, ViewerProperty));
                transpiler.Add(new CodeInstruction(OpCodes.Ldfld, UsernameField));

                transpiler.Add(
                    AltPatchTypes.Contains(original.DeclaringType)
                        ? new CodeInstruction(OpCodes.Call, FormatMessageMethodAlt)
                        : new CodeInstruction(OpCodes.Call, FormatMessageMethod)
                );

                transpiler.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                transpiler.Add(instruction);
            }

            return transpiler;
        }

        [CanBeNull]
        public static Exception Cleanup([CanBeNull] MethodBase original, [CanBeNull] Exception exception)
        {
            if (original == null || exception == null)
            {
                return null;
            }

            LogHelper.Warn(
                $"Could not patch method {original.DeclaringType?.FullName}.{original.Name}! Reason: {exception.Message}\n\nFull stacktrace: {exception}"
            );
            return null;
        }

        private static string FormatMessage(string message, bool _, string username)
        {
            return FormatMessage(message, username);
        }

        [NotNull]
        private static string FormatMessage([NotNull] string message, string username)
        {
            if (!message.StartsWith($"@{username}") || !message.StartsWith(username))
            {
                return $"@{username} → {message}";
            }

            return message;
        }
    }
}

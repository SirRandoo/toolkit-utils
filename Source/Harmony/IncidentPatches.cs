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

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class IncidentPatches
    {
        private static readonly MethodInfo PurchaseMessageMethod = AccessTools.Method(
            typeof(VariablesHelpers),
            "SendPurchaseMessage"
        );

        private static readonly MethodInfo ViewerProperty = AccessTools.PropertyGetter(
            typeof(IncidentHelperVariables),
            "Viewer"
        );

        private static readonly FieldInfo UsernameField = AccessTools.Field(typeof(Viewer), "username");

        private static readonly MethodInfo FormatMessageMethod = AccessTools.Method(
            typeof(IncidentPatches),
            "FormatMessage",
            new[] {typeof(string), typeof(string)}
        );

        private static readonly MethodInfo FormatMessageMethodAlt = AccessTools.Method(
            typeof(IncidentPatches),
            "FormatMessage",
            new[] {typeof(string), typeof(bool), typeof(string)}
        );

        private static readonly Type[] AltPatchTypes = {typeof(DropRaid)};

        public static IEnumerable<MethodBase> TargetMethods()
        {
            return DefDatabase<StoreIncidentVariables>.AllDefs
               .Select(incident => incident.incidentHelper.GetMethod("TryExecute"))
               .Where(method => method != null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
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

        public static Exception Cleanup(MethodBase original, Exception exception)
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

        private static string FormatMessage(string message, string username)
        {
            if (!message.StartsWith($"@{username}") || !message.StartsWith(username))
            {
                return $"@{username} → {message}";
            }

            return message;
        }
    }
}

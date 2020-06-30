using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    internal static class StoreIncidentWindowVariables
    {
        public static readonly ConstructorInfo OldClassConstructor =
            typeof(StoreIncidentEditor).GetConstructor(new[] {typeof(StoreIncident)});

        public static readonly ConstructorInfo NewClassConstructor =
            typeof(Windows.StoreIncidentEditor).GetConstructor(new[] {typeof(StoreIncident)});

        public static readonly Type OldClassType = typeof(StoreIncidentEditor);
        public static readonly Type NewClassType = typeof(Windows.StoreIncidentEditor);
    }

    [HarmonyPatch(typeof(StoreIncidentsWindow), "DoRow")]
    [UsedImplicitly]
    public static class StoreIncidentsWindowPatch
    {
        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj
                    && instruction.OperandIs(StoreIncidentWindowVariables.OldClassConstructor))
                {
                    instruction.operand = StoreIncidentWindowVariables.NewClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken
                         && instruction.OperandIs(StoreIncidentWindowVariables.OldClassType))
                {
                    instruction.operand = StoreIncidentWindowVariables.NewClassType;
                }

                yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(Window_Trackers), "DoWindowContents")]
    [UsedImplicitly]
    public static class WindowTrackersPatch
    {
        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj
                    && instruction.OperandIs(StoreIncidentWindowVariables.OldClassConstructor))
                {
                    instruction.operand = StoreIncidentWindowVariables.NewClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken
                         && instruction.OperandIs(StoreIncidentWindowVariables.OldClassType))
                {
                    instruction.operand = StoreIncidentWindowVariables.NewClassType;
                }

                yield return instruction;
            }
        }
    }
}

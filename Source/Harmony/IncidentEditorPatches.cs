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
    [HarmonyPatch]
    public static class StoreIncidentsWindowPatch
    {
        private static readonly ConstructorInfo OldClassConstructor = AccessTools.Constructor(
            typeof(StoreIncidentEditor),
            new[] {typeof(StoreIncident)}
        );

        private static readonly ConstructorInfo NewClassConstructor = AccessTools.Constructor(
            typeof(Windows.StoreIncidentEditor),
            new[] {typeof(StoreIncident)}
        );

        private static readonly Type OldClassType = typeof(StoreIncidentEditor);
        private static readonly Type NewClassType = typeof(Windows.StoreIncidentEditor);

        [UsedImplicitly]
        public static IEnumerable<MethodBase> GetTargetMethods()
        {
            yield return AccessTools.Method(typeof(StoreIncidentsWindow), "DoRow");
            yield return AccessTools.Method(typeof(Window_Trackers), "DoWindowContents");
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj && instruction.OperandIs(OldClassConstructor))
                {
                    instruction.operand = NewClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken && instruction.OperandIs(OldClassType))
                {
                    instruction.operand = NewClassType;
                }

                yield return instruction;
            }
        }
    }
}

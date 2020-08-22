using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using TwitchToolkit.Windows;
using StoreIncidentEditor = TwitchToolkit.Windows.StoreIncidentEditor;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    public static class SettingsStorePatch
    {
        private static readonly ConstructorInfo OldClassConstructor =
            typeof(StoreItemsWindow).GetConstructor(new Type[] { });

        private static readonly ConstructorInfo
            NewClassConstructor = typeof(StoreDialog).GetConstructor(new Type[] { });

        private static readonly Type OldClassType = typeof(StoreItemsWindow);
        private static readonly Type NewClassType = typeof(StoreDialog);

        [UsedImplicitly]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Settings_Store), "DoWindowContents");
            yield return AccessTools.Method(typeof(StoreIncidentEditor), "DoWindowContents");
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using TwitchToolkit.Windows;
using StoreIncidentEditor = TwitchToolkit.Windows.StoreIncidentEditor;

namespace SirRandoo.ToolkitUtils.Harmony
{
    internal static class StoreItemsVariables
    {
        public static readonly ConstructorInfo OldClassConstructor =
            typeof(StoreItemsWindow).GetConstructor(new Type[] { });

        public static readonly ConstructorInfo NewClassConstructor = typeof(StoreDialog).GetConstructor(new Type[] { });

        public static readonly Type OldClassType = typeof(StoreItemsWindow);
        public static readonly Type NewClassType = typeof(StoreDialog);
    }

    [HarmonyPatch(typeof(Settings_Store), "DoWindowContents")]
    public static class SettingsStorePatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj
                    && instruction.OperandIs(StoreItemsVariables.OldClassConstructor))
                {
                    instruction.operand = StoreItemsVariables.NewClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken
                         && instruction.OperandIs(StoreItemsVariables.OldClassType))
                {
                    instruction.operand = StoreItemsVariables.NewClassType;
                }

                yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(StoreIncidentEditor), "DoWindowContents")]
    public static class StoreIncidentEditorShopPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj
                    && instruction.OperandIs(StoreItemsVariables.OldClassConstructor))
                {
                    instruction.operand = StoreItemsVariables.NewClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken
                         && instruction.OperandIs(StoreItemsVariables.OldClassType))
                {
                    instruction.operand = StoreItemsVariables.NewClassType;
                }

                yield return instruction;
            }
        }
    }
}

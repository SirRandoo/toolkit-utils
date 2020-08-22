using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit;
using TwitchToolkit.Settings;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Settings_Karma), "DoWindowContents")]
    public static class KarmaMinimumPatch
    {
        private static readonly FieldInfo SettingMarker = AccessTools.Field(typeof(ToolkitSettings), "KarmaMinimum");

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var marker = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldsflda && instruction.OperandIs(SettingMarker))
                {
                    marker = true;
                }

                if (instruction.opcode == OpCodes.Ldc_R4 && marker)
                {
                    instruction.operand = -1E+09f;
                    marker = false;
                }

                yield return instruction;
            }
        }
    }
}

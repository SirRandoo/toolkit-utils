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
    [UsedImplicitly]
    public class KarmaMinimumPatch
    {
        private static readonly FieldInfo settingMarker = AccessTools.Field(typeof(ToolkitSettings), "KarmaMinimum");

        [HarmonyTranspiler]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var marker = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldsflda && instruction.OperandIs(settingMarker))
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

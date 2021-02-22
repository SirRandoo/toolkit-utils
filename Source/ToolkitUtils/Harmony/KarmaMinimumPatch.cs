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
using TwitchToolkit;
using TwitchToolkit.Settings;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Settings_Karma), "DoWindowContents")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class KarmaMinimumPatch
    {
        private static readonly FieldInfo SettingMarker;

        static KarmaMinimumPatch()
        {
            SettingMarker = AccessTools.Field(typeof(ToolkitSettings), "KarmaMinimum");
        }

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

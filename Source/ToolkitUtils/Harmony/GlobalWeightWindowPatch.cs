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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Storytellers.StorytellerPackWindows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class GlobalWeightWindowPatch
    {
        private static ConstructorInfo _oldClassConstructor;
        private static ConstructorInfo _newClassConstructor;
        private static Type _oldClassType;
        private static Type _newClassType;

        public static bool Prepare()
        {
            _newClassType ??= typeof(GlobalWeightDialog);
            _oldClassType ??= typeof(Window_GlobalVoteWeights);
            _newClassConstructor ??= AccessTools.Constructor(typeof(GlobalWeightDialog), Type.EmptyTypes);
            _oldClassConstructor ??= AccessTools.Constructor(typeof(Window_GlobalVoteWeights), Type.EmptyTypes);

            return true;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Window_ToryTalkerSettings), "DoWindowContents");
            yield return AccessTools.Method(typeof(Window_StorytellerPacks), "DoWindowContents");
        }

        [ItemNotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Newobj && instruction.OperandIs(_oldClassConstructor))
                {
                    instruction.operand = _newClassConstructor;
                }
                else if (instruction.opcode == OpCodes.Ldtoken && instruction.OperandIs(_oldClassType))
                {
                    instruction.operand = _newClassType;
                }

                yield return instruction;
            }
        }
    }
}

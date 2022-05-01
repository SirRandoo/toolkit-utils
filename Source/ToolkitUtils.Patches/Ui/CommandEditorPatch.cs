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
using TwitchToolkit;
using TwitchToolkit.Windows;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class CommandEditorPatch
    {
        private static ConstructorInfo _oldClassConstructor;
        private static ConstructorInfo _newClassConstructor;
        private static Type _oldClassType;
        private static Type _newClassType;

        private static bool Prepare()
        {
            _newClassType ??= typeof(CommandEditorDialog);
            _oldClassType ??= typeof(Window_CommandEditor);
            _newClassConstructor ??= typeof(CommandEditorDialog).GetConstructor(new[] { typeof(Command) });
            _oldClassConstructor ??= typeof(Window_CommandEditor).GetConstructor(new[] { typeof(Command) });

            return true;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Window_Commands), "DoRow");
            yield return AccessTools.Method(typeof(Window_NewCustomCommand), "TrySubmitNewCommand");
        }

        [ItemNotNull]
        private static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Newobj, _oldClassConstructor))
                {
                    instruction.operand = _newClassConstructor;
                }
                else if (instruction.Is(OpCodes.Ldtoken, _oldClassType))
                {
                    instruction.operand = _newClassType;
                }

                yield return instruction;
            }
        }
    }
}

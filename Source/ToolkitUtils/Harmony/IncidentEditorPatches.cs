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
using TwitchToolkit.Incidents;
using TwitchToolkit.Windows;
using StoreIncidentEditor = SirRandoo.ToolkitUtils.Windows.StoreIncidentEditor;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class StoreIncidentsWindowPatch
    {
        private static ConstructorInfo _oldClassConstructor;
        private static ConstructorInfo _newClassConstructor;
        private static Type _oldClassType;
        private static Type _newClassType;

        public static bool Prepare()
        {
            _newClassType ??= typeof(StoreIncidentEditor);
            _oldClassType ??= typeof(TwitchToolkit.Windows.StoreIncidentEditor);
            _newClassConstructor ??= AccessTools.Constructor(
                typeof(StoreIncidentEditor),
                new[] { typeof(StoreIncident) }
            );
            _oldClassConstructor ??= AccessTools.Constructor(
                typeof(TwitchToolkit.Windows.StoreIncidentEditor),
                new[] { typeof(StoreIncident) }
            );
            return true;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(StoreIncidentsWindow), "DoRow");
            yield return AccessTools.Method(typeof(Window_Trackers), "DoWindowContents");
        }

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

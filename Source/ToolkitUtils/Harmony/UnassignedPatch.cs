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
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class UnassignedPatch
    {
        private static MethodInfo _pawnHistoryRemove;
        private static FieldInfo _pawnHistoryField;
        private static MethodInfo _renameAndRemoveMethod;
        private static FieldInfo _viewerComponentField;

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Window_Viewers), "DoWindowContents");
        }

        public static bool Prepare()
        {
            _viewerComponentField = AccessTools.Field(typeof(Window_Viewers), "component");
            _renameAndRemoveMethod = AccessTools.Method(typeof(UnassignedPatch), "RenameAndRemove");
            _pawnHistoryField = AccessTools.Field(typeof(GameComponentPawns), "pawnHistory");
            _pawnHistoryRemove = AccessTools.Method(typeof(Dictionary<string, Pawn>), "Remove", new[] { typeof(string) });

            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var methodFound = false;
            var componentFound = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(_viewerComponentField))
                {
                    componentFound = true;
                }

                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(_pawnHistoryField) && componentFound)
                {
                    instruction.opcode = OpCodes.Nop;
                    componentFound = false;
                }

                if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(_pawnHistoryRemove))
                {
                    instruction.operand = _renameAndRemoveMethod;
                    methodFound = true;
                }

                if (instruction.opcode == OpCodes.Pop && methodFound)
                {
                    instruction.opcode = OpCodes.Nop;
                    methodFound = false;
                }

                yield return instruction;
            }
        }

        [UsedImplicitly]
        private static void RenameAndRemove([CanBeNull] GameComponentPawns component, [CanBeNull] string username)
        {
            if (username == null || component == null)
            {
                return;
            }

            Pawn pawn = component.PawnAssignedToUser(username);

            if (pawn?.Name is NameTriple name)
            {
                pawn.Name = new NameTriple(name.First, name.Last, name.Last);
            }

            component.pawnHistory.Remove(username);
        }
    }
}

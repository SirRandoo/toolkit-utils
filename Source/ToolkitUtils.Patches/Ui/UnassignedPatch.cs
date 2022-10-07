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
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    /// <summary>
    ///     A Harmony patch for removing a viewer's name from a pawn when the
    ///     streamer unassigns their pawn from the viewers dialog.
    /// </summary>
    /// <remarks>
    ///     Prior to this, Utils would automatically reassign the pawn to the
    ///     viewer as the pawn was still named after the viewer.
    /// </remarks>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class UnassignedPatch
    {
        private static MethodInfo _pawnHistoryRemove;
        private static FieldInfo _pawnHistoryField;
        private static MethodInfo _renameAndRemoveMethod;
        private static FieldInfo _viewerComponentField;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Window_Viewers), nameof(Window_Viewers.DoWindowContents));
        }

        [CanBeNull]
        private static Exception Cleanup(MethodBase original, [CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

            return null;
        }

        private static bool Prepare()
        {
            _viewerComponentField = AccessTools.Field(typeof(Window_Viewers), "component");
            _renameAndRemoveMethod = AccessTools.Method(typeof(UnassignedPatch), nameof(RenameAndRemove));
            _pawnHistoryField = AccessTools.Field(typeof(GameComponentPawns), nameof(GameComponentPawns.pawnHistory));
            _pawnHistoryRemove = AccessTools.Method(typeof(Dictionary<string, Pawn>), nameof(Dictionary<string, Pawn>.Remove), new[] { typeof(string) });

            return true;
        }

        [ItemNotNull]
        private static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var methodFound = false;
            var componentFound = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Ldfld, _viewerComponentField))
                {
                    componentFound = true;
                }

                if (instruction.Is(OpCodes.Ldfld, _pawnHistoryField) && componentFound)
                {
                    instruction.opcode = OpCodes.Nop;
                    componentFound = false;
                }

                if (instruction.Is(OpCodes.Callvirt, _pawnHistoryRemove))
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

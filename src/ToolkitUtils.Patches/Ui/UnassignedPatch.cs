// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Patches. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Api;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Windows;
using Verse;

namespace ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for removing a viewer's name from a pawn when the streamer unassigns their pawn from the
///     viewers dialog.
/// </summary>
/// <remarks>
///     Prior to this, Utils would automatically reassign the pawn to the viewer as the pawn was still named after the
///     viewer.
/// </remarks>
[HarmonyPatch]
[SuppressMessage(category: "csharpsquid", checkId: "S1144")]
[SuppressMessage(category: "csharpsquid", checkId: "S3400")]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global")]
[SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
internal static class UnassignedPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(UnassignedPatch));

    private static readonly MethodInfo _pawnHistoryRemove = AccessTools.Method(typeof(Dictionary<string, Pawn>), nameof(Dictionary<string, Pawn>.Remove), [typeof(string),]);

    private static readonly FieldInfo _pawnHistoryField = AccessTools.Field(typeof(GameComponentPawns), nameof(GameComponentPawns.pawnHistory));
    private static readonly MethodInfo _renameAndRemoveMethod = AccessTools.Method(typeof(UnassignedPatch), nameof(RenameAndRemove));
    private static readonly FieldInfo _viewerComponentField = AccessTools.Field(typeof(Window_Viewers), name: "component");

    private static readonly MethodInfo ViewerWindowContentsMethod = AccessTools.Method(typeof(Window_Viewers), nameof(Window_Viewers.DoWindowContents));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return ViewerWindowContentsMethod;
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception = null)
    {
        if (exception == null) return null;

        Logger.Error(
            exception,
            message: "Could not patch {Method} :: Unassigned pawns will still have the viewer's name, which will cause the pawn to be reassigned to the viewer",
            original.FullDescription()
        );

        return null;
    }

    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var methodFound = false;
        var componentFound = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Is(OpCodes.Ldfld, _viewerComponentField)) componentFound = true;

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
    private static void RenameAndRemove(GameComponentPawns? component, string? username)
    {
        if (username == null || component == null) return;

        Pawn pawn = component.PawnAssignedToUser(username);

        if (pawn?.Name is NameTriple name) pawn.Name = new NameTriple(name.First, name.Last, name.Last);

        component.pawnHistory.Remove(username);
    }
}

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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;
using TwitchToolkit.Settings;
using TwitchToolkit.Windows;

namespace ToolkitUtils.Mod.Patching;

[PublicAPI]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class ItemStoreReplacementPatch
{
    private static readonly ConstructorInfo OriginalItemStoreConstructor = AccessTools.Constructor(typeof(StoreItemsWindow));
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Settings_Store), nameof(Settings_Store.DoWindowContents));
        yield return AccessTools.Method(typeof(StoreIncidentEditor), nameof(StoreIncidentEditor.DoWindowContents));
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (ReplacementPatchHelper.IsConstructorInvocation(instruction, OriginalItemStoreConstructor)) instruction.operand = null; // FIXME

            yield return instruction;
        }
    }
}

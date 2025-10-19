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
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Api;
using TwitchToolkit.Settings;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Windows;

namespace ToolkitUtils.Patches;

/// <summary>
///     A common patch that replaces constructor calls in the given set of methods through a "prefix" patch via
///     Harmony, the patching library used by RimWorld modders.
/// </summary>
/// <remarks>
///     This patch serves as a generic patch class for consolidating all other "constructor replacement" patches within the
///     mod. The list of replacements done by this class include:
///     <ul>
///         <li>Twitch Toolkit's "command editor" dialog</li> <li>Twitch Toolkit's "incident editor" dialog</li>
///         <li>Twitch Toolkit's "global weights" dialog</li> <li>Twitch Toolkit's "item store" dialog</li>
///     </ul>
/// </remarks>
/// <inheritdoc cref="DisablerPatch" path="/remarks[@id='patch']" />
[HarmonyPatch]
internal static class ReplacingPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(ReplacingPatch));

    private static readonly Type[] EmptyTypes = [];
    private static readonly Dictionary<Type, Type> TypeReplacements = new();
    private static readonly Dictionary<ConstructorInfo, ConstructorInfo> ConstructorReplacements = new();

    [UsedImplicitly]
    private static bool Prepare()
    {
        // TODO: Populate this with constructor replacements.
        ConstructorInfo storeItemsWindowConstructor = AccessTools.Constructor(typeof(StoreItemsWindow), EmptyTypes);

        return true;
    }

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // Known methods that use Twitch Toolkit's item store dialog.
        yield return AccessTools.Method(typeof(Settings_Store), nameof(Settings_Store.DoWindowContents));
        yield return AccessTools.Method(typeof(StoreIncidentEditor), nameof(StoreIncidentEditor.DoWindowContents));

        // Known methods that use Twitch Toolkit's command editor dialog.
        yield return AccessTools.Method(typeof(Window_Commands), name: "DoRow");
        yield return AccessTools.Method(typeof(Window_NewCustomCommand), name: "TrySubmitNewCommand");

        // Known methods that use Twitch Toolkit's global weight dialog.
        yield return AccessTools.Method(typeof(Window_ToryTalkerSettings), nameof(Window_ToryTalkerSettings.DoWindowContents));
        yield return AccessTools.Method(typeof(Window_StorytellerPacks), nameof(Window_StorytellerPacks.DoWindowContents));

        // Known methods that use Twitch Toolkit's incident dialog.
        yield return AccessTools.Method(typeof(StoreIncidentsWindow), name: "DoRow");
        yield return AccessTools.Method(typeof(Window_Trackers), nameof(Window_Trackers.DoWindowContents));
    }

    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Newobj && ConstructorReplacements.TryGetValue((ConstructorInfo)instruction.operand, out ConstructorInfo constructorInfo))
                instruction.operand = constructorInfo;
            else if (instruction.opcode == OpCodes.Ldtoken && TypeReplacements.TryGetValue((Type)instruction.operand, out Type replacementType))
                instruction.operand = replacementType;

            yield return instruction;
        }
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception = null)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not patch {Method} :: One or more menus will be vanilla Twitch Toolkit menus", original.FullDescription());

        return null;
    }
}

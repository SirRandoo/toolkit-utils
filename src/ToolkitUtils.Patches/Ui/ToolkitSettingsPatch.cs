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
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitUtils.Api;
using UnityEngine;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

/// <summary>A Harmony patch for changing Twitch Toolkit's settings menu to be its old, classic style.</summary>
[HarmonyPatch]
[SuppressMessage(category: "csharpsquid", checkId: "S1144")]
[SuppressMessage(category: "csharpsquid", checkId: "S3400")]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global")]
[SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
internal static class ToolkitSettingsPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(ToolkitSettingsPatch));

    private static readonly MethodInfo ToolkitSettingsWindowContentsMethod = AccessTools.Method(
        typeof(TwitchToolkit.TwitchToolkit),
        nameof(TwitchToolkit.TwitchToolkit.DoSettingsWindowContents)
    );

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return ToolkitSettingsWindowContentsMethod;
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception = null)
    {
        if (exception == null) return null;

        Logger.Error(
            exception,
            message:
            "Could not patch {Method} :: You won't be taken to ToolkitUtils' reimplementation of Twitch Toolkit's settings menu when accessed from the game's options menu",
            original.FullDescription()
        );

        return null;
    }

    [UsedImplicitly]
    private static bool Prefix(Rect inRect) =>

        // TODO: Implement Twitch Toolkit's settings window.
        // ToolkitSettingsWorker.Draw(inRect);
        false;
}

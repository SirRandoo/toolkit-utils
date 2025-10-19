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
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitUtils.Api;
using TwitchToolkit;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for disabling the karma bump from <see cref="KarmaType.Neutral" /> karma type purchases, if
///     the relevant setting is enabled.
/// </summary>
/// <inheritdoc cref="DisablerPatch" path="/remarks[@id='patch']" />
[HarmonyPatch]
[UsedImplicitly]
internal static class TrueNeutralPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(TrueNeutralPatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Viewer), nameof(Viewer.CalculateNewKarma));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not apply 'true neutral' patch :: You can expect Neutral karma type purchases to provide a small bump in a viewer's karma.");

        return null;
    }

    [UsedImplicitly]
    private static bool Prefix(KarmaType karmaType) => // !SettingsRegistry.Shop.Settings.TrueNeutral ||
        karmaType != KarmaType.Neutral;
}

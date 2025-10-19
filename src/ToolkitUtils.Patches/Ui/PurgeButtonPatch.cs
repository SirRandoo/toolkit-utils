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
using ToolkitUtils.Mod.Localization;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for inserting a "Purge" button in the Viewers dialog. The purge button is responsible for
///     opening the <see cref="PurgeViewersDialog" />.
/// </summary>
[HarmonyPatch]
[UsedImplicitly]
internal static class PurgeButtonPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(PurgeButtonPatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Window_Viewers), nameof(Window_Viewers.DoWindowContents));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not apply 'purge button' patch :: You can expect the 'purge' button from the viewers window to be missing.");

        return null;
    }

    [UsedImplicitly]
    private static void Postfix(Rect inRect)
    {
        var canvas = new Rect(inRect.width - 60f, y: 0f, width: 60f, height: 28f);

        if (Widgets.ButtonText(canvas, TranslationService.Instance.GetTranslation("TKUtils.Buttons.Purge")))
        {
            // TODO: Implement viewer purge dialog.
            // Find.WindowStack.Add(new PurgeViewersDialog());
        }
    }
}

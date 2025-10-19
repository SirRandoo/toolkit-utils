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
using RimWorld;
using ToolkitUtils.Api;
using UnityEngine;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

[HarmonyPatch]
[SuppressMessage(category: "csharpsquid", checkId: "S1144")]
[SuppressMessage(category: "csharpsquid", checkId: "S3400")]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global")]
[SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
internal static class StorytellerInterfacePatch
{
    private const float ButtonHeight = Text.SmallFontHeight * 1.25f;
    private const float ButtonWidth = 290f;
    private const float ButtonMargin = 24f;

    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(StorytellerInterfacePatch));

    private static readonly MethodInfo StorytellerSelectionInterfaceMethod = AccessTools.Method(typeof(StorytellerUI), nameof(StorytellerUI.DrawStorytellerSelectionInterface));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return StorytellerSelectionInterfaceMethod;
    }

    [UsedImplicitly]
    private static void Postfix(Rect rect, ref StorytellerDef? chosenStoryteller)
    {
        // TODO: This patch doesn't translate "Storyteller Packs"

        if (chosenStoryteller == null || !string.Equals(chosenStoryteller.defName, b: "StorytellerPacks", StringComparison.Ordinal)) return;

        int height = Mathf.FloorToInt(f: ButtonHeight);
        var btnRegion = new Rect(Storyteller.PortraitSizeTiny.x + ButtonMargin, rect.height, ButtonWidth, height);

        if (Widgets.ButtonText(btnRegion, label: "Storyteller Packs"))
        {
            // TODO: Implement the storyteller pack dialog.
            // Find.WindowStack.Add(new StorytellerPackDialog());
        }
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception = null)
    {
        if (exception == null) return null;

        Logger.Error(
            exception,
            message: "Could not patch {Method} :: The 'Storyteller Packs' button in the storyteller menu will clash with other UI elements",
            original.FullDescription()
        );

        return null;
    }
}

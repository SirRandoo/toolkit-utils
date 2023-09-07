// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches;

[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class StorytellerInterfacePatch
{
    private static readonly MethodBase DrawMethod;

    static StorytellerInterfacePatch()
    {
        DrawMethod = AccessTools.Method(typeof(StorytellerUI), nameof(StorytellerUI.DrawStorytellerSelectionInterface));
    }

    private static bool Prepare()
    {
        if (CompatRegistry.ToolkitCompatible)
        {
            return false;
        }

        PatchRunner.Harmony.Unpatch(DrawMethod, HarmonyPatchType.Postfix, "com.github.harmony.rimworld.mod.twitchtoolkit");

        return true;
    }

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return DrawMethod;
    }

    private static void Postfix(Rect rect, ref StorytellerDef? chosenStoryteller)
    {
        if (chosenStoryteller == null || !string.Equals(chosenStoryteller.defName, "StorytellerPacks", StringComparison.Ordinal))
        {
            return;
        }

        int height = Mathf.FloorToInt(Text.SmallFontHeight * 1.25f);
        var btnRegion = new Rect(Storyteller.PortraitSizeTiny.x + 24f, rect.height, 290f, height);

        if (Widgets.ButtonText(btnRegion, "Storyteller Packs"))
        {
            Find.WindowStack.Add(new StorytellerPackDialog());
        }
    }
}
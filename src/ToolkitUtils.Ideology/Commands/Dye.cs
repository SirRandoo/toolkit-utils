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
// with ToolkitUtils.Ideology. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Ideology.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Dye(ExecutionContext context, TranslationService translationService) : CommandGroup
{
    /// <summary>Changes the color of apparel items for a specific pawn based on the given dye targets.</summary>
    /// <param name="dyeTargets">An array of strings representing the apparel items and their respective colors to dye.</param>
    /// <returns>A task returning an <see cref="Remora.Results.IResult" /> indicating the success or failure of the operation.</returns>
    [Command("dye")]
    public async Task<Result> DyeApparelAsync(params string[] dyeTargets)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(translationService.GetPawnRequiredError(context.Invoker));

        List<DyeTarget> targets = await ParseColorsAsync(pawn, dyeTargets);

        for (var index = 0; index < targets.Count; index++)
        {
            DyeTarget? target = targets[index];

            if (target == null) continue;

            await MainThreadExtensions.OnMainAsync(DyeApparel, target.Apparel, target.Color);
        }

        return await context.SendReplyAsync(translationService.GetTranslation("TKUtils.Responses.ApparelDyed"));
    }

    private static async Task<List<DyeTarget>> ParseColorsAsync(Pawn? pawn, params string[] dyeTargets)
    {
        var container = new List<DyeTarget>();

        if (pawn == null) return [];

        for (var index = 0; index < dyeTargets.Length; index++)
        {
            string argument = dyeTargets[index];
            int equalsIndex = argument.IndexOf('=');

            if (argument.IndexOf('=') <= -1) continue;

            string target = argument[..equalsIndex];
            string color = argument[(equalsIndex + 1)..];

            DyeTarget? dyeTarget = await ParseDyeTargetAsync(pawn, target, color);

            if (dyeTarget == null) continue;

            container.Add(dyeTarget);
        }

        return container;
    }

    private static ValueTask<DyeTarget?> ParseDyeTargetAsync(Pawn pawn, string target, string hexColor)
    {
        Color? color;
        List<Apparel?> apparel = pawn.apparel.WornApparel;
        Apparel? clothing = apparel.Find(a => string.Equals(a!.def.label, target, StringComparison.InvariantCultureIgnoreCase));

        if (clothing is null) return new ValueTask<DyeTarget?>((DyeTarget?)null);

        if (!ColorUtility.TryParseHtmlString(hexColor, out Color parsed))
            color = null;
        else
            color = parsed;

        return new ValueTask<DyeTarget?>(new DyeTarget(color, clothing));
    }

    private static void DyeApparel(Apparel apparel, Color? color)
    {
        if (color.HasValue)
        {
            apparel.TryGetComp<CompColorable>()?.SetColor(color.Value);

            return;
        }

        if (apparel.Wearer?.story.favoriteColor == null) return;

        apparel.TryGetComp<CompColorable>()?.SetColor(apparel.Wearer.story.favoriteColor.color);
    }

    private sealed record DyeTarget(Color? Color, Apparel Apparel);
}

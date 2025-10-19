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

/// <summary>Provides a command to set the favorite color for a user's associated pawn.</summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class SetFavoriteColor(ExecutionContext context, TranslationService translationService) : CommandGroup
{
    /// <summary>Sets the favorite color for the invoking user's associated pawn asynchronously.</summary>
    /// <param name="color">The new favorite color to set for the pawn.</param>
    /// <returns>A result indicating success or failure of the operation.</returns>
    [Command("setfavoritecolor")]
    public async Task<Result> SetFavoriteColorAsync(Color color)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(translationService.GetPawnRequiredError(context.Invoker));

        await MainThreadExtensions.OnMainAsync(SetFavoriteColorInternal, pawn, color);

        return await context.SendReplyAsync(translationService.GetTranslation("TKUtils.Responses.FavoriteColorChanged"));
    }

    private static void SetFavoriteColorInternal(Pawn pawn, Color color)
    {
        pawn.story.favoriteColor = new ColorDef
        {
            defName = "Color_" + color, label = color.ToString(), color = color,
        };
    }
}

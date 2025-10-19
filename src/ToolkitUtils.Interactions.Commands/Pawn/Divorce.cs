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
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Commands;

[PublicAPI]
public sealed class Divorce(ExecutionContext context) : CommandGroup
{
    [Command("divorce")]
    public async Task<Result> DivorceAsync(Viewer viewer)
    {
        Pawn? invokerPawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (invokerPawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn? spousePawn = ViewerPawnRegistry.Get(viewer.Id);

        if (spousePawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredOther(viewer));

        List<Pawn> spouses = await MainThreadExtensions.OnMainAsync(SpouseRelationUtility.GetSpouses, invokerPawn, arg2: false);

        if (spouses.Count <= 0) return Result.Fail(TranslationService.Instance.GetTranslation("TKUtils.Errors.NoSpouses"));

        if (spouses.Find(s => s == spousePawn) == null)
        {
            return Result.Fail(
                TranslationService.Instance.GetTranslation("TKUtils.Errors.NotMarried.Other").Format(
                    new
                    {
                        ViewerName = viewer.Name,
                    }
                )
            );
        }

        await MainThreadExtensions.OnMainAsync(SpouseRelationUtility.DoDivorce, invokerPawn, spousePawn);

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Divorce.Finalized").Format(
                new
                {
                    ExName = viewer.Name,
                }
            )
        );
    }
}

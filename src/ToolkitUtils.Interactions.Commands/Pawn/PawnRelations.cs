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
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnRelations(RelationSettings settings, ExecutionContext context) : CommandGroup
{
    [Command("relations")]
    public async Task<Result> GetPawnRelationshipsAsync(Viewer? query)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
        if (query == null) return await ExecuteBroadRelation(pawn);

        Pawn? queriedPawn = ViewerPawnRegistry.Get(query.Id);

        if (queriedPawn == null) return await ExecuteBroadRelation(pawn);

        return Result.Ok(await GetRelationshipAsync(pawn, queriedPawn));
    }

    private async Task<Result> ExecuteBroadRelation(Pawn pawn)
    {
        List<Pawn> pawns = await RouterService.Instance.RouteToMainAsync(Find.ColonistBar.GetColonistsInOrder);

        if (pawns.Count <= 0) return Result.Fail(TranslationService.Instance.GetNoGameError());

        var builder = new StringBuilder();

        for (var i = 0; i < pawns.Count; i++)
        {
            Pawn p = pawns[i];

            builder.Append(p.LabelShort);
            builder.Append(": ");
            builder.Append(await GetRelationshipAsync(pawn, pawns[i]));
            builder.Append(", ");
        }

        return Result.Ok(builder.ToString(startIndex: 0, builder.Length - 2));
    }

    private async Task<string> GetRelationshipAsync(Pawn pawn, Pawn targetPawn)
    {
        int theirOpinion = await RouterService.Instance.RouteToMainAsync(pawn.relations.OpinionOf, targetPawn);
        int myOpinion = await RouterService.Instance.RouteToMainAsync(targetPawn.relations.OpinionOf, pawn);

        string? relationship = await GetRelationshipLabelAsync(pawn, targetPawn, myOpinion, overrideSettings: true);

        return string.IsNullOrEmpty(relationship)
            ? $"{myOpinion.ToStringWithSign()} ({targetPawn.LabelShort}), ${theirOpinion.ToStringWithSign()} ({pawn.LabelShort})"
            : $"{relationship} | {myOpinion.ToStringWithSign()} ({targetPawn.LabelShort}), ${theirOpinion.ToStringWithSign()} ({pawn.LabelShort})";
    }

    private async Task<string?> GetRelationshipLabelAsync(Pawn pawn, Pawn otherPawn, int opinion, bool overrideSettings = false)
    {
        PawnRelationDef relations = await RouterService.Instance.RouteToMainAsync(pawn.GetMostImportantRelation, otherPawn);

        if (relations != null) return await RouterService.Instance.RouteToMainAsync(relations.GetGenderSpecificLabelCap, otherPawn);
        if (!overrideSettings && settings.UseMinimalRelations) return null;
        if (opinion < -20) return TranslationService.Instance.GetTranslation("Rival");

        return opinion > 20 ? TranslationService.Instance.GetTranslation("Friend") : TranslationService.Instance.GetTranslation("Acquaintance");
    }
}

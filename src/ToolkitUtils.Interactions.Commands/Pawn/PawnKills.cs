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
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PawnKills(ExecutionContext context) : CommandGroup
{
    [Command("mypawnkills")]
    public async Task<IResult> RunCommand()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        int totalKills = await RouterService.Instance.RouteToMainAsync(pawn.records.GetAsInt, RecordDefOf.Kills);
        int animalKills = await RouterService.Instance.RouteToMainAsync(pawn.records.GetAsInt, RecordDefOf.KillsAnimals);
        int humanLikeKills = await RouterService.Instance.RouteToMainAsync(pawn.records.GetAsInt, RecordDefOf.KillsHumanlikes);
        int mechanoidKills = await RouterService.Instance.RouteToMainAsync(pawn.records.GetAsInt, RecordDefOf.KillsMechanoids);
        int entityKills = await RouterService.Instance.RouteToMainAsync(pawn.records.GetAsInt, RecordDefOf.KillsEntities);

        var builder = new StringBuilder();
        builder.Append(TranslationService.Instance.GetTranslation(RecordDefOf.Kills.defName, DefTranslationKey.Label).CapitalizeFirst());
        builder.Append(": ");
        builder.Append(totalKills.ToString("N0"));
        builder.Append(" | ");
        builder.Append(TranslationService.Instance.GetTranslation(RecordDefOf.KillsHumanlikes.defName, DefTranslationKey.Label).CapitalizeFirst());
        builder.Append(": ");
        builder.Append(humanLikeKills.ToString("N0"));
        builder.Append(", ");
        builder.Append(TranslationService.Instance.GetTranslation(RecordDefOf.KillsAnimals.defName, DefTranslationKey.Label).CapitalizeFirst());
        builder.Append(": ");
        builder.Append(animalKills.ToString("N0"));
        builder.Append(", ");
        builder.Append(TranslationService.Instance.GetTranslation(RecordDefOf.KillsMechanoids.defName, DefTranslationKey.Label).CapitalizeFirst());
        builder.Append(": ");
        builder.Append(mechanoidKills.ToString("N0"));
        builder.Append(", ");
        builder.Append(TranslationService.Instance.GetTranslation(RecordDefOf.KillsEntities.defName, DefTranslationKey.Label).CapitalizeFirst());
        builder.Append(entityKills.ToString("N0"));

        return await context.SendReplyAsync(builder.ToString());
    }
}

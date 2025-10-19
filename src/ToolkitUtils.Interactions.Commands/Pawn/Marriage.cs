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
using ToolkitUtils.Interactions.Commands.Components;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Marriage(ExecutionContext context, CommandSettings commandSettings) : CommandGroup
{
    private const string MarriageCommandText = "marry";

    [Command(MarriageCommandText)]
    public async Task<Result> ProposeAsync(Viewer viewer)
    {
        if (Current.Game == null) return Result.Fail(TranslationService.Instance.GetNoGameError());

        Pawn? askerPawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (askerPawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        Pawn? askeePawn = ViewerPawnRegistry.Get(viewer.Id);

        if (askeePawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredOther(viewer));

        List<Pawn> askerSpouses = await MainThreadExtensions.OnMainAsync(SpouseRelationUtility.GetSpouses, askerPawn, arg2: false);

        for (var i = 0; i < askerSpouses.Count; i++)
            if (askerSpouses[i].Equals(askeePawn))
                return Result.Fail(TranslationService.Instance.GetTranslation("TKUtils.Errors.AlreadyMarried"));

        if (!await MainThreadExtensions.OnMainAsync(HasOpenSpouseSlot, askerPawn))
            return Result.Fail(TranslationService.Instance.FormatMarriageForbiddenByBeliefs(context.Invoker));

        if (!await MainThreadExtensions.OnMainAsync(HasOpenSpouseSlot, askeePawn)) return Result.Fail(TranslationService.Instance.FormatMarriageForbiddenByBeliefs(viewer));

        var component = Current.Game.GetComponent<MarriageGameComponent>();

        Result<MarriageGameComponent.FiancePair> pair = component.HasPendingProposal(context.Invoker.Id);

        if (pair.IsSuccess)
        {
            await MainThreadExtensions.OnMainAsync(MarriageCeremonyUtility.Married, askerPawn, askeePawn);

            return Result.Ok(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Marriage.Wedded").Format(
                    new
                    {
                        SpouseName = viewer.Name,
                    }
                )
            );
        }

        component.Propose(context.Invoker.Id, viewer.Id);

        return Result.Ok(
            string.Format(
                format: "@{0}:{1}",
                viewer.Name,
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Marriage.Proposal").Format(
                    new
                    {
                        ProposerName = context.Invoker.Name, commandSettings.CommandPrefix, MarriageCommand = MarriageCommandText,
                    }
                )
            )
        );
    }

    private bool HasOpenSpouseSlot(Pawn pawn) => pawn.GetSpouseCount(false) <= 0;
}

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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using RimWorld.Planet;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Logging;
using ToolkitUtils.Mod.Services;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnLeave(LeaveSettings settings, ExecutionContext context, ILetterService letterService) : CommandGroup
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    [Command("leave")]
    public async Task<Result> RunCommand()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
        if (await MainThreadExtensions.OnMainAsync(CaravanUtility.IsCaravanMember, pawn))
            return Result.Ok(TranslationService.Instance.GetTranslation("TKUtils.Errors.CannotLeaveWhileInCaravan"));

        Current.Game.GetComponent<GameComponentPawns>();

        IPawnProvider? provider = Registries.Compatibilities.GetPawnProvider("com.sirrandoo.tkutils:magic.compatibility.pawn");

        // TODO: This branch will be entered when A RimWorld of Magic isn't loaded.
        //       This should be handled better.
        if (provider == null)
        {
            Logger.Warn("No provider found for Magic compatibility.");

            return Result.Fail();
        }

        Result isValidPawn = provider.IsValidPawnCandidate(pawn);

        if (isValidPawn is { IsSuccess: true, })
        {
            // TODO: Send a message indicating the request was blocked.
            ViewerPawnRegistry.Unregister(context.Invoker.Id);

            pawn.Name = pawn.Name switch
            {
                NameTriple name => new NameTriple(name.First, name.Last, name.Last),
                NameSingle      => new NameSingle($"{pawn.def.label}{ThreadSafeRandom.Next()}"),
                var _           => pawn.Name,
            };

            return Result.Fail(new Translation("pawn undead".MarkNotTranslated()));
        }

        await ForceLeaveAsync(pawn);
        ViewerPawnRegistry.Unregister(context.Invoker.Id);

        return Result.Ok();
    }

    private async Task ForceLeaveAsync(Pawn pawn)
    {
        var component = Current.Game.GetComponent<GameComponentPawns>();
        string? user = component.UserAssignedToPawn(pawn);
        if (!string.IsNullOrEmpty(user)) component.pawnHistory.Remove(user);

        if (settings.LeaveMethod == LeaveMethod.Dust
         && FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Ash, pawn.LabelShortCap, Mathf.CeilToInt(pawn.BodySize * 0.6f)))
        {
            await context.SendReplyAsync(TranslationService.Instance.GetTranslation("TKUtils.Responses.PawnLeave.Thanos"));

            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.PawnLeave.Thanos.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.PawnLeave.Thanos.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name,
                    }
                ),
                new LookTargets(pawn.Position, pawn.Map)
            );

            await RouterService.Instance.RouteToMainAsync(pawn.Destroy, DestroyMode.Vanish);
        }
        else
        {
            if (settings.LeaveMethod == LeaveMethod.Drop && pawn.AnythingToStrip()) pawn.Strip();

            await context.SendReplyAsync(TranslationService.Instance.GetTranslation("TKUtils.Responses.PawnLeave.Generic"));

            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.PawnLeave.Generic.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.PawnLeave.Generic.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name,
                    }
                ),
                new LookTargets(pawn.Position, pawn.Map)
            );

            if (pawn.Faction != null) await RouterService.Instance.RouteToMainAsync(pawn.SetFaction, (Faction?)null, (Pawn?)null);

            await RouterService.Instance.RouteToMainAsync(pawn.jobs.StopAll, arg1: false, arg2: true);
            await RouterService.Instance.RouteToMainAsync(pawn.health.surgeryBills.Clear);
        }
    }
}

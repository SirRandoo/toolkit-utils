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
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Extensions;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Logging;
using ToolkitUtils.Mod.Services;
using Verse;
using Result = ToolkitUtils.Mod.Result;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class ReviveBuyGroup : CommandGroup
{
    [Group("revive")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class Revive(ExecutionContext context, ILetterService letterService, IResurrectionService resurrectionService) : CommandGroup
    {
        private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

        [Command("self")]
        public async Task<Result> ReviveSelfAsync()
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
            if (pawn is not { Dead: true, }) return Result.Fail(TranslationService.Instance.GetNotDeadError(pawn));

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Revive.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.FormatInsufficientBalance(StoreIncidentDefOfs.Revive.cost, context.Invoker.Coins));

            Result resurrected = await resurrectionService.ResurrectWithSideEffectsAsync(pawn);

            if (!resurrected.IsSuccess)
            {
                context.Invoker.AbortTransaction(transaction.Value);

                return resurrected;
            }

            transaction.Value.PostTransaction();
            await letterService.SendPositiveLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Singular.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Self.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name,
                    }
                ),
                pawn
            );

            return Result.Ok(TranslationService.Instance.GetTranslation("TKUtils.Responses.Revive.Self.Complete"));
        }

        [Command("all")]
        public async Task<Result> ReviveAllAsync()
        {
            List<Pawn> candidates = Find.Maps.AsParallel().SelectMany(m => m.mapPawns.AllPawns).Where(p => p.IsColonist && p is { IsPlayerControlled: true, Dead: true, }).ToList();

            if (candidates.Count <= 0) return Result.Fail(new Translation("No one's dead".MarkNotTranslated()));

            int revivalCost = StoreIncidentDefOfs.Revive.cost * candidates.Count;
            Result<Transaction> transaction = context.Invoker.ReserveCoins(revivalCost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.FormatInsufficientBalance(revivalCost, context.Invoker.Coins));

            var resurrectedCount = 0;

            for (var i = 0; i < candidates.Count; i++)
            {
                Pawn pawn = candidates[i];

                Result resurrected = await resurrectionService.ResurrectWithSideEffectsAsync(pawn);

                if (!resurrected.IsSuccess)
                {
                    Logger.Warn(message: "Could not resurrect the pawn '{LabelShort}'", pawn.LabelShort);

                    continue;
                }

                resurrectedCount++;
            }

            if (resurrectedCount <= 0)
            {
                context.Invoker.AbortTransaction(transaction.Value);

                return Result.Fail(new Translation("No one was revived".MarkNotTranslated()));
            }

            await letterService.SendPositiveLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Plural.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Plural.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name, RevivedPawnNames = string.Join(separator: ", ", candidates.Select(p => p.LabelShort)),
                    }
                ),
                candidates
            );

            return Result.Ok(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Revive.Colony.Complete").Format(
                    new
                    {
                        TotalRevived = resurrectedCount.ToString("N0"), TotalCandidates = candidates.Count.ToString("N0"),
                    }
                )
            );
        }

        [Command("random")]
        public async Task<Result> ReviveRandomAsync()
        {
            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Revive.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.FormatInsufficientBalance(StoreIncidentDefOfs.Revive.cost, context.Invoker.Coins));

            List<Pawn> candidates = Find.Maps.AsParallel().SelectMany(m => m.mapPawns.AllPawns).Where(p => p.IsColonist && p is { IsPlayerControlled: true, Dead: true, }).ToList();

            if (candidates.Count <= 0)
            {
                context.Invoker.AbortTransaction(transaction.Value);

                return Result.Fail(new Translation("There's no one dead, so you can't revive anyone right now.".MarkNotTranslated()));
            }

            int randomIndex = ThreadSafeRandom.Next(candidates.Count);
            Pawn randomPawn = candidates[randomIndex];

            Result result = await resurrectionService.ResurrectWithSideEffectsAsync(randomPawn);

            if (!result.IsSuccess)
            {
                context.Invoker.AbortTransaction(transaction.Value);

                return result;
            }

            context.Invoker.FinalizeTransaction(transaction.Value);
            await letterService.SendPositiveLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Singular.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Other.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name, RevivedPawnName = randomPawn.LabelShort,
                    }
                ),
                randomPawn
            );

            return Result.Ok(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Revive.Other.Complete").Format(
                    new
                    {
                        RevivedPerson = randomPawn.LabelShort,
                    }
                )
            );
        }

        [Command("viewer")]
        public async Task<Result> ReviveViewerAsync(Viewer targetViewer)
        {
            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Revive.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.FormatInsufficientBalance(StoreIncidentDefOfs.Revive.cost, context.Invoker.Coins));

            Pawn? pawn = ViewerPawnRegistry.Get(targetViewer.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
            if (pawn is not { Dead: true, }) return Result.Fail(TranslationService.Instance.GetNotDeadError(pawn));

            Result resurrected = await resurrectionService.ResurrectWithSideEffectsAsync(pawn);

            if (!resurrected.IsSuccess)
            {
                context.Invoker.AbortTransaction(transaction.Value);

                return resurrected;
            }

            context.Invoker.Charge(StoreIncidentDefOfs.Revive);
            await letterService.SendPositiveLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Singular.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Revive.Singular.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name, RevivedPawnName = pawn.LabelShort,
                    }
                ),
                pawn
            );

            return Result.Ok(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Revive.Other.Complete").Format(
                    new
                    {
                        RevivedPerson = pawn.LabelShort,
                    }
                )
            );
        }
    }
}

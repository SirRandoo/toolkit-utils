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
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[PublicAPI]
[Group("buy")]
public sealed class RandomInspire : CommandGroup
{
    [PublicAPI]
    [Group("inspiration")]
    public sealed class Inspiration(ExecutionContext context) : CommandGroup
    {
        [Command("random")]
        public Task<Result> RandomInspireAsync()
        {
            if (Current.Game == null) return Task.FromResult(Result.Fail(TranslationService.Instance.GetNoGameError()));

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.RandomInspire.cost);

            if (!transaction.IsSuccess)
                return Task.FromResult(Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.RandomInspire.cost, context.Invoker.Coins)));

            List<InspirationDef> inspirationDefs = DefDatabase<InspirationDef>.AllDefsListForReading;
            int randomIndex = ThreadSafeRandom.Next(minimum: 0, inspirationDefs.Count);
            InspirationDef inspirationDef = inspirationDefs[randomIndex];

            List<Pawn> pawnCandidates = Current.Game.Maps.AsParallel().SelectMany(m => m.mapPawns.AllPawnsSpawned)
                                               .Where(p => !p.Inspired && inspirationDef.Worker.InspirationCanOccur(p)).ToList();

            if (pawnCandidates.Count <= 0) return Task.FromResult(Result.Fail(new Translation("No pawns found to inspire.".MarkNotTranslated())));

            Pawn candidate = pawnCandidates.RandomElementByWeight(p => inspirationDef.Worker.CommonalityFor(p));

            if (candidate == null) return Task.FromResult(Result.Fail(new Translation("No pawns found to inspire.".MarkNotTranslated())));
            if (!candidate.mindState.inspirationHandler.TryStartInspiration(inspirationDef)) return Task.FromResult(Result.Fail());

            transaction.Value.PostTransaction();

            return Task.FromResult(
                Result.Ok(
                    TranslationService.Instance.GetTranslation("TKUtils.Responses.RandomInspire.Complete").Format(
                        new
                        {
                            ViewerName = candidate.LabelShort, InspirationName = inspirationDef.label,
                        }
                    )
                )
            );
        }
    }
}

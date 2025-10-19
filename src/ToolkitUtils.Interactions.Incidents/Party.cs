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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Party : CommandGroup
{
    [Command("party")]
    public async Task<Result> HostPartyAsync()
    {
        Map map = await MainThreadExtensions.OnMainAsync(() => Find.AnyPlayerHomeMap);

        if (map == null) return Result.Ok(TranslationService.Instance.FormatNoPlayerMapFound());

        var worker = new GatheringWorker_Party
        {
            def = GatheringDefOf.Party,
        };
        bool canParty = await MainThreadExtensions.OnMainAsync(worker.CanExecute, map, (Pawn)null!);

        if (!canParty) return Result.Fail(new Translation("Someone tried to throw a party, but everyone was busy."));

        bool heldParty = await MainThreadExtensions.OnMainAsync(worker.TryExecute, map, (Pawn)null!);

        return !heldParty ? Result.Fail(new Translation("Someone threw a party, but no one showed up.")) : Result.Ok(new Translation("Someone decided to throw a party."));
    }
}

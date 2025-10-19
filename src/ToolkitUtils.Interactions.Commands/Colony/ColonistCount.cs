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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class ColonistCount : CommandGroup
{
    [Command("colonists")]
    public async Task<Result> GetColonistCountAsync()
    {
        if (Find.CurrentMap is null) return Result.Fail(TranslationService.Instance.FormatNoMapFound());

        int total = await RouterService.Instance.RouteToMainAsync(GetColonistCount);

        return Result.Ok(TranslationService.Instance.FormatColonistCount(total));
    }

    private static int GetColonistCount()
    {
        // TODO: This is a very inefficient way to get the colonist count.
        //       Find a better way to do this. Maybe caching or a "push" system?
        return Find.Maps.SelectMany(m => m.mapPawns.AllPawns).Count(p => p.IsColonist && p.HomeFaction == Faction.OfPlayer);
    }
}

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
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Wealth : CommandGroup
{
    [Command("wealth")]
    public async Task<Result> GetMapWealthAsync()
    {
        int totalMaps = Find.Maps.Count;

        if (Current.Game == null || totalMaps <= 0) return Result.Fail(TranslationService.Instance.GetNoGameError());
        if (totalMaps > 1) return await GetMapWealthInternalWithGlobalAsync();

        return await GetMapWealthInternalAsync();
    }

    private async Task<Result> GetMapWealthInternalAsync()
    {
        float currentMapWealth = await RouterService.Instance.RouteToMainAsync(GetCurrentMapWealth);

        return Result.Ok(TranslationService.Instance.FormatMapWealth(currentMapWealth));
    }

    private async Task<Result> GetMapWealthInternalWithGlobalAsync()
    {
        float totalWealth = await RouterService.Instance.RouteToMainAsync(GetPlayerWealth);
        float currentMapWealth = await RouterService.Instance.RouteToMainAsync(GetCurrentMapWealth);

        return Result.Ok(TranslationService.Instance.FormatMapWealthWithGlobal(currentMapWealth, totalWealth));
    }

    private static float GetPlayerWealth() => WealthUtility.PlayerWealth;

    private static float GetCurrentMapWealth() => Find.CurrentMap.wealthWatcher.WealthTotal;
}

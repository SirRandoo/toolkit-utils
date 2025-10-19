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
using System;
using System.Linq;
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

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class PawnStats(ExecutionContext context) : CommandGroup
{
    private static readonly string[] DefaultStats =
    [
        "MeleeDPS", "MeleeHitChance", "MeleeArmorPenetration", "MeleeDodgeChance", "ShootingAccuracyPawn", "AimingDelayFactor", "IncomingDamageFactor",
    ];

    [Command("stats")]
    public async Task<Result> RunCommand(params string[] stats)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn != null) return await GetStatsAsync(pawn, stats.Length <= 0 ? DefaultStats : stats);

        return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
    }

    private async Task<Result> GetStatsAsync(Pawn pawn, params string[] statNames)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < statNames.Length; i++)
        {
            string stat = statNames[i];
            StatDef? statDef = await FindStat(stat);

            if (statDef == null) continue;

            float statValue = await RouterService.Instance.RouteToMainAsync(StatExtension.GetStatValue, pawn, statDef, arg3: true, arg4: -1);
            string stringifiedStatValue = await RouterService.Instance.RouteToMainAsync(statDef.ValueToString, statValue, ToStringNumberSense.Absolute, arg3: true);

            builder.Append(statDef.label.CapitalizeFirst());
            builder.Append(": ");
            builder.Append(stringifiedStatValue);
            builder.Append(", ");
        }

        return Result.Ok(builder.ToString(startIndex: 0, builder.Length - 2));
    }

    private static ValueTask<StatDef?> FindStat(string query)
    {
        return new ValueTask<StatDef?>(DefDatabase<StatDef>.AllDefs.FirstOrDefault(s => IsValidStat(s) && IsStat(s, query)));
    }

    private static bool IsValidStat(StatDef stat) => stat is { showOnHumanlikes: true, showOnPawns: true, };

    private static bool IsStat(Def stat, string query) =>
        stat.label.ToToolkit().Equals(query, StringComparison.InvariantCultureIgnoreCase) || stat.defName.Equals(query, StringComparison.InvariantCultureIgnoreCase);
}

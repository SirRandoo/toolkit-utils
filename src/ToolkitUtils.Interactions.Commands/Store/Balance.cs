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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Api;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using TwitchToolkit;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Balance(ExecutionContext context) : CommandGroup
{
    [Command("bal")]
    public async Task<Result> GetViewerBalanceAsync()
    {
        var container = new string[3];

        if (ToolkitSettings.UnlimitedCoins)
            container[0] = $"{UnicodeCharacter.CoinBag.Codepoint} {UnicodeCharacter.Infinity.Codepoint}";
        else
            container[0] = $"{UnicodeCharacter.CoinBag.Codepoint} {context.Invoker.Coins:N0}";

        container[1] = $"{UnicodeCharacter.BalanceScale.Codepoint} {context.Invoker.Karma / 100f:P0}";

        if (ToolkitSettings.EarningCoins)
        {
            int income = await CalculateCoinAwardAsync();
            string emoji;
            string sign;

            if (income >= 0)
            {
                emoji = UnicodeCharacter.ChartIncreasing.Codepoint;
                sign = "+";
            }
            else
            {
                emoji = UnicodeCharacter.ChartDecreasing.Codepoint;
                sign = "-";
            }

            container[1] = $"{emoji} {sign}{income:N0}";
        }

        return Result.Ok(string.Join(separator: " | ", container));
    }

    private ValueTask<int> CalculateCoinAwardAsync()
    {
        int baseCoins = ToolkitSettings.CoinAmount;
        float multiplier = context.Invoker.Karma / 100f;

        if (context.Invoker.UserTypes.HasFlagFast(UserTypes.Subscriber))
        {
            baseCoins += ToolkitSettings.SubscriberExtraCoins;
            multiplier *= ToolkitSettings.SubscriberCoinMultiplier;
        }
        else if (context.Invoker.UserTypes.HasFlagFast(UserTypes.Vip))
        {
            baseCoins += ToolkitSettings.VIPExtraCoins;
            multiplier *= ToolkitSettings.VIPCoinMultiplier;
        }
        else if (context.Invoker.UserTypes.HasFlagFast(UserTypes.Moderator))
        {
            baseCoins += ToolkitSettings.ModExtraCoins;
            multiplier *= ToolkitSettings.ModCoinMultiplier;
        }

        double minutesElapsed = (DateTime.UtcNow - context.Invoker.LastSeen).TotalMinutes;

        if (ToolkitSettings.ChatReqsForCoins)
        {
            if (minutesElapsed > ToolkitSettings.TimeBeforeHalfCoins) multiplier *= 0.5f;
            if (minutesElapsed > ToolkitSettings.TimeBeforeNoCoins) multiplier *= 0.0f;
        }

        return new ValueTask<int>((int)Math.Ceiling((double)baseCoins * multiplier));
    }
}

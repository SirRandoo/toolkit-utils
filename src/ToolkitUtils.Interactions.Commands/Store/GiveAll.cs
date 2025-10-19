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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using ToolkitUtils.Api;
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Localization;
using TwitchToolkit;
using Viewer = ToolkitUtils.Mod.Data.Viewer;

namespace ToolkitUtils.Interactions.Commands;

/// <summary>
///     Represents a command group that provides functionality to distribute a specified number of coins to all
///     registered viewers under certain configurable conditions.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GiveAll : CommandGroup
{
    /// <summary>
    ///     Distributes a specified number of coins to all registered viewers, applying certain conditions based on viewer
    ///     activity and settings.
    /// </summary>
    /// <param name="recipient">The viewer receiving the coins.</param>
    /// <param name="amount">The number of coins to distribute.</param>
    /// <returns>A task representing the result of the operation.</returns>
    [Command("giveall")]
    public ValueTask<Result> GiveAllAsync(Viewer recipient, int amount)
    {
        DateTime now = DateTime.UtcNow;
        IReadOnlyList<Viewer> viewers = Registries.Viewers.AllRegistrants;

        for (var i = 0; i < viewers.Count; i++)
        {
            Viewer viewer = viewers[i];

            if (ToolkitSettings.ChatReqsForCoins && (now - viewer.LastSeen).TotalMinutes > ToolkitSettings.TimeBeforeNoCoins) continue;

            viewers[i].AddCoins(amount);
        }

        return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatGiveAllCoins(amount, viewers.Count)));
    }
}

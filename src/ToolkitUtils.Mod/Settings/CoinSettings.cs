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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToolkitUtils.Mod.Domain.Settings;

/// <summary>
///     Represents configuration settings for coin-related operations, including attributes for controlling coin
///     behavior, such as intervals for earning coins, initial balance, and whether coin limits apply.
/// </summary>
/// <param name="CoinInterval">The time duration between awarded coins.</param>
/// <param name="StartingBalance">The initial number of coins available.</param>
/// <param name="CoinsAwardedPerInterval">The number of coins awarded at each interval.</param>
/// <param name="AreCoinsUnlimited">Indicates whether coins are not subject to a maximum limit.</param>
/// <param name="CanEarnCoins">Specifies if the earning of coins is enabled.</param>
/// <param name="Version">The version number associated with the coin settings structure.</param>
[PublicAPI]
public sealed record CoinSettings(
    [property: JsonProperty("coin_interval")] TimeSpan CoinInterval,
    [property: JsonProperty("starting_balance")] int StartingBalance = 100,
    [property: JsonProperty("coins_awarded_per_interval")] int CoinsAwardedPerInterval = 50,
    [property: JsonProperty("are_coins_unlimited")] bool AreCoinsUnlimited = false,
    [property: JsonProperty("can_earn_coins")] bool CanEarnCoins = true,
    [property: JsonProperty("version")] byte Version = 1
);

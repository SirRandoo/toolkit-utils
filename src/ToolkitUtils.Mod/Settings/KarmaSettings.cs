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
///     Defines the configuration settings interface required for managing karma behavior and attributes. This
///     includes properties for specifying the initial karma value, boundaries for the karma range, whether neutral karma
///     is supported, general enablement of the karma system, the specific type of karma service, and tier settings used
///     for further customization.
/// </summary>
[PublicAPI]
public sealed record KarmaSettings(
    [property: JsonProperty("karma_range")] IntegerRange KarmaRange,
    [property: JsonProperty("service_type")] KarmaServiceType ServiceType,
    [property: JsonProperty("tiers")] KarmaTierSettings[] Tiers,
    [property: JsonProperty("decay_tiers")] KarmaDecayTier[] DecayTiers,
    [property: JsonProperty("starting_karma")] int StartingKarma = 100,
    [property: JsonProperty("minimum_karma_for_sending_gifts")] int MinimumKarmaForSendingGifts = 0,
    [property: JsonProperty("minimum_karma_for_receiving_gifts")] int MinimumKarmaForReceivingGifts = 0,
    [property: JsonProperty("ban_viewers_who_always_purchase_bad")] bool BanViewersWhoAlwaysPurchaseBad = true,
    [property: JsonProperty("use_true_neutral")] bool UseTrueNeutral = true,
    [property: JsonProperty("is_karma_enabled")] bool IsKarmaEnabled = true,
    [property: JsonProperty("version")] byte Version = 1
);

/// <summary>
///     Provides an interface defining tier-specific configuration settings for the karma system. This interface
///     includes properties to represent the tier level and any associated bonus value specific to that tier, enabling
///     advanced customization of tiered behavior within the karma management framework.
/// </summary>
[PublicAPI]
public sealed record KarmaTierSettings(
    [property: JsonProperty("tier")] byte Tier,
    [property: JsonProperty("bonus")] int Bonus,
    [property: JsonProperty("version")] byte Version = 1
);

/// <summary>Represents a one tier in the karma decay system.</summary>
[PublicAPI]
public sealed record KarmaDecayTier(
    [property: JsonProperty("period")] TimeSpan Period,
    [property: JsonProperty("decay_percent")] float DecayPercent,
    [property: JsonProperty("version")] byte Version = 1
);

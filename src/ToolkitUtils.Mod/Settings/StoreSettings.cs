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
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToolkitUtils.Mod.Domain.Settings;

/// <summary>
///     Represents configurations for store-related settings, allowing customization of options related to purchase
///     pricing, confirmation preferences, and external pricing data.
/// </summary>
[PublicAPI]
public sealed record StoreSettings(
    [property: JsonProperty("minimum_purchase_price")] int MinimumPurchasePrice = 60,
    [property: JsonProperty("send_purchase_confirmations")] bool SendPurchaseConfirmations = true,
    [property: JsonProperty("custom_pricing_sheet_link")] string? CustomPricingSheetLink = null,
    [property: JsonProperty("version")] byte Version = 1
);

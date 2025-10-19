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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit.Incidents;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>
///     The StoreIncidentDefOfs class is a static container class that holds references to various predefined incident
///     definitions related to store operations in the mod.
/// </summary>
/// <remarks>
///     This class provides static access to specific instances of StoreIncident, which are used within the
///     application to describe various events or actions within the store-related context. Each field represents a
///     distinct type of incident that can be triggered or managed by the application.
/// </remarks>
[DefOf]
[PublicAPI]
[StaticConstructorOnStartup]
public static class StoreIncidentDefOfs
{
    public static readonly StoreIncident Sanctuary = null!;
    public static readonly StoreIncident Heal = null!;
    public static readonly StoreIncident Revive = null!;
    public static readonly StoreIncident AddPassion = null!;
    public static readonly StoreIncident RandomInspire = null!;
    public static readonly StoreIncident RemovePassion = null!;
    public static readonly StoreIncident PassionShuffle = null!;
    public static readonly StoreIncident GenderSwap = null!;
    public static readonly StoreIncident RandomAdulthood = null!;
    public static readonly StoreIncident RandomChildhood = null!;
    public static readonly StoreIncident AddTrait = null!;
    public static readonly StoreIncident RemoveTrait = null!;
}

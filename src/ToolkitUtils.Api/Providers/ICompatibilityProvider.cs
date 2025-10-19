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
using ToolkitUtils.Mod;

namespace ToolkitUtils.Api;

/// <summary>Represents a special object capable of changing the behavior of certain aspects of the mod's functions.</summary>
public interface ICompatibilityProvider : IIdentifiable
{
    /// <summary>
    ///     The relative priority of the compatibility provider. Providers with a higher priority will execute before
    ///     providers with a lower priority.
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     A collection of mod ids that are required for before this compatibility provider will alter the mod's
    ///     functions.
    /// </summary>
    string[] RequiredMods { get; }
}

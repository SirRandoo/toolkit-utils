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
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>Provides extension methods for the FactionManager class.</summary>
[PublicAPI]
public static class FactionManagerExtensions
{
    /// <summary>Asynchronously retrieves a list of factions from the given FactionManager.</summary>
    /// <param name="manager">The FactionManager instance from which to retrieve the list of factions.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of factions.</returns>
    public static async Task<List<Faction>> GetFactionsAsync(this FactionManager manager) => await MainThreadExtensions.OnMainAsync(GetFactionsListForReading, manager);

    /// <summary>Retrieves a list of factions that are available for reading from the specified FactionManager.</summary>
    /// <param name="manager">The FactionManager instance from which to retrieve the list of factions.</param>
    /// <returns>A List of Faction objects available for reading.</returns>
    private static List<Faction> GetFactionsListForReading(FactionManager manager) => manager.AllFactionsListForReading;
}

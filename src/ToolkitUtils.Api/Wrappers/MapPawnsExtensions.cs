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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>
///     A collection of extension methods for querying information from <see cref="MapPawns" /> asynchronously in a
///     safe way.
/// </summary>
[PublicAPI]
public static class MapPawnsExtensions
{
    /// <summary>Returns an array of pawns registered within the given map.</summary>
    /// <param name="mapPawns">A <see cref="MapPawns" /> instance containing the pawns active within a given map.</param>
    public static async Task<List<Pawn>> GetPawnsAsync(this MapPawns mapPawns) => await MainThreadExtensions.OnMainAsync(mapPawns.AllPawns.ToList);

    /// <summary>Returns an array of pawns spawned on the given map.</summary>
    /// <param name="mapPawns">A <see cref="MapPawns" /> instance containing the pawns active within a given map.</param>
    public static async Task<List<Pawn>> GetSpawnedPawnsAsync(this MapPawns mapPawns) => await MainThreadExtensions.OnMainAsync(mapPawns.AllPawnsSpawned.ToList);

    /// <summary>Returns an array of pawns that aren't spawned on the given map, but still exist on the map.</summary>
    /// <param name="mapPawns">A <see cref="MapPawns" /> instance containing the pawns active within a given map.</param>
    public static async Task<List<Pawn>> GetUnspawnedPawnsAsync(this MapPawns mapPawns) => await MainThreadExtensions.OnMainAsync(mapPawns.AllPawnsUnspawned.ToList);
}

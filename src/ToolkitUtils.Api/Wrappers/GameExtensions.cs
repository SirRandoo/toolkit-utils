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
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld.Planet;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>A collection of extension for querying information from the game asynchronously in a safe way.</summary>
[PublicAPI]
public static class GameExtensions
{
    /// <summary>Returns an array of maps currently generated within the game.</summary>
    /// <param name="game">A <see cref="Game" /> instance which houses information about the game.</param>
    public static async Task<Map[]> GetMapsAsync(this Game game) => await MainThreadExtensions.OnMainAsync(game.Maps.ToArray);

    /// <summary>Returns a map with the given parent.</summary>
    /// <param name="game">A <see cref="Game" /> instance which houses information about the game.</param>
    /// <param name="parent">The parent of the map being queried.</param>
    public static async Task<Map> FindMapAsync(this Game game, MapParent parent) => await MainThreadExtensions.OnMainAsync(game.FindMap, parent);

    /// <summary>Returns a map at the given tile.</summary>
    /// <param name="game">A <see cref="Game" /> instance which houses information about the game.</param>
    /// <param name="tile">An integer representing a tile on the world map.</param>
    public static async Task<Map> FindMapAsync(this Game game, PlanetTile tile) => await MainThreadExtensions.OnMainAsync(game.FindMap, tile);

    /// <summary>Adds a new map to the game.</summary>
    /// <param name="game">A <see cref="Game" /> instance which houses information about the game.</param>
    /// <param name="map">The map to add to the game.</param>
    public static async Task AddMapAsync(this Game game, Map map)
    {
        await MainThreadExtensions.OnMainAsync(game.AddMap, map);
    }
}

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
using System.Threading.Tasks;
using Verse;

namespace ToolkitUtils.Mod.Services;

/// <summary>Provides services for spawning in-game objects such as pawns and items.</summary>
public interface ISpawnService
{
    /// Asynchronously spawns a pawn at a specified position on a given map.
    /// Attempts to route spawning through a gateway, if available, or performs direct spawning if routing fails.
    /// <param name="pawn">The pawn to be spawned.</param>
    /// <param name="map">The map on which the pawn should be spawned.</param>
    /// <param name="position">The position on the map where the pawn should be spawned.</param>
    /// <returns>A result indicating the success or failure of the spawn operation.</returns>
    Task<Result> SpawnPawnAsync(Pawn pawn, Map map, IntVec3 position);

    /// Spawns the specified item on the given map at the specified position.
    /// <param name="thing">The item to be spawned.</param>
    /// <param name="map">The map where the item will be spawned.</param>
    /// <param name="position">The position on the map where the item will be spawned.</param>
    /// <returns>A result indicating the success or failure of the spawning operation.</returns>
    Task<Result> SpawnItemAsync(Thing thing, Map map, IntVec3 position);
}

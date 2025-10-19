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
using RimWorld;
using ToolkitUtils.Mod.Domain;
using Verse;

namespace ToolkitUtils.Mod.Services;

/// <inheritdoc />
internal sealed class SpawnService(RouterService routerService, IGatewayService gatewayService) : ISpawnService
{
    /// <inheritdoc />
    public async Task<Result> SpawnPawnAsync(Pawn pawn, Map map, IntVec3 position)
    {
        await ValidateSpawnAsync(pawn, map, position);

        if (!gatewayService.TryGetRandomGateway(SpawnFlags.AllowsPawns, out IToolkitGateway? gateway)) return await SpawnPawnDirect(pawn, map, position);

        bool spawned = await routerService.RouteToMainAsync(gateway.TrySpawn, pawn);

        if (spawned) return Result.Ok();

        return await SpawnPawnDirect(pawn, map, position);
    }

    /// <inheritdoc />
    public async Task<Result> SpawnItemAsync(Thing thing, Map map, IntVec3 position)
    {
        await ValidateSpawnAsync(thing, map, position);

        if (!gatewayService.TryGetRandomGateway(SpawnFlags.AllowsItems, out IToolkitGateway? gateway)) return await SpawnItemDirectAsync(thing, map, position);

        bool spawned = await routerService.RouteToMainAsync(gateway.TrySpawn, thing);

        if (spawned) return Result.Ok();

        return await SpawnItemDirectAsync(thing, map, position);
    }

    private async Task<Result> SpawnPawnDirect(Pawn pawn, Map map, IntVec3 position)
    {
        await routerService.RouteToMainAsync(GenSpawn.Spawn, pawn, position, map, WipeMode.Vanish);

        return Result.Ok();
    }

    private async Task<Result> SpawnItemDirectAsync(Thing thing, Map map, IntVec3 position)
    {
        await routerService.RouteToMainAsync(TradeUtility.SpawnDropPod, position, map, thing);

        return Result.Ok();
    }

    private static ValueTask<Result> ValidateSpawnAsync(Thing thing, Map map, IntVec3 position)
    {
        // We'll ensure items that can hatch, like eggs, are assigned to the player's faction.
        // Without this, the animals that hatch would be wild animals, not animals that the player
        // would have control over.
        if (thing.TryGetComp(out CompHatcher hatcher) && hatcher.hatcheeFaction != Faction.OfPlayer) hatcher.hatcheeFaction = Faction.OfPlayer;

        return new ValueTask<Result>(Result.Ok());
    }

    private static ValueTask<Result> ValidateSpawnAsync(Pawn pawn, Map map, IntVec3 position) => new(Result.Ok());
}

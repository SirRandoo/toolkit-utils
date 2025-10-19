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
using System.Diagnostics.CodeAnalysis;
using NLog;
using ToolkitUtils.Mod.Domain;
using ToolkitUtils.Mod.Logging;
using Verse;

namespace ToolkitUtils.Mod.Services;

internal sealed class NoOpGatewayService : IGatewayService
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    public bool TryRegisterGateway(IToolkitGateway gateway)
    {
        Logger.Info(message: "Attempting to register gateway {GatewayId}", gateway.Id);

        if (Current.Game == null)
        {
            Logger.Warn("There is no active game; aborting gateway registration");

            return false;
        }

        bool hasMaps = Current.Game.Maps.Count > 0;

        Logger.Info(message: "Gateway registered? {HasMaps}", hasMaps ? "Yes" : "No");

        return hasMaps;
    }

    public bool TryUnregisterGateway(IToolkitGateway gateway)
    {
        Logger.Info(message: "Attempting to unregister gateway {GatewayId}", gateway.Id);

        if (Current.Game == null)
        {
            Logger.Warn("There is no active game; aborting gateway unregistration");

            return false;
        }


        bool hasMaps = Current.Game.Maps.Count > 0;

        Logger.Info(message: "Gateway unregistered? {HasMaps}", hasMaps ? "Yes" : "No");

        return hasMaps;
    }

    public bool TryGetRandomGateway(SpawnFlags flags, [NotNullWhen(true)] out IToolkitGateway? gateway)
    {
        Logger.Info(message: "Attempting to get a random gateway with flags: {Flags}", flags);

        if (Current.Game?.Maps is { Count: <= 0, })
        {
            Logger.Warn("No maps are loaded; aborting gateway retrieval");

            gateway = null;

            return false;
        }

        Logger.Info("Gateway retrieved");
        gateway = new NoOpGateway(flags);

        return true;
    }

    private sealed class NoOpGateway(SpawnFlags flags) : IToolkitGateway
    {
        [SuppressMessage(category: "ReSharper", checkId: "MemberHidesStaticFromOuterClass")]
        private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

        public SpawnFlags SpawnFlags { get; } = flags;
        public string Id { get; } = Guid.NewGuid().ToString();

        public bool TrySpawn(Thing thing)
        {
            Logger.Info("Attempting to spawn item");

            if (!SpawnFlags.HasFlagFast(SpawnFlags.AllowsItems))
            {
                Logger.Warn("Attempted to spawn item with a gateway that doesn't allow items; aborting...");

                return false;
            }

            return true;
        }

        public bool TrySpawn(Pawn pawn)
        {
            Logger.Info(message: "Attempting to spawn pawn {PawnId}", pawn.GetUniqueLoadID());

            switch (pawn.RaceProps.Animal)
            {
                case true when SpawnFlags.HasFlagFast(SpawnFlags.AllowsAnimals):
                    Logger.Info(message: "Spawned animal {AnimalId}", pawn.GetUniqueLoadID());

                    return true;
                case false when SpawnFlags.HasFlagFast(SpawnFlags.AllowsPawns):
                    Logger.Info(message: "Spawned pawn {PawnId}", pawn.GetUniqueLoadID());

                    return true;
                default:
                    Logger.Warn("Attempted to spawn pawn with a gateway that doesn't allow it; aborting...");

                    return false;
            }
        }
    }
}

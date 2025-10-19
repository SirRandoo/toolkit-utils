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
using System.Linq;
using ConcurrentCollections;
using ToolkitUtils.Mod.Domain;
using TwitchToolkit;

namespace ToolkitUtils.Mod.Services;

/// <inheritdoc />
internal sealed class GatewayService : IGatewayService
{
    private static readonly Lazy<IGatewayService> InstanceField = new(() => new GatewayService());
    private readonly ConcurrentHashSet<IToolkitGateway> _gateways = [];

    /// <summary>
    ///     Gets the singleton instance of the gateway service, ensuring that the same instance is shared across the
    ///     application. This ensures centralized management for registering, unregistering, and retrieving gateways.
    /// </summary>
    internal static IGatewayService Instance => InstanceField.Value;

    /// <inheritdoc />
    public bool TryRegisterGateway(IToolkitGateway gateway) => _gateways.Add(gateway);

    /// <inheritdoc />
    public bool TryUnregisterGateway(IToolkitGateway gateway) => _gateways.TryRemove(gateway);

    /// <inheritdoc />
    public bool TryGetRandomGateway(SpawnFlags flags, [NotNullWhen(true)] out IToolkitGateway? gateway)
    {
        gateway = _gateways.Where(g => g.SpawnFlags == flags).RandomElement(ThreadSafeRandom.Instance);

        return gateway != null;
    }
}

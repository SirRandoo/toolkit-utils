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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Domain;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Provides methods for managing and interacting with toolkit gateways. This service allows for registering,
///     unregistering, and retrieving gateways dynamically.
/// </summary>
[PublicAPI]
public interface IGatewayService
{
    /// <summary>Attempts to register an instance of <see cref="IToolkitGateway" /> with the gateway service.</summary>
    /// <param name="gateway">The gateway instance to register.</param>
    /// <returns>True if the gateway was successfully registered; otherwise, false.</returns>
    bool TryRegisterGateway(IToolkitGateway gateway);

    /// <summary>Attempts to unregister a specified gateway from the gateway service.</summary>
    /// <param name="gateway">The gateway to be unregistered.</param>
    /// <returns>True if the gateway was successfully unregistered; otherwise, false.</returns>
    bool TryUnregisterGateway(IToolkitGateway gateway);

    /// Attempts to retrieve a random gateway from the registered gateways.
    /// <param name="flags">The spawn flags that must be set in the gateway.</param>
    /// <param name="gateway">When this method returns, contains the randomly selected gateway if found; otherwise, null.</param>
    /// <returns>True if a random gateway was successfully retrieved; otherwise, false.</returns>
    bool TryGetRandomGateway(SpawnFlags flags, [NotNullWhen(true)] out IToolkitGateway? gateway);
}

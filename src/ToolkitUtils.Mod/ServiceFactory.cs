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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ToolkitUtils.Mod;

/// <summary>Provides a mechanism for managing and retrieving service instances based on execution variants.</summary>
/// <typeparam name="TService">The type of service instance managed by the factory.</typeparam>
public static class ServiceFactory<TService>
{
    private static readonly ConcurrentDictionary<ExecutionVariant, TService> Services = [];

    /// <summary>Registers a service instance for a specific execution variant.</summary>
    /// <param name="variant">The execution variant for which the service is being registered.</param>
    /// <param name="service">The service instance to register for the specified variant.</param>
    public static void RegisterService(ExecutionVariant variant, TService service) => Services[variant] = service;

    /// <summary>Unregisters a service associated with the specified execution variant.</summary>
    /// <param name="variant">The execution variant for which the service is to be unregistered.</param>
    public static void UnregisterService(ExecutionVariant variant) => Services.TryRemove(variant, out TService _);

    /// <summary>Retrieves the registered service associated with the specified execution variant.</summary>
    /// <param name="variant">The execution variant for which the service is requested.</param>
    /// <returns>The registered service corresponding to the given execution variant.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no service is registered for the specified execution variant.</exception>
    public static TService GetService(ExecutionVariant variant) => Services.TryGetValue(variant, out TService? service) ? service : throw new KeyNotFoundException(); 
}

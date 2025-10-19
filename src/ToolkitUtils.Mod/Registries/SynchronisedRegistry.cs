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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ToolkitUtils.Mod.Registries;

/// <summary>
///     A registry that uses eventual consistency to synchronize its contents. This implementation allows for
///     concurrent modifications and processes changes in a background task.
/// </summary>
/// <typeparam name="T">
///     The type of the class being represented within the registry. Must implement
///     <see cref="IIdentifiable" /> to ensure each object has a unique identifier.
/// </typeparam>
public class SynchronisedRegistry<T> : IRegistry<T> where T : class, IIdentifiable
{
    private const int BatchSizeThreshold = 10; // Threshold for batch processing
    private const int MinimumDelayMs = 50;     // Minimum delay between processing
    private const int MaximumDelayMs = 200;    // Maximum delay between processing
    private readonly ConcurrentDictionary<string, T> _allRegistrantsKeyed = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConcurrentQueue<RegistryChangeEvent> _pendingChanges = new();
    private int _pendingChangeCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SynchronisedRegistry{T}" /> class. Starts a background task to
    ///     process pending changes to the registry.
    /// </summary>
    public SynchronisedRegistry()
    {
        Task.Factory.StartNew(function: () => ProcessChanges(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
    }

    /// <returns>
    ///     An immutable list of all currently registered objects in the registry. This provides a snapshot of the
    ///     registry's contents at the time of the call.
    /// </returns>
    public IReadOnlyList<T> AllRegistrants => _allRegistrantsKeyed.Values.ToList().AsReadOnly();

    /// <summary>
    ///     Registers a new object in the registry by enqueuing a registration change event. The actual registration will
    ///     be processed asynchronously.
    /// </summary>
    /// <param name="obj">The object to register.</param>
    /// <returns>
    ///     <see langword="true" /> to indicate that the registration request has been enqueued; <see langword="false" />
    ///     is not returned as this implementation always enqueues successfully.
    /// </returns>
    public bool Register([DisallowNull] T obj)
    {
        _pendingChanges.Enqueue(new RegistryChangeEvent(obj.Id, obj, IsRegistering: true));
        Interlocked.Increment(ref _pendingChangeCount);

        return true;
    }

    /// <summary>
    ///     Unregisters an existing object from the registry by enqueuing an unregistration change event. The actual
    ///     unregistration will be processed asynchronously.
    /// </summary>
    /// <param name="obj">The object to unregister.</param>
    /// <returns>
    ///     <see langword="true" /> to indicate that the unregistration request has been enqueued;
    ///     <see langword="false" /> is not returned as this implementation always enqueues successfully.
    /// </returns>
    public bool Unregister([DisallowNull] T obj)
    {
        _pendingChanges.Enqueue(new RegistryChangeEvent(obj.Id, obj, IsRegistering: false));
        Interlocked.Increment(ref _pendingChangeCount);

        return true;
    }

    /// <summary>
    ///     Retrieves an object from the registry using its unique identifier. Returns <see langword="null" /> if the
    ///     object does not exist.
    /// </summary>
    /// <param name="id">The unique identifier of the object to retrieve.</param>
    /// <returns>The object associated with the specified identifier, or <see langword="null" /> if not found.</returns>
    public T? GetById(string id) => _allRegistrantsKeyed.GetValueOrDefault(id);

    public T? GetByName(string name)
    {
        for (var i = 0; i < AllRegistrants.Count; i++)
            if (string.Equals(AllRegistrants[i].Name, name, StringComparison.CurrentCultureIgnoreCase))
                return AllRegistrants[i];

        return null;
    }

    /// <inheritdoc />
    public bool TryGetById(string id, [NotNullWhen(true)] out T? obj)
    {
        obj = GetById(id);

        return obj != null;
    }

    /// <inheritdoc />
    public bool TryGetByName(string name, [NotNullWhen(true)] out T? obj)
    {
        obj = GetByName(name);

        return obj != null;
    }

    /// <summary>
    ///     Creates an instance of the <see cref="SynchronisedRegistry{T}" /> and initializes it with the specified
    ///     registrants. This method ensures that all provided registrants are registered in the new instance.
    /// </summary>
    /// <param name="registrants">A read-only list of registrants to initialize the registry with.</param>
    /// <returns>A new instance of <see cref="SynchronisedRegistry{T}" /> with the provided registrants.</returns>
    /// <exception cref="InvalidOperationException">Thrown if any of the provided registrants have duplicate identifiers.</exception>
    public static SynchronisedRegistry<T> CreateInstance(IReadOnlyList<T> registrants)
    {
        var registry = new SynchronisedRegistry<T>();

        foreach (T? registrant in registrants)
        {
            if (!registry._allRegistrantsKeyed.TryAdd(registrant.Id, registrant)) throw new InvalidOperationException($"An entry with the id '{registrant.Id}' already exists.");

            // Enqueue the registration event to process it in the background.
            registry._pendingChanges.Enqueue(new RegistryChangeEvent(registrant.Id, registrant, IsRegistering: true));
        }

        return registry;
    }

    /// <summary>
    ///     Processes pending changes in the registry in a background task. This method runs continuously, checking for
    ///     changes to apply and managing the timing of processing.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to signal when processing should stop.</param>
    private async Task ProcessChanges(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_pendingChangeCount > 0)
            {
                var changesToProcess = new List<RegistryChangeEvent>();

                while (_pendingChanges.TryDequeue(out RegistryChangeEvent change) && changesToProcess.Count < BatchSizeThreshold) changesToProcess.Add(change);

                foreach (RegistryChangeEvent change in changesToProcess)
                {
                    if (change.IsRegistering)
                        _allRegistrantsKeyed.TryAdd(change.Id, change.Registrant);
                    else
                        _allRegistrantsKeyed.TryRemove(change.Id, out T _);
                }

                Interlocked.Add(ref _pendingChangeCount, -changesToProcess.Count);
            }

            int delay = Math.Min(Math.Max(MinimumDelayMs + _pendingChangeCount * 5, MinimumDelayMs), MaximumDelayMs);

            await Task.Delay(delay, cancellationToken);
        }
    }

    /// <summary>
    ///     Represents a change event in the registry, which includes the identifier of the object, the object itself, and
    ///     a flag indicating whether the operation is a registration or unregistration.
    /// </summary>
    /// <param name="Id">The unique identifier of the registrant.</param>
    /// <param name="Registrant">The object being registered or unregistered.</param>
    /// <param name="IsRegistering">
    ///     A flag indicating whether the operation is for registration (true) or unregistration
    ///     (false).
    /// </param>
    private record struct RegistryChangeEvent(string Id, T Registrant, bool IsRegistering);
}

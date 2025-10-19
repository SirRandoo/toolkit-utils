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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Messaging;

/// <summary>
///     GlobalEventSystem is a sealed class that implements the IGlobalEventSystem interface. It provides methods to
///     publish events and allows subscribers to receive these events. The GlobalEventSystem manages a collection of event
///     handlers for different event types.
/// </summary>
[PublicAPI]
public sealed class GlobalEventSystem
{
    private static readonly Lazy<GlobalEventSystem> InstanceField = new(() => new GlobalEventSystem());
    private readonly Dictionary<Type, List<Delegate>> _handlers = [];

    /// <summary>
    ///     Provides a thread-safe, lazily initialized, singleton instance of the <see cref="GlobalEventSystem" /> class.
    ///     The instance is used to manage publishing and subscribing to events within the system.
    /// </summary>
    public static GlobalEventSystem Instance => InstanceField.Value;

    /// <summary>Publishes an event to all registered subscribers of the specified event type.</summary>
    /// <typeparam name="TEvent">The type of the event being published.</typeparam>
    /// <param name="eventData">The event data to publish to the subscribers.</param>
    public void Publish<TEvent>(TEvent eventData)
    {
        if (!_handlers.TryGetValue(typeof(TEvent), out List<Delegate>? handlers)) return;

        foreach (Action<TEvent> handler in handlers.Cast<Action<TEvent>>()) handler(eventData);
    }

    /// <summary>Subscribes to an event of the specified type with the given handler.</summary>
    /// <typeparam name="TEvent">The type of the event to subscribe to.</typeparam>
    /// <param name="handler">The callback to handle the subscribed event.</param>
    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        if (!_handlers.ContainsKey(typeof(TEvent))) _handlers[typeof(TEvent)] = [];

        _handlers[typeof(TEvent)].Add(handler);
    }

    /// <summary>Unsubscribes a specified handler from receiving events of the specified event type.</summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <param name="handler">The handler to be removed from the subscription list.</param>
    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        if (!_handlers.TryGetValue(typeof(TEvent), out List<Delegate>? handlers)) return;

        handlers.Remove(handler);

        if (handlers.Count <= 0) _handlers.Remove(typeof(TEvent));
    }
}

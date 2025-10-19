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
using NetEscapades.EnumGenerators;
using Verse;

namespace ToolkitUtils.Mod.Domain;

/// <summary>Represents the different categories of entities that can emerge from the <see cref="IToolkitGateway" />.</summary>
[Flags]
[EnumExtensions]
public enum SpawnFlags
{
    /// <summary>Indicates that no entities can emerge from the gateway.</summary>
    None = 0,

    /// <summary>Indicates that animals can emerge from the gateway.</summary>
    AllowsAnimals = 1,

    /// <summary>Indicates that items can emerge from the gateway.</summary>
    AllowsItems = 2,

    /// <summary>Indicates that pawns can emerge from the gateway.</summary>
    AllowsPawns = 4,
}

/// <summary>
///     Defines a gateway interface for managing the spawning of entities within a certain context. Provides methods
///     for attempting to spawn items or pawns, with support for managing spawn flags.
/// </summary>
public interface IToolkitGateway
{
    /// <summary>Specifies the spawn permissions for different entity types within the context of a gateway system.</summary>
    /// <remarks>
    ///     This enumeration is used to define which types of entities, such as animals, items, or pawns, can be spawned
    ///     by implementations of <see cref="IToolkitGateway" />. Each flag represents a specific category, and the flags can
    ///     be combined using bitwise operations to allow multiple categories simultaneously.
    /// </remarks>
    SpawnFlags SpawnFlags { get; }

    /// <summary>Returns the unique id of the gateway.</summary>
    string Id { get; }

    /// <summary>Attempts to spawn the specified thing at the gateway's position.</summary>
    /// <param name="thing">The <see cref="Thing" /> instance to be spawned.</param>
    /// <returns>Returns <c>true</c> if the thing was successfully spawned; otherwise <c>false</c>.</returns>
    bool TrySpawn(Thing thing);

    /// <summary>Attempts to spawn a specified pawn into the world at the gateway's location.</summary>
    /// <param name="pawn">The pawn to be spawned.</param>
    /// <returns>True if the pawn was successfully spawned; otherwise, false.</returns>
    bool TrySpawn(Pawn pawn);
}

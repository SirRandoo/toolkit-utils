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

namespace ToolkitUtils.Mod;

/// <summary>
///     Defines the types of events that can occur within the domain. Each value represents a specific category of
///     event that can be flagged and processed accordingly.
/// </summary>
[Flags]
[EnumExtensions]
public enum EventTypes
{
    /// <summary>
    ///     Indicates the absence of any event types or represents an uninitialized state. This is often used as a default
    ///     value or to signify no specific event types are applicable.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Represents an event type associated with an item. This value is used to indicate that the event pertains
    ///     specifically to an item
    /// </summary>
    Item = 1,

    /// <summary>
    ///     Represents an event type associated with a pawn. Typically used for events involving specific characters,
    ///     entities, or units within the domain.
    /// </summary>
    Pawn = 2,

    /// <summary>
    ///     Represents an event type associated with traits, typically used to signify or track the presence of specific
    ///     personality or behavioral characteristics for an entity.
    /// </summary>
    Trait = 4,

    /// <summary>
    ///     Represents an event type tied to dynamically changing or configurable properties. This value can be used to
    ///     signify elements or entities that involve variable factors or custom setups.
    /// </summary>
    Variable = 8,

    /// <summary>
    ///     Represents an event type associated with the act of revival, typically used to indicate an event that restores
    ///     a previously incapacitated entity to an active or functional state.
    /// </summary>
    Revival = 16,

    /// <summary>
    ///     Represents an event type associated with the action of healing. This can be used to identify or handle events
    ///     that involve the restoration of health or similar recovery-related activities in the system.
    /// </summary>
    Heal = 32,
}

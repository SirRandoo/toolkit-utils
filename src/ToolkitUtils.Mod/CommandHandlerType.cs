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
namespace ToolkitUtils.Mod;

/// <summary>Defines a distinct type for handling commands based on a specific name and value.</summary>
/// <remarks>
///     This structure provides a clear distinction for command handlers by associating them with a name and a
///     corresponding byte value. It encapsulates predefined handler types to maintain consistency in command processing
///     logic, such as classic or default handlers.
/// </remarks>
public readonly record struct CommandHandlerType(string Name, byte Value)
{
    /// <summary>Represents the "Classic" command handler type, identified by its unique name and associated value.</summary>
    /// <remarks>
    ///     This handler type typically corresponds to the traditional or legacy mode of processing commands within the
    ///     system, offering compatibility with older implementations or expected legacy behavior.
    /// </remarks>
    public static readonly CommandHandlerType Classic = new(Name: "Classic", Value: 0);

    /// <summary>Represents the default command handler type with predefined settings for standard operations.</summary>
    public static readonly CommandHandlerType Default = new(Name: "Default", Value: 1);
}

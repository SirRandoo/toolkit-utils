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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod;

/// <summary>Represents a method used to determine a leave mechanism.</summary>
/// <remarks>
///     This struct provides predefined methods for leave scenarios. Users can specify custom methods using the
///     <see cref="Name" /> and <see cref="Value" /> properties.
/// </remarks>
/// <param name="Name">The name of the leave method.</param>
/// <param name="Value">The value associated with the leave method.</param>
[PublicAPI]
public readonly record struct LeaveMethod(string Name, byte Value)
{
    private const string DefaultName = "Default";
    private const byte DefaultValue = 0;
    private const string DustName = "Dust";
    private const byte DustValue = 1;
    private const string DropName = "Drop";
    private const byte DropValue = 2;

    /// <summary>Represents the default leave method within the system.</summary>
    /// <remarks>
    ///     This value signifies the standard or predefined behavior for a pawn's departure process, serving as the
    ///     fallback or base configuration in the absence of a custom leave method specification.
    /// </remarks>
    public static readonly LeaveMethod Default = new(DefaultName, DefaultValue);

    /// <summary>Represents a leave method where the pawn disintegrates into dust upon departure.</summary>
    /// <remarks>
    ///     This value defines an alternative leave method labeled as "Dust," assigned a unique identifier. It signifies
    ///     that the selected behavior for leaving results in the pawn turning into dust, providing a distinct visual or
    ///     gameplay effect.
    /// </remarks>
    public static readonly LeaveMethod Dust = new(DustName, DustValue);

    /// <summary>Represents a leave method where the pawn drops items upon departure.</summary>
    /// <remarks>
    ///     This value defines a specific leave behavior labeled as "Drop," assigned a unique identifier. It signifies
    ///     that when this method is selected, the pawn's departure results in their inventory being dropped, allowing for item
    ///     retrieval or redistribution in the game world.
    /// </remarks>
    public static readonly LeaveMethod Drop = new(DropName, DropValue);

    /// <summary>Returns a <see cref="LeaveMethod" /> object based on the provided leave method name.</summary>
    /// <param name="name">The name of the leave method to convert.</param>
    /// <returns>A <see cref="LeaveMethod" /> corresponding to the provided name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided name does not match any predefined leave method.</exception>
    public static LeaveMethod FromName(string name) =>
        name switch
        {
            DefaultName => Default,
            DustName    => Dust,
            DropName    => Drop,
            var _       => throw new ArgumentOutOfRangeException(nameof(name), name, message: "No suitable leave method conversion found."),
        };

    /// <summary>Returns a <see cref="LeaveMethod" /> object based on the provided leave method value.</summary>
    /// <param name="value">The value of the leave method to convert.</param>
    /// <returns>A <see cref="LeaveMethod" /> corresponding to the provided value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided value does not match any predefined leave method.</exception>
    public static LeaveMethod FromValue(byte value) =>
        value switch
        {
            DefaultValue => Default,
            DustValue    => Dust,
            DropValue    => Drop,
            var _        => throw new ArgumentOutOfRangeException(nameof(value), value, message: "No suitable leave method conversion found."),
        };
}

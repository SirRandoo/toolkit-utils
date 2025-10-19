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
using JetBrains.Annotations;
using UnityEngine;

namespace ToolkitUtils.Mod;

/// <summary>Represents a color with a name attached to it, like "red" or "gold".</summary>
[PublicAPI]
public sealed record NamedColor(string Name, Color Color) : IIdentifiable
{
    /// <summary>
    ///     A <see cref="UnityEngine.Color" /> object containing the red, green, blue, and alpha channels of the
    ///     associated color name.
    /// </summary>
    public Color Color { get; } = Color;

    /// <inheritdoc />
    public string Id { get; } = Name.Replace(oldValue: " ", newValue: "");

    /// <inheritdoc />
    public string Name { get; init; } = Name;
}

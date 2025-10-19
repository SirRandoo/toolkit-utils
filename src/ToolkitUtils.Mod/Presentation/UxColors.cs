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

namespace ToolkitUtils.Mod.Presentation;

/// <summary>Contains a set of colors used by the library.</summary>
[PublicAPI]
public static class UxColors
{
    public const string RedishPinkHex = "#FF87C1";
    public static readonly Color Transparent = new(r: 1f, g: 1f, b: 1f, a: 0f);
    public static readonly Color MostlyTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.25f);
    public static readonly Color HalfTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.5f);
    public static readonly Color SomewhatTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.75f);
    public static readonly Color RedishPink = new(r: 1f, g: 1.8888888f, b: 1.3212435f, a: 1f);
}

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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;
using ToolkitUtils.Mod;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>The ThingWrapper class is a generic record designed to wrap objects of type Thing.</summary>
/// <typeparam name="T">
///     A type parameter that must be a subclass of Thing. This enables the wrapper to work with any object
///     that inherits from the Thing class.
/// </typeparam>
/// <remarks>
///     This wrapper provides a basic identification interface by implementing the IIdentifiable interface, allowing
///     an object's identifier and name to be used and retrieved in a consistent manner across different instances. The Id
///     property is derived from the wrapped Thing's defName, and attempts to change the Name property will result in a
///     NotSupportedException.
/// </remarks>
public record ThingWrapper<T>(T Thing) : IIdentifiable where T : Thing
{
    /// <inheritdoc />
    public virtual string Id { get; init; } = Thing.def.defName;

    /// <inheritdoc />
    public string Name
    {
        get => Thing.Label;
        init => throw new NotSupportedException("Cannot rename things from a wrapper.");
    }
}

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

/// <summary>A set of flags that describes comparison operations between one or more values.</summary>
/// <remarks>
///     Traditional operations can be described using multiple flags, such as <see cref="Greater" /> and
///     <see cref="Equal" />. However, implementors should guard against cases where <see cref="Greater" /> and
///     <see cref="Less" /> are both set.
/// </remarks>
[Flags]
[EnumExtensions]
public enum ComparisonType
{
    /// <summary>Describes an operation that determines if one object is greater than another object.</summary>
    Greater,

    /// <summary>Describes an operation that determines if one object is less than another object.</summary>
    Less,

    /// <summary>Describes an operation that determines if one object is equal to another object.</summary>
    Equal,

    /// <summary>
    ///     Describes an operation that determines the inverse of the current comparison. For example, if one is not equal
    ///     to two, this flag will invert the final value of the comparison. This means that one would be equal to two.
    /// </summary>
    Inverse,
}

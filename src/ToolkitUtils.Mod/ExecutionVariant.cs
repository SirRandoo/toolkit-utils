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

namespace ToolkitUtils.Mod;

/// <summary>Represents a specific variant or mode of execution for game actions.</summary>
/// <remarks>
///     Variants define unique contexts or behaviors for executing actions within the game and can reflect different
///     interaction types, such as auxiliary evaluations or restricted operations.
/// </remarks>
[PublicAPI]
public readonly record struct ExecutionVariant(string Name, int Value)
{
    /// <summary>The default execution variant for an action.</summary>
    /// <remarks>
    ///     Represents the baseline behavior or the absence of specific interaction types. Typically used to indicate a
    ///     standard or non-specialized execution context.
    /// </remarks>
    public static readonly ExecutionVariant Default = new(Name: "Default", Value: 0);

    /// <summary>Represents a specific execution variant intended for verifying item prices.</summary>
    public static readonly ExecutionVariant PriceCheck = new(Name: "Price Check", Value: 1);
}

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
using ToolkitUtils.Mod.Data;

namespace ToolkitUtils.Mod;

/// <summary>
///     Encapsulates contextual information required for executing a specific operation, including the relevant
///     message, the invoking entity, and operational flags.
/// </summary>
[PublicAPI]
public sealed record ExecutionContext(Message Message, Viewer Invoker, ExecutionVariant Variant)
{
    /// <summary>
    ///     Creates a default instance of the <see cref="ExecutionContext" /> with the specified message and viewer, and
    ///     sets the execution variant to the default value.
    /// </summary>
    /// <param name="message">The message associated with this execution context.</param>
    /// <param name="viewer">The viewer or invoker for this execution context.</param>
    /// <returns>A new instance of <see cref="ExecutionContext" /> configured with the default execution variant.</returns>
    public static ExecutionContext CreateDefaultInstance(Message message, Viewer viewer) => new(message, viewer, ExecutionVariant.Default);

    /// <summary>Creates a new instance of <see cref="ExecutionContext" /> configured for a "Price Check" operation.</summary>
    /// <param name="message">The message providing contextual information for the operation.</param>
    /// <param name="viewer">The viewer invoking the operation.</param>
    /// <returns>A new instance of <see cref="ExecutionContext" /> with the "Price Check" execution variant.</returns>
    public static ExecutionContext CreatePriceCheckInstance(Message message, Viewer viewer) => new(message, viewer, ExecutionVariant.PriceCheck);
}

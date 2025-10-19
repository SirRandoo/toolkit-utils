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
using System.Threading.Tasks;
using Verse;

namespace ToolkitUtils.Api;

/// <inheritdoc cref="ICompatibilityProvider" />
public interface IUsabilityProvider : ICompatibilityProvider
{
    /// <summary>Instructs the pawn to use the given item.</summary>
    /// <param name="pawn">The pawn using the item.</param>
    /// <param name="thing">The thing being used by the pawn.</param>
    /// <returns>Whether the thing was used by the pawn.</returns>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> UseAsync(Pawn pawn, ThingDef thing);

    /// <summary>Returns whether the thing can be used by the pawn.</summary>
    /// <param name="pawn">The pawn the viewer has assigned to them.</param>
    /// <param name="thing">The thing the viewer wants to use.</param>
    /// <returns>Whether the item can be used by the pawn.</returns>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> IsUsableAsync(Pawn pawn, ThingDef thing);
}

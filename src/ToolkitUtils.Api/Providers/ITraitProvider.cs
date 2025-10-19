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
using RimWorld;
using ToolkitUtils.Mod;
using Verse;

namespace ToolkitUtils.Api;

/// <inheritdoc cref="ICompatibilityProvider" />
public interface ITraitProvider : ICompatibilityProvider
{
    /// <summary>Returns whether the trait can be purchased by the viewer.</summary>
    /// <param name="pawn">The pawn assigned to the viewer.</param>
    /// <param name="trait">The trait the viewer wants to buy.</param>
    /// <param name="severity">The severity of the trait the viewer wants to buy.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> CanPurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity);

    /// <summary>Returns whether the trait can be removed by the viewer.</summary>
    /// <param name="pawn">The pawn assigned to the viewer.</param>
    /// <param name="trait">The trait the viewer wants to remove.</param>
    /// <param name="severity">The severity of the trait the viewer wants to remove.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> CanPurchaseTraitRemovalAsync(Pawn pawn, TraitDef trait, int severity);

    /// <summary>Purchases the trait, and adds it to the viewer's pawn.</summary>
    /// <param name="pawn">The pawn the trait is being added to.</param>
    /// <param name="trait">The trait the viewer purchased.</param>
    /// <param name="severity">The severity the viewer purchased.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> PurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity);

    /// <summary>Removes the trait, and adds it to the viewer's pawn.</summary>
    /// <param name="pawn">The pawn the trait is being removed from.</param>
    /// <param name="trait">The trait the viewer removed.</param>
    /// <param name="severity">The severity the viewer removed.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> PurchaseTraitRemovalAsync(Pawn pawn, TraitDef trait, int severity);
}

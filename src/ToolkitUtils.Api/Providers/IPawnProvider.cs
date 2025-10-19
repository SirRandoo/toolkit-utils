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
public interface IPawnProvider : ICompatibilityProvider
{
    /// <summary>
    ///     Returns whether a <see cref="PawnKindDef" /> can be created with the optional accompanying
    ///     <see cref="XenotypeDef" />.
    /// </summary>
    /// <param name="kind">The <see cref="PawnKindDef" /> being checked.</param>
    /// <param name="xenotype">The accompanying <see cref="XenotypeDef" /> for the pawn kind.</param>
    /// <remarks>
    ///     <see cref="PawnKindDef" />s are the old pre-biotech way of purchasing different kinds of pawns, like androids.
    ///     Custom <see cref="PawnKindDef" />s inherently rely on the "Humanoid Alien Races" RimWorld mod in order to function
    ///     properly, and as a result implementors should reference for more information about the given pawn kind, as well as
    ///     the pawn kind's mod author.
    /// </remarks>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> CanCreateAsync(PawnKindDef kind, XenotypeDef? xenotype = null);

    /// <summary>Creates a new pawn the mod can then spawn at a given location.</summary>
    /// <param name="kind">The <see cref="PawnKindDef" /> being checked.</param>
    /// <param name="xenotype">The accompanying <see cref="XenotypeDef" /> for the pawn kind.</param>
    /// <returns>The newly created pawn.</returns>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Pawn> CreateAsync(PawnKindDef kind, XenotypeDef? xenotype = null);

    /// <summary>Transforms the pawn's body into a different body type.</summary>
    /// <param name="pawn">The pawn whose body is being transformed.</param>
    /// <param name="bodyType">The new body type of the pawn.</param>
    /// <returns>The pawn with a new body.</returns>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Pawn> TransformBodyAsync(Pawn pawn, BodyTypeDef? bodyType = null);

    /// <summary>Returns whether the pawn passed can be assigned to viewers.</summary>
    /// <param name="pawn">The pawn in question.</param>
    Result IsValidPawnCandidate(Pawn pawn);
}

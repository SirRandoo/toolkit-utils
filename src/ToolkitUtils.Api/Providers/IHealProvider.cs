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
using ToolkitUtils.Mod;
using Verse;

namespace ToolkitUtils.Api;

/// <inheritdoc cref="ICompatibilityProvider" />
public interface IHealProvider : ICompatibilityProvider
{
    /// <summary>Returns whether the heal provider can heal the given <see cref="Hediff" />.</summary>
    /// <param name="hediff">The hediff being healed.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Result> CanHealAsync(Hediff hediff);

    /// <summary>Returns whether the heal provider can heal the given <see cref="BodyPartRecord" />.</summary>
    /// <param name="pawn">The pawn whose body part is being queried about.</param>
    /// <param name="record">The body part in question.</param>
    /// <inheritdoc cref="CanHealAsync(Verse.Hediff)" path="/exception[@cref='System.NotSupportedException']" />
    Task<Result> CanHealAsync(Pawn pawn, BodyPartRecord record);

    /// <summary>Heals the given <see cref="Hediff" /> from the pawn it's attached to.</summary>
    /// <param name="hediff">The hediff getting healed from the pawn.</param>
    /// <returns>Whether the hediff was removed from the pawn.</returns>
    /// <inheritdoc cref="CanHealAsync(Verse.Hediff)" path="/exception[@cref='System.NotSupportedException']" />
    Task<Result> HealAsync(Hediff hediff);

    /// <summary>Heals the given <see cref="BodyPartRecord" /> from the pawn it's attached to.</summary>
    /// <param name="pawn">The pawn whose body part is being healed.</param>
    /// <param name="record">The body part being healed.</param>
    /// <returns>Whether the body part was re-added to the pawn.</returns>
    /// <inheritdoc cref="CanHealAsync(Verse.Hediff)" path="/exception[@cref='System.NotSupportedException']" />
    Task<Result> HealAsync(Pawn pawn, BodyPartRecord record);

    /// <summary>Attempts to resurrect the pawn.</summary>
    /// <param name="pawn">The pawn to attempt to resurrect.</param>
    /// <returns>A <see cref="Result" /> indicating whether the pawn was successfully resurrected.</returns>
    /// <inheritdoc cref="CanHealAsync(Verse.Hediff)" path="/exception[@cref='System.NotSupportedException']" />
    Task<Result> TryResurrectAsync(Pawn pawn);
}

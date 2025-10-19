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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Mod.Services;

/// <summary>Defines methods for resurrecting a pawn, with configurable options or potential side effects.</summary>
[PublicAPI]
public interface IResurrectionService
{
    /// <summary>Attempts to resurrect a specified pawn with configurable resurrection options.</summary>
    /// <param name="options">
    ///     The resurrection options which include configurable parameters such as scar chance, restoration
    ///     of missing parts, and potential side effects.
    /// </param>
    /// <param name="pawn">The pawn to be resurrected. The pawn must be dead and not discarded for the operation to succeed.</param>
    /// <returns>
    ///     A <see cref="Result" /> that represents the outcome of the resurrection operation. Returns a success result if
    ///     the resurrection completes successfully, or an error result if the operation fails (e.g., the pawn is not dead, or
    ///     the corpse is unnatural).
    /// </returns>
    Task<Result> ResurrectAsync(ResurrectionOptions options, Pawn pawn);

    /// <summary>
    ///     Attempts to resurrect a specified pawn while introducing potential side effects influenced by the body's
    ///     condition.
    /// </summary>
    /// <param name="pawn">
    ///     The pawn to be resurrected. The pawn must be dead and not discarded for the operation to succeed.
    ///     The body's condition determines the type and severity of side effects that may occur.
    /// </param>
    /// <returns>
    ///     A <see cref="Result" /> representing the outcome of the resurrection process. Returns a success result if the
    ///     resurrection and associated side effects are handled successfully, or an error result if the operation cannot be
    ///     completed.
    /// </returns>
    Task<Result> ResurrectWithSideEffectsAsync(Pawn pawn);
}

/// <summary>
///     Encapsulates various configurable parameters that govern the resurrection process of a pawn, impacting
///     behavior, side effects, and outcomes of the resurrection.
/// </summary>
public sealed record ResurrectionOptions(
    float ScarChance = 0f,
    bool CanKidnap = true,
    bool CanTimeoutOrFlee = true,
    bool Sappers = false,
    bool UseAvoidGridSmart = false,
    bool CanSteal = true,
    bool Breachers = false,
    bool CanPickupOpportunisticWeapons = false,
    bool RestoreMissingParts = true,
    bool NoLord = false,
    bool DontSpawn = false,
    bool InvisibleStun = false,
    bool RemoveDiedThoughts = true
)
{
    public static readonly ResurrectionOptions Default = new();
}

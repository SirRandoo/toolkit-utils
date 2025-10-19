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
public interface ISurgeryProvider : ICompatibilityProvider
{
    /// <summary>Returns whether the recipe given is an instance of a surgery recipe class.</summary>
    /// <param name="recipe">The recipe in question</param>
    /// <remarks>
    ///     As other mods may not directly use <see cref="RimWorld.Recipe_Surgery" />, and may instead opt to create a
    ///     custom <see cref="Verse.RecipeWorker" /> with their own functionality. As a result, deciding whether a recipe is a
    ///     surgery is up to the individual provider to decide.
    /// </remarks>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> IsSurgeryAsync(RecipeDef recipe);

    /// <summary>Returns whether the recipe is schedulable on a given pawn.</summary>
    /// <param name="recipe">The recipe in question.</param>
    /// <param name="pawn">The pawn in question.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> CanScheduleForAsync(RecipeDef recipe, Pawn pawn);
}

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
using Verse;

namespace ToolkitUtils.Api;

/// <inheritdoc cref="ICompatibilityProvider" />
public interface IItemProvider : ICompatibilityProvider
{
    /// <summary>Returns whether the meal(s) can be purchased by the viewer.</summary>
    /// <param name="meal">The meal(s) being purchased.</param>
    /// <param name="amount">The amount of meals being purchased.</param>
    /// <param name="ingredients">The ingredients within the meal(s).</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> CanCreateAsync(ThingDef meal, int amount, params ThingDef[] ingredients);

    /// <summary>Returns whether the thing(s) can be purchased by the viewer.</summary>
    /// <param name="thing">The thing(s) being purchased.</param>
    /// <param name="amount">The amount of things being purchased.</param>
    /// <param name="material">The optional material of the thing(s) being purchased.</param>
    /// <param name="quality">The optional quality of the thing(s) being purchased.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<bool> CanCreateAsync(ThingDef thing, int amount, ThingDef? material, QualityCategory? quality = null);

    /// <summary>Returns the meal(s) that was created.</summary>
    /// <param name="meal">The meal(s) purchased.</param>
    /// <param name="amount">The amount of meals purchased.</param>
    /// <param name="ingredients">The ingredients within the meal(s).</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Thing> CreateAsync(ThingDef meal, int amount, params ThingDef[] ingredients);

    /// <summary>Returns the thing(s) that was created.</summary>
    /// <param name="thing">The thing(s) purchased.</param>
    /// <param name="amount">The amount of things purchased.</param>
    /// <param name="material">The optional material of the thing(s) purchased.</param>
    /// <param name="quality">The optional quality of the thing(s) purchased.</param>
    /// <exception cref="System.NotSupportedException">
    ///     Thrown to indicate that the implementation doesn't support this specific
    ///     function of a compatibility provider.
    /// </exception>
    Task<Thing> CreateAsync(ThingDef thing, int amount, ThingDef? material, QualityCategory? quality = null);
}

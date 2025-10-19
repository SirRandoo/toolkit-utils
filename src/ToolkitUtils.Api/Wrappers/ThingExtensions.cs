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
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>Provides extension methods for the <see cref="Thing" /> class to facilitate asynchronous operations.</summary>
[PublicAPI]
public static class ThingExtensions
{
    /// <summary>Asynchronously retrieves a component of the specified type from the given Thing instance if it exists.</summary>
    /// <typeparam name="T">The type of the component to retrieve, which must derive from ThingComp.</typeparam>
    /// <param name="thing">The Thing instance from which to retrieve the component.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the component of type T if it exists, otherwise
    ///     null.
    /// </returns>
    public static async Task<T?> GetCompAsync<T>(this Thing thing) where T : ThingComp => await MainThreadExtensions.OnMainAsync(thing.TryGetComp<T>);

    /// <summary>Asynchronously retrieves the quality category of a given Thing if it has a quality component.</summary>
    /// <param name="thing">The Thing for which to retrieve the quality category.</param>
    /// <returns>
    ///     A Task representing the asynchronous operation, with a nullable QualityCategory as the result. Returns null if
    ///     the Thing does not have a quality component.
    /// </returns>
    public static async Task<QualityCategory?> GetQualityAsync(this Thing thing)
    {
        return await MainThreadExtensions.OnMainAsync(GetQuality, thing);

        QualityCategory? GetQuality(Thing t)
        {
            if (t.TryGetQuality(out QualityCategory category)) return category;

            return null;
        }
    }
}

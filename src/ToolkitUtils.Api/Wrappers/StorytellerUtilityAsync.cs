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

namespace ToolkitUtils.Api.Wrappers;

/// <summary>
///     An asynchronous utility class that provides methods related to the storyteller mechanism, leveraging
///     RimWorld's <see cref="StorytellerUtility" /> tasks.
/// </summary>
[PublicAPI]
public static class StorytellerUtilityAsync
{
    /// <summary>
    ///     Asynchronously retrieves the default incident parameters for the given incident category and target at the
    ///     current time.
    /// </summary>
    /// <param name="category">The category of the incident for which to retrieve parameters.</param>
    /// <param name="target">The target of the incident.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the default incident parameters for the
    ///     specified category and target.
    /// </returns>
    public static async Task<IncidentParms> DefaultParmsNowAsync(IncidentCategoryDef category, IIncidentTarget target) =>
        await MainThreadExtensions.OnMainAsync(StorytellerUtility.DefaultParmsNow, category, target);

    /// <summary>
    ///     Asynchronously retrieves the default threat points for the current situation based on the specified incident
    ///     target.
    /// </summary>
    /// <param name="target">The target of the incident, which provides context for calculating threat points.</param>
    /// <returns>A task that represents the asynchronous operation, containing the default threat points as a float.</returns>
    public static async Task<float> DefaultThreatPointsNowAsync(IIncidentTarget target) =>
        await MainThreadExtensions.OnMainAsync(StorytellerUtility.DefaultThreatPointsNow, target);

    /// <summary>Asynchronously calculates the default site threat points using the main thread.</summary>
    /// <returns>A task representing the asynchronous operation, with a float result indicating the default site threat points.</returns>
    public static async Task<float> DefaultSitePointsNowAsync() => await MainThreadExtensions.OnMainAsync(StorytellerUtility.DefaultSiteThreatPointsNow);
}

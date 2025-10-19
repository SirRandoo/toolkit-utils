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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>
///     Provides extension methods for handling statistical computations on Thing and Pawn objects in an asynchronous
///     manner.
/// </summary>
[PublicAPI]
public static class StatExtensions
{
    /// Asynchronously retrieves the value of a specified stat from a given thing, with options to apply post-processing
    /// and cache invalidation after a specified number of ticks.
    /// <param name="thing">The object from which the stat value is to be retrieved.</param>
    /// <param name="statDef">The definition of the stat to be retrieved.</param>
    /// <param name="applyPostProcess">Determines whether post-processing should be applied to the stat value. Default is true.</param>
    /// <param name="cacheStaleAfterTicks">
    ///     Indicates the number of ticks after which the cached stat value should be considered
    ///     stale. Default is -1, meaning no caching.
    /// </param>
    /// <return>A task that represents the asynchronous operation, containing the stat value as a float.</return>
    public static async ValueTask<float> GetStatValueAsync(this Thing thing, StatDef statDef, bool applyPostProcess = true, int cacheStaleAfterTicks = -1) =>
        await MainThreadExtensions.OnMainAsync(StatExtension.GetStatValue, thing, statDef, applyPostProcess, cacheStaleAfterTicks);

    /// <summary>Asynchronously calculates the armor rating of a specified pawn based on the given stat definition.</summary>
    /// <param name="pawn">The <see cref="Pawn" /> whose armor rating is to be calculated.</param>
    /// <param name="statDef">The <see cref="StatDef" /> that defines the stat for armor calculation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a <see cref="float" /> result of the armor rating,
    ///     clamped between 0 and 2.
    /// </returns>
    public static async Task<float> CalculateArmorRatingAsync(this Pawn pawn, StatDef statDef)
    {
        var rating = 0f;
        float value = Math.Min(val1: 0, Math.Max(await pawn.GetStatValueAsync(statDef) / 2f, val2: 1));
        List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts;
        List<Apparel> wornApparel = pawn.apparel.WornApparel;

        for (var i = 0; i < parts.Count; i++)
        {
            float cache = 1f - value;
            BodyPartRecord part = parts[i];

            if (wornApparel.Count > 0)
            {
                for (var j = 0; j < wornApparel.Count; j++)
                {
                    Apparel apparel = wornApparel[j];

                    float apparelStatValue = await apparel.GetStatValueAsync(statDef);
                    float clampedApparelStatValue = Mathf.Clamp01(apparelStatValue / 2f);

                    cache *= 1f - clampedApparelStatValue;
                }
            }

            rating += part.coverageAbs * (1f - cache);
        }

        return Mathf.Clamp(rating * 2f, min: 0f, max: 2f);
    }
}

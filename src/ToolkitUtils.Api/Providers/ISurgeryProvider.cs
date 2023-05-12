﻿// MIT License
// 
// Copyright (c) 2023 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Threading.Tasks;
using Verse;

namespace ToolkitUtils.Api
{
    /// <inheritdoc cref="ICompatibilityProvider"/>
    public interface ISurgeryProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Returns whether the recipe given is an instance of a surgery
        ///     recipe class.
        /// </summary>
        /// <param name="recipe">The recipe in question</param>
        /// <remarks>
        ///     As other mods may not directly use
        ///     <see cref="RimWorld.Recipe_Surgery"/>, and may instead opt to
        ///     create a custom <see cref="Verse.RecipeWorker"/> with their own
        ///     functionality. As a result, deciding whether a recipe is a
        ///     surgery is up to the individual provider to decide.
        /// </remarks>
        ValueTask<bool> IsSurgeryAsync(RecipeDef recipe);

        /// <summary>
        ///     Returns whether the recipe is schedulable on a given pawn.
        /// </summary>
        /// <param name="recipe">The recipe in question.</param>
        /// <param name="pawn">The pawn in question.</param>
        Task<bool> CanScheduleForAsync(RecipeDef recipe, Pawn pawn);
    }
}

// MIT License
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
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api
{
    /// <inheritdoc cref="ICompatibilityProvider"/>
    public interface IItemProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Returns whether the meal(s) can be purchased by the viewer.
        /// </summary>
        /// <param name="meal">The meal(s) being purchased.</param>
        /// <param name="amount">The amount of meals being purchased.</param>
        /// <param name="ingredients">The ingredients within the meal(s).</param>
        Task<bool> CanCreateAsync(ThingDef meal, int amount, params ThingDef[] ingredients);

        /// <summary>
        ///     Returns whether the thing(s) can be purchased by the viewer.
        /// </summary>
        /// <param name="thing">The thing(s) being purchased.</param>
        /// <param name="amount">The amount of things being purchased.</param>
        /// <param name="material">
        ///     The optional material of the thing(s) being
        ///     purchased.
        /// </param>
        /// <param name="quality">
        ///     The optional quality of the thing(s) being
        ///     purchased.
        /// </param>
        Task<bool> CanCreateAsync(ThingDef thing, int amount, [CanBeNull] ThingDef material, QualityCategory? quality);

        /// <summary>
        ///     Returns the meal(s) that was created.
        /// </summary>
        /// <param name="meal">The meal(s) purchased.</param>
        /// <param name="amount">The amount of meals purchased.</param>
        /// <param name="ingredients">The ingredients within the meal(s).</param>
        Task<Thing> CreateAsync(ThingDef meal, int amount, params ThingDef[] ingredients);

        /// <summary>
        ///     Returns the thing(s) that was created.
        /// </summary>
        /// <param name="thing">The thing(s) purchased.</param>
        /// <param name="amount">The amount of things purchased.</param>
        /// <param name="material">
        ///     The optional material of the thing(s)
        ///     purchased.
        /// </param>
        /// <param name="quality">The optional quality of the thing(s) purchased.</param>
        Task<Thing> CreateAsync(ThingDef thing, int amount, [CanBeNull] ThingDef material, QualityCategory? quality);
    }
}

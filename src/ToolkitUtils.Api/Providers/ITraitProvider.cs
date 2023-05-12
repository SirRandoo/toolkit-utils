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
using RimWorld;
using Verse;

namespace ToolkitUtils.Api
{
    /// <inheritdoc cref="ICompatibilityProvider"/>
    public interface ITraitProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Returns whether the trait can be purchased by the viewer.
        /// </summary>
        /// <param name="pawn">The pawn assigned to the viewer.</param>
        /// <param name="trait">The trait the viewer wants to buy.</param>
        /// <param name="severity">
        ///     The severity of the trait the viewer wants to
        ///     buy.
        /// </param>
        Task<bool> CanPurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity);

        /// <summary>
        ///     Purchases the trait, and adds it to the viewer's pawn.
        /// </summary>
        /// <param name="pawn">The pawn the trait is being added to.</param>
        /// <param name="trait">The trait the viewer purchased.</param>
        /// <param name="severity">The severity the viewer purchased.</param>
        Task<bool> PurchaseTraitAsync(Pawn pawn, TraitDef trait, int severity);
    }
}

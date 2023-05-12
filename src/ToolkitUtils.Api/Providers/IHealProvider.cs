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
using Verse;

namespace ToolkitUtils.Api
{
    /// <inheritdoc cref="ICompatibilityProvider"/>
    public interface IHealProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Returns whether the heal provider can heal the given
        ///     <see cref="Hediff"/>.
        /// </summary>
        /// <param name="hediff">The hediff being healed.</param>
        Task<bool> CanHeal(Hediff hediff);

        /// <summary>
        ///     Returns whether the heal provider can heal the given
        ///     <see cref="BodyPartRecord"/>.
        /// </summary>
        /// <param name="record">The body part in question.</param>
        Task<bool> CanHeal(BodyPartRecord record);

        /// <summary>
        ///     Heals the given <see cref="Hediff"/> from the pawn it's attached
        ///     to.
        /// </summary>
        /// <param name="hediff">
        ///     The hediff getting healed from the pawn it;s
        ///     own.
        /// </param>
        /// <returns>Whether the hediff was removed from the pawn.</returns>
        Task<bool> HealAsync(Hediff hediff);

        /// <summary>
        ///     Heals the given <see cref="BodyPartRecord"/> from the pawn it's
        ///     attached to.
        /// </summary>
        /// <param name="record">The body part being healed.</param>
        /// <returns>Whether the body part was re-added to the pawn.</returns>
        Task<bool> HealAsync(BodyPartRecord record);
    }
}

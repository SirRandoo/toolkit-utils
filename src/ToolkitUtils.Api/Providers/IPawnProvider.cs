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
    public interface IPawnProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Returns whether a <see cref="PawnKindDef"/> can be created with
        ///     the optional accompanying <see cref="XenotypeDef"/>.
        /// </summary>
        /// <param name="kind">The <see cref="PawnKindDef"/> being checked.</param>
        /// <param name="xenotype">
        ///     The accompanying <see cref="XenotypeDef"/> for
        ///     the pawn kind.
        /// </param>
        /// <remarks>
        ///     <see cref="PawnKindDef"/>s are the old pre-biotech way of
        ///     purchasing different kinds of pawns, like androids. Custom
        ///     <see cref="PawnKindDef"/>s inherently rely on the "Humanoid Alien
        ///     Races" RimWorld mod in order to function properly, and as a
        ///     result implementors should reference for more information about
        ///     the given pawn kind, as well as the pawn kind's mod author.
        /// </remarks>
        Task<bool> CanCreateAsync(PawnKindDef kind, [CanBeNull] XenotypeDef xenotype);

        /// <summary>
        ///     Creates a new pawn the mod can then spawn at a given location.
        /// </summary>
        /// <param name="kind">The <see cref="PawnKindDef"/> being checked.</param>
        /// <param name="xenotype">
        ///     The accompanying <see cref="XenotypeDef"/> for
        ///     the pawn kind.
        /// </param>
        /// <returns>The newly created pawn.</returns>
        Task<Pawn> CreateAsync(PawnKindDef kind, [CanBeNull] XenotypeDef xenotype);
    }
}

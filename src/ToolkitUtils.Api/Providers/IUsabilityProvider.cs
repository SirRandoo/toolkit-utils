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
    public interface IUsabilityProvider : ICompatibilityProvider
    {
        /// <summary>
        ///     Instructs the pawn to use the given item.
        /// </summary>
        /// <param name="pawn">The pawn using the item.</param>
        /// <param name="thing">The thing being used by the pawn.</param>
        /// <returns>Whether the thing was used by the pawn.</returns>
        Task<bool> UseAsync(Pawn pawn, ThingDef thing);

        /// <summary>
        ///     Returns whether the thing can be used by the pawn.
        /// </summary>
        /// <param name="pawn">The pawn the viewer has assigned to them.</param>
        /// <param name="thing">The thing the viewer wants to use.</param>
        /// <returns>Whether the item can be used by the pawn.</returns>
        Task<bool> IsUsableAsync(Pawn pawn, ThingDef thing);
    }
}

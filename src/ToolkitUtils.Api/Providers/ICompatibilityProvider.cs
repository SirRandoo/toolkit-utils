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

using ToolkitUtils.Data.Models;

namespace ToolkitUtils.Api
{
    /// <summary>
    ///     Represents a special object capable of changing the behavior of
    ///     certain aspects of the mod's functions.
    /// </summary>
    public interface ICompatibilityProvider : IIdentifiable
    {
        /// <summary>
        ///     The relative priority of the compatibility provider. Providers
        ///     with a higher priority will execute before providers with a lower
        ///     priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     A collection of mod ids that are required for before this
        ///     compatibility provider will alter the mod's functions.
        /// </summary>
        string[] RequiredMods { get; }
    }
}

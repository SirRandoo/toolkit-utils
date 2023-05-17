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

using TwitchToolkit.Incidents;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     A wrapper for housing <see cref="StoreIncident"/>s. This wrapper
    ///     serves as a compatibility layer between the new ToolkitUtils, and
    ///     the old Twitch Toolkit.
    /// </summary>
    public class IncidentWrapper : DefWrapper<StoreIncident>
    {
        /// <inheritdoc/>
        public IncidentWrapper(StoreIncident def) : base(def)
        {
        }

        /// <summary>
        ///     The string a viewer must type in order to purchase the given
        ///     incident.
        /// </summary>
        /// <remarks>
        ///     Implementors should ensure that the value of this property is
        ///     compared case-insensitively to preserve the experience users have
        ///     come to expect from Twitch Toolkit, and because case sensitivity
        ///     is an awful experience on mobile.
        /// </remarks>
        public string Code
        {
            get => Def.abbreviation;
            set => Def.abbreviation = value;
        }
    }
}

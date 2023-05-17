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

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     Represents a mod within the game, and its associated data.
    /// </summary>
    public interface IMod : IIdentifiable
    {
        /// <summary>
        ///     The current version of the mod.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        ///     A list of people who wrote, and currently maintain the mod.
        /// </summary>
        string[] Authors { get; set; }

        /// <summary>
        ///     An unsigned int64 representing the mod's id on the Steam
        ///     workshop.
        /// </summary>
        ulong SteamId { get; set; }
    }
}

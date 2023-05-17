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

using UnityEngine;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     Represents an action executed in the "Editor," ToolkitUtils' menu
    ///     for editing the shop in bulk in a variety of different ways.
    /// </summary>
    public interface IEditorAction : IIdentifiable
    {
        /// <summary>
        ///     Requests the action prepare its state for display in the editor.
        /// </summary>
        /// <remarks>
        ///     Typically this method is where you'd fetch objects that'd only
        ///     require one initialization per editor display, like translation
        ///     strings.
        /// </remarks>
        void Prepare();

        /// <summary>
        ///     Draws the action in the given region of the screen.
        /// </summary>
        /// <param name="region">The region the action should be drawn in.</param>
        void Draw(Rect region);
    }
}

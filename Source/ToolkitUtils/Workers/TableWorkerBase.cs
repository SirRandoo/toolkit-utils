// MIT License
// 
// Copyright (c) 2021 SirRandoo
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
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    /// <summary>
    ///     The base implementation of table worker, a way of drawing a table
    ///     of store data in a portable, reusable way.
    /// </summary>
    public abstract class TableWorkerBase
    {
        protected float LineHeight => Mathf.FloorToInt(Text.LineHeight);
        protected float RowLineHeight => Mathf.FloorToInt(Text.LineHeight * 1.333f);
        protected abstract void DrawHeaders(Rect region);
        protected abstract void DrawTableContents(Rect region);

        /// <summary>
        ///     Called once to prepare the worker's internal state, like
        ///     translation strings.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        ///     Called once when the table's contents need to be resorted
        ///     according to its internal sort order.
        /// </summary>
        public abstract void NotifySortRequested();

        /// <summary>
        ///     Called once when the table's contents need to filtered based
        ///     on the a query given by a user or the worker's containing class.
        /// </summary>
        /// <param name="query">The query to filter contents by</param>
        public abstract void NotifySearchRequested(string query);

        /// <summary>
        ///     Called once to notify the table its available draw region was
        ///     adjusted in some way. The table will recalculate its internal
        ///     <see cref="Rect"/>s to adjust to the new resolution.
        /// </summary>
        /// <param name="region">The new region the table will be drawn in</param>
        public abstract void NotifyResolutionChanged(Rect region);
        private protected abstract void FilterDataBySearch(string query);

        /// <summary>
        ///     Draws the table as-is in the region given.
        /// </summary>
        /// <param name="region">The region to draw the table in</param>
        public void Draw(Rect region)
        {
            var headerRow = new Rect(region.x, region.y, region.width, LineHeight);
            var tableContents = new Rect(region.x, region.y + headerRow.height, region.width, region.height - headerRow.height);

            GUI.BeginGroup(headerRow);
            DrawHeaders(new Rect(0f, 0f, headerRow.width, headerRow.height));
            GUI.EndGroup();

            GUI.BeginGroup(tableContents);
            DrawTableContents(new Rect(0f, 0f, tableContents.width, tableContents.height));
            GUI.EndGroup();
        }
    }
}

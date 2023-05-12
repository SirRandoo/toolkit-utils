// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using UnityEngine;
using Verse;

namespace ToolkitUtils.Workers
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

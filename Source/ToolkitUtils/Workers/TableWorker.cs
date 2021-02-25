﻿// ToolkitUtils
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

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public enum SortOrder { Ascending, Descending }

    public abstract class TableWorker<T>
    {
        private protected List<T> _data;

        protected float LineHeight => Mathf.FloorToInt(Text.LineHeight);

        protected float RowLineHeight => Mathf.FloorToInt(Text.LineHeight * 1.333f);

        public IEnumerable<T> Data => _data;

        public void Draw(Rect canvas)
        {
            var headerRow = new Rect(canvas.x, canvas.y, canvas.width, LineHeight);
            var tableContents = new Rect(
                canvas.x,
                canvas.y + headerRow.height,
                canvas.width,
                canvas.height - headerRow.height
            );

            GUI.BeginGroup(headerRow);
            DrawHeaders(new Rect(0f, 0f, headerRow.width, headerRow.height));
            GUI.EndGroup();

            GUI.BeginGroup(tableContents);
            DrawTableContents(new Rect(0f, 0f, tableContents.width, tableContents.height));
            GUI.EndGroup();
        }

        public abstract void DrawHeaders(Rect canvas);
        public abstract void DrawTableContents(Rect canvas);
        public abstract void Prepare();
        public abstract void EnsureExists(T data);
        public abstract void NotifySortRequested();
        public abstract void NotifySearchRequested(string query);
        public abstract void NotifyResolutionChanged(Rect canvas);
        public abstract void NotifyCustomSearchRequested(Func<T, bool> worker);
        protected abstract void FilterDataBySearch(string query);
    }
}

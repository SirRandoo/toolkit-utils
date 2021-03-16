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
    public abstract class TableWorkerBase
    {
        protected float LineHeight => Mathf.FloorToInt(Text.LineHeight);
        protected float RowLineHeight => Mathf.FloorToInt(Text.LineHeight * 1.333f);
        public abstract void DrawHeaders(Rect canvas);
        public abstract void DrawTableContents(Rect canvas);
        public abstract void Prepare();
        public abstract void NotifySortRequested();
        public abstract void NotifySearchRequested(string query);
        public abstract void NotifyResolutionChanged(Rect canvas);
        protected abstract void FilterDataBySearch(string query);

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
    }
}

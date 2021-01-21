using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public enum SortOrder { Ascending, Descending }

    public abstract class TableWorker<T>
    {
        protected List<T> Data;

        protected float LineHeight => Mathf.FloorToInt(Text.LineHeight);

        protected float RowLineHeight => Mathf.FloorToInt(Text.LineHeight * 1.333f);

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

        public abstract void NotifySortRequested();
        public abstract void NotifySearchRequested(string query);
        public abstract void NotifyResolutionChanged(Rect canvas);
        protected abstract void FilterDataBySearch(string query);
    }
}

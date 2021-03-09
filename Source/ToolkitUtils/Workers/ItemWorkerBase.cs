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

namespace SirRandoo.ToolkitUtils.Workers
{
    public abstract class ItemWorkerBase
    {
        private protected Rect ModifierRect = Rect.zero;
        private protected Rect SelectorRect = Rect.zero;

        private protected Rect TableRect = Rect.zero;
        private ItemTableWorker worker;

        protected ItemWorkerBase()
        {
            worker = new ItemTableWorker();
        }

        public void Draw()
        {
            GUI.BeginGroup(SelectorRect);
            DrawSelectorMenu(SelectorRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(ModifierRect);
            DrawModifierMenu(ModifierRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(TableRect);
            worker.Draw(TableRect);
            GUI.EndGroup();
        }

        public abstract void DrawSelectorMenu(Rect canvas);
        public abstract void DrawModifierMenu(Rect canvas);

        public void NotifyResolutionChanged(Rect canvas)
        {
            var userRect = new Rect(0f, 0f, canvas.width, Mathf.FloorToInt(canvas.height / 2f) - 2f);
            SelectorRect = new Rect(0f, 0f, Mathf.FloorToInt(userRect.width / 2f) - 2f, userRect.height);
            ModifierRect = new Rect(SelectorRect.x + SelectorRect.width + 4f, 0f, SelectorRect.width, userRect.height);

            TableRect = new Rect(0f, userRect.y + 4f, canvas.width, canvas.height - userRect.height - 4f);
        }
    }
}

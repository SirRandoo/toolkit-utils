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

using System;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class TabItem
    {
        private string label;

        public string Label
        {
            get => label;
            set
            {
                label = value;
                Width = Text.CalcSize(label).x;
            }
        }

        public float Width { get; private set; }

        public Action<Rect> ContentDrawer { get; set; }
        public Func<bool> Clicked { get; set; }

        public void Draw(Rect canvas, float margins = 16f)
        {
            GUI.color = new Color(0.46f, 0.49f, 0.5f);
            Widgets.DrawHighlight(canvas);
            GUI.color = Color.white;

            Rect transformedCanvas = canvas.ContractedBy(margins);
            GUI.BeginGroup(transformedCanvas);
            ContentDrawer(new Rect(0f, 0f, transformedCanvas.width, transformedCanvas.height));
            GUI.EndGroup();
        }
    }
}

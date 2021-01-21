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

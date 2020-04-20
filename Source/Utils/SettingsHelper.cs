using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class SettingsHelper
    {
        public static bool DrawClearButton(Rect canvas)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "×", false);

            var clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Event.current.clickCount > 0;

            if (!clicked)
            {
                return false;
            }

            GUI.FocusControl(null);
            return true;
        }
    }
}

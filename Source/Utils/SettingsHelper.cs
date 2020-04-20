using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class SettingsHelper
    {
        public static bool DrawClearButton(Rect canvas)
        {
            return Widgets.ButtonText(canvas, "×", false);
        }
    }
}

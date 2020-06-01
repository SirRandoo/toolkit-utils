using HarmonyLib;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_Viewers), "DoWindowContents")]
    public static class ViewerWindowPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Rect inRect)
        {
            TaggedString text = "TKUtils.Windows.Purge.Button.Label".Translate();
            float width = Text.CalcSize(text).x * 1.5f;
            var canvas = new Rect(inRect.width - width, 0f, width, 28f);

            if (Widgets.ButtonText(canvas, "TKUtils.Windows.Purge.Button.Label".Translate()))
            {
                Find.WindowStack.Add(new PurgeViewersDialog());
            }
        }
    }
}

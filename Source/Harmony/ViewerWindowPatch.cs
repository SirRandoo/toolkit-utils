using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_Viewers), "DoWindowContents")]
    [UsedImplicitly]
    public static class ViewerWindowPatch
    {
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Postfix(Rect inRect)
        {
            string text = "TKUtils.Purge.Button".Localize();
            float width = Text.CalcSize(text).x * 1.5f;
            var canvas = new Rect(inRect.width - width, 0f, width, 28f);

            if (Widgets.ButtonText(canvas, text))
            {
                Find.WindowStack.Add(new PurgeViewersDialog());
            }
        }
    }
}

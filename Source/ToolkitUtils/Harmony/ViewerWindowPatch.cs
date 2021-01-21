using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window_Viewers), "DoWindowContents")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class ViewerWindowPatch
    {
        public static void Postfix(Rect inRect)
        {
            string text = "TKUtils.Buttons.Purge".Localize();
            float width = Text.CalcSize(text).x + 16f;
            var canvas = new Rect(inRect.width - width, 0f, width, 28f);

            if (Widgets.ButtonText(canvas, text))
            {
                Find.WindowStack.Add(new PurgeViewersDialog());
            }
        }
    }
}

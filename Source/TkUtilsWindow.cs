using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class TkUtilsWindow : ToolkitWindow
    {
        public TkUtilsWindow(Mod mod) : base(mod)
        {
            Mod = mod;

            doCloseButton = false;
            doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }
    }
}

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
        }

        public override void DoWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 40f - CloseButSize.y));
        }
    }
}

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class ToolkitWindow : TwitchToolkit.Settings.ToolkitWindow
    {
        public ToolkitWindow(Mod mod) : base(mod) => this.Mod = mod;

        public override void DoWindowContents(Rect inRect) => this.Mod.GetSettings<TKSettings>().DoWindowContents(inRect);
    }
}

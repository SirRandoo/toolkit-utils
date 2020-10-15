using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class ModConstraint : ConstraintBase
    {
        private readonly string labelText;

        public ModConstraint()
        {
            labelText = "TKUtils.PurgeMenu.Mod".Localize().CapitalizeFirst();
        }

        public override void Draw(Rect canvas)
        {
            SettingsHelper.DrawLabel(canvas, labelText);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.mod;
        }
    }
}

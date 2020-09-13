using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class VipConstraint : ConstraintBase
    {
        private readonly string labelText;

        public VipConstraint()
        {
            labelText = "TKUtils.PurgeMenu.Vip".Localize().CapitalizeFirst();
        }

        public override void Draw(Rect canvas)
        {
            SettingsHelper.DrawLabelAnchored(canvas, labelText, TextAnchor.MiddleLeft);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.IsVIP;
        }
    }
}

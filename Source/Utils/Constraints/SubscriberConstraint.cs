using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class SubscriberConstraint : ConstraintBase
    {
        private readonly string labelText;

        public SubscriberConstraint()
        {
            labelText = "TKUtils.PurgeMenu.Sub".Localize().CapitalizeFirst();
        }

        public override void Draw(Rect canvas)
        {
            SettingsHelper.DrawLabel(canvas, labelText);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.IsSub;
        }
    }
}

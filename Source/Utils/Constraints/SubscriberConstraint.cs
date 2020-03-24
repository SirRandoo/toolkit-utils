using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class SubscriberConstraint : ConstraintBase
    {
        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, "TKUtils.Windows.Purge.Constraints.Sub".Translate());
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.IsSub;
        }
    }
}

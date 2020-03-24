using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class VipConstraint : ConstraintBase
    {
        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, "TKUtils.Windows.Purge.Constraints.Vip".Translate());
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.IsVIP;
        }
    }
}

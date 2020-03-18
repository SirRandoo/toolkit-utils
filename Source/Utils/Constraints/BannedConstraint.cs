using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public class BannedConstraint : ConstraintBase
    {
        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, "TKUtils.Windows.Purge.Constraints.Banned".Translate());
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return viewer.IsBanned;
        }
    }
}

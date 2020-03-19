using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public class ModConstraint : ConstraintBase
    {
        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, "TKUtils.Windows.Purge.Constraints.Mod".Translate());
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.mod;
        }
    }
}

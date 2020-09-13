using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class BannedConstraint : ConstraintBase
    {
        private readonly string labelText;

        public BannedConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Banned".Localize();
        }

        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, labelText);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return viewer.IsBanned;
        }
    }
}

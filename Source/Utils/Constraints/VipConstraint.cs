using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    [UsedImplicitly]
    public class VipConstraint : ConstraintBase
    {
        private readonly string labelText;

        public VipConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Vip".Localize();
        }

        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, labelText);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.IsVIP;
        }
    }
}

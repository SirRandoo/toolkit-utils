using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    [UsedImplicitly]
    public class ModConstraint : ConstraintBase
    {
        private readonly string labelText;

        public ModConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Mod".Localize();
        }

        public override void Draw(Rect canvas)
        {
            Widgets.Label(canvas, labelText);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            return !viewer.mod;
        }
    }
}

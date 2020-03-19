using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public class KarmaConstraint : ComparableConstraint
    {
        private string buffer = "0";
        private int karma = 0;

        public override void Draw(Rect canvas)
        {
            var right = canvas.RightHalf();

            Widgets.Label(canvas.LeftHalf(), "TKUtils.Windows.Purge.Constraints.Karma".Translate());
            DrawButton(right.LeftHalf());
            Widgets.TextFieldNumeric(right.RightHalf(), ref karma, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch(Strategy)
            {
                case ComparisonTypes.Equal:
                    return viewer.karma == karma;

                case ComparisonTypes.Greater:
                    return viewer.karma > karma;

                case ComparisonTypes.Less:
                    return viewer.karma < karma;

                case ComparisonTypes.GreaterEqual:
                    return viewer.karma >= karma;

                case ComparisonTypes.LessEqual:
                    return viewer.karma <= karma;

                default:
                    return false;
            }
        }
    }
}

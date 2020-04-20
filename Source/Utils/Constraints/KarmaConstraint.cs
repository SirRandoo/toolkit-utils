using System;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class KarmaConstraint : ComparableConstraint
    {
        private string buffer = "0";
        private int karma;

        public override void Draw(Rect canvas)
        {
            var right = canvas.RightHalf().Rounded();
            var newWidth = (float) Math.Floor(right.width / 1.5);

            right = new Rect(canvas.width - newWidth, canvas.y, newWidth, canvas.height).Rounded();
            var left = new Rect(canvas.x, canvas.y, canvas.width - right.width, canvas.height).Rounded();

            Widgets.Label(left, "TKUtils.Windows.Purge.Constraints.Karma".Translate());
            DrawButton(new Rect(right.x, right.y, 50f, right.height));
            Widgets.TextFieldNumeric(
                new Rect(right.x + 55f, right.y, right.width - 55f, right.height),
                ref karma,
                ref buffer
            );
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Strategy)
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

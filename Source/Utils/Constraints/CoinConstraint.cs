using TwitchToolkit;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public class CoinConstraint : ComparableConstraint
    {
        private string buffer = "0";
        private int coins = 0;

        public override void Draw(Rect canvas)
        {
            var right = canvas.RightHalf();
            var buttonRect = new Rect(right.x, right.y, (right.width * 0.25f) - 10f, right.height);
            var fieldRect = new Rect(buttonRect.x + buttonRect.width + 10f, buttonRect.y, (right.width * 0.75f) - 10f, right.height);

            Widgets.Label(canvas.LeftHalf(), "TKUtils.Windows.Purge.Constraints.Coins".Translate());

            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(fieldRect, ref coins, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch(Strategy)
            {
                case ComparisonTypes.Equal:
                    return viewer.coins == coins;

                case ComparisonTypes.Greater:
                    return viewer.coins > coins;

                case ComparisonTypes.Less:
                    return viewer.coins < coins;

                case ComparisonTypes.GreaterEqual:
                    return viewer.coins >= coins;

                case ComparisonTypes.LessEqual:
                    return viewer.coins <= coins;

                default:
                    return false;
            }
        }
    }
}

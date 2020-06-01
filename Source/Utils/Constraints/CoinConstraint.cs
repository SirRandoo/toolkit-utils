using System;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CoinConstraint : ComparableConstraint
    {
        private string buffer = "0";
        private int coins;

        public override void Draw(Rect canvas)
        {
            Rect right = canvas.RightHalf().Rounded();
            var newWidth = (float) Math.Floor(right.width / 1.5);

            right = new Rect(canvas.width - newWidth, canvas.y, newWidth, canvas.height).Rounded();
            Rect left = new Rect(canvas.x, canvas.y, canvas.width - right.width, canvas.height).Rounded();

            var buttonRect = new Rect(right.x, right.y, 50f, right.height);
            var fieldRect = new Rect(
                buttonRect.x + 55f,
                buttonRect.y,
                right.width - 55f,
                right.height
            );

            Widgets.Label(left, "TKUtils.Windows.Purge.Constraints.Coins".Translate());

            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(fieldRect, ref coins, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Strategy)
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

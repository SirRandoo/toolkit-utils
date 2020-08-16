using System;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CoinConstraint : ComparableConstraint
    {
        private readonly string labelText;
        private string buffer = "0";
        private int coins;

        public CoinConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Coins".Localize();
        }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);

            Widgets.Label(labelRect, labelText);

            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(inputRect, ref coins, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Comparison)
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

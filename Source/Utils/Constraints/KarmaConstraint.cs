using System;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class KarmaConstraint : ComparableConstraint
    {
        private readonly string labelText;
        private string buffer = "0";
        private int karma;

        public KarmaConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Karma".Localize();
        }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);

            Widgets.Label(labelRect, labelText);
            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(inputRect, ref karma, ref buffer);
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            switch (Comparison)
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

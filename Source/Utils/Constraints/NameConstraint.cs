using System;
using System.Linq;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum NameComparisonTypes { Is, Not }

    public class NameConstraint : ConstraintBase
    {
        private NameComparisonTypes nameStrategy = NameComparisonTypes.Is;
        private string username;

        public override void Draw(Rect canvas)
        {
            var right = canvas.RightHalf().Rounded();
            var newWidth = (float) Math.Floor(right.width / 3);

            right = new Rect(canvas.width - newWidth, canvas.y, newWidth, canvas.height).Rounded();
            var left = new Rect(canvas.x, canvas.y, canvas.width - right.width, canvas.height).Rounded();
            var rightWidth = right.width * 0.3f;

            Widgets.Label(left, "TKUtils.Windows.Purge.Constraints.Name".Translate());

            if (Widgets.ButtonText(
                new Rect(right.x, right.y, rightWidth, right.height),
                $"TKUtils.Windows.Purge.NameComparisonTypes.{Enum.GetName(typeof(NameComparisonTypes), nameStrategy)}"
                    .Translate()
            ))
            {
                var names = Enum.GetNames(typeof(NameComparisonTypes));
                var options = new List<FloatMenuOption>();

                foreach(var name in names)
                {
                    options.Add(
                        new FloatMenuOption(
                            $"TKUtils.Windows.Purge.NameComparisonTypes.{name}".Translate(),
                            () => nameStrategy = (NameComparisonTypes) Enum.Parse(typeof(NameComparisonTypes), name)
                        )
                    );
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }

            username = Widgets.TextField(
                new Rect(right.x + rightWidth, right.y, right.width - rightWidth, right.height),
                username
            );
        }

        public void SetComparison(NameComparisonTypes type)
        {
            nameStrategy = type;
        }

        public void SetUsername(string name)
        {
            username = name;
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            var result = viewer.username.EqualsIgnoreCase(username);

            return nameStrategy == NameComparisonTypes.Is ? result : !result;
        }
    }
}

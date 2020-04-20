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
            var newWidth = (float) Math.Floor(right.width / 1.5);

            right = new Rect(canvas.width - newWidth, canvas.y, newWidth, canvas.height).Rounded();
            var left = new Rect(canvas.x, canvas.y, canvas.width - right.width, canvas.height).Rounded();

            Widgets.Label(left, "TKUtils.Windows.Purge.Constraints.Name".Translate());

            if (Widgets.ButtonText(
                new Rect(right.x, right.y, 50f, right.height),
                $"TKUtils.Windows.Purge.NameComparisonTypes.{Enum.GetName(typeof(NameComparisonTypes), nameStrategy)}"
                    .Translate()
            ))
            {
                var names = Enum.GetNames(typeof(NameComparisonTypes));
                var options = names.Select(
                        name => new FloatMenuOption(
                            $"TKUtils.Windows.Purge.NameComparisonTypes.{name}".Translate(),
                            () => nameStrategy = (NameComparisonTypes) Enum.Parse(typeof(NameComparisonTypes), name)
                        )
                    )
                    .ToList();

                Find.WindowStack.Add(new FloatMenu(options));
            }

            username = Widgets.TextField(
                new Rect(right.x + 55f, right.y, right.width - 55f, right.height),
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

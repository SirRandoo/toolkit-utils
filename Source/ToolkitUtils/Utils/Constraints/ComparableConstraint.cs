using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum ComparisonTypes { Greater, Equal, Less, GreaterEqual, LessEqual }

    public class ComparableConstraint : ConstraintBase
    {
        private readonly List<FloatMenuOption> comparisonOptions;
        private ComparisonTypes comparison;
        private string comparisonButtonText;
        private string comparisonText;

        protected ComparableConstraint()
        {
            Comparison = ComparisonTypes.Equal;
            comparisonOptions = Enum.GetNames(typeof(ComparisonTypes))
               .Select(
                    t => new FloatMenuOption(
                        $"TKUtils.PurgeMenu.{t}".Localize(),
                        () => Comparison = (ComparisonTypes) Enum.Parse(typeof(ComparisonTypes), t)
                    )
                )
               .ToList();
        }

        protected ComparisonTypes Comparison
        {
            get => comparison;
            private set
            {
                if (comparison != value)
                {
                    comparisonText = Enum.GetName(typeof(ComparisonTypes), value);
                    comparisonButtonText = $"TKUtils.PurgeMenu.{comparisonText}".Localize();
                }

                comparison = value;
            }
        }

        protected void DrawButton(Rect canvas)
        {
            if (!Widgets.ButtonText(canvas, comparisonButtonText))
            {
                return;
            }

            Find.WindowStack.Add(new FloatMenu(comparisonOptions));
        }
    }
}

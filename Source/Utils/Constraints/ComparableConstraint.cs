using System;
using System.Collections.Generic;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Constraints
{
    public enum ComparisonTypes
    {
        Greater,
        Equal,
        Less,
        GreaterEqual,
        LessEqual
    }

    public class ComparableConstraint : ConstraintBase
    {
        protected ComparisonTypes Strategy = ComparisonTypes.Equal;

        protected static TaggedString GetStrategyString(string root, ComparisonTypes strategy)
        {
            return $"{root}.{Enum.GetName(typeof(ComparisonTypes), strategy)}".Translate();
        }

        protected void DrawButton(Rect canvas)
        {
            var text = GetStrategyString("TKUtils.Windows.Purge.ComparisonTypes", Strategy);

            if(!text.NullOrEmpty() && Widgets.ButtonText(canvas, text))
            {
                var names = Enum.GetNames(typeof(ComparisonTypes));
                var options = new List<FloatMenuOption>();

                foreach(var name in names)
                {
                    options.Add(
                        new FloatMenuOption(
                            $"TKUtils.Windows.Purge.ComparisonTypes.{name}".Translate(),
                            () => Strategy = (ComparisonTypes) Enum.Parse(typeof(ComparisonTypes), name)
                        )
                    );
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }
        }
    }
}

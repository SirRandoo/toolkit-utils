using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum ComparisonTypes
    {
        Greater, Equal, Less,
        GreaterEqual, LessEqual
    }

    public class ComparableConstraint : ConstraintBase
    {
        protected ComparisonTypes Strategy = ComparisonTypes.Equal;

        private static TaggedString GetStrategyString(string root, ComparisonTypes strategy)
        {
            return $"{root}.{Enum.GetName(typeof(ComparisonTypes), strategy)}".Translate();
        }

        protected void DrawButton(Rect canvas)
        {
            TaggedString text = GetStrategyString("TKUtils.Windows.Purge.ComparisonTypes", Strategy);

            if (text.NullOrEmpty() || !Widgets.ButtonText(canvas, text))
            {
                return;
            }

            string[] names = Enum.GetNames(typeof(ComparisonTypes));
            List<FloatMenuOption> options = names.Select(
                    name => new FloatMenuOption(
                        $"TKUtils.Windows.Purge.ComparisonTypes.{name}".Translate(),
                        () => Strategy = (ComparisonTypes) Enum.Parse(typeof(ComparisonTypes), name)
                    )
                )
                .ToList();

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}

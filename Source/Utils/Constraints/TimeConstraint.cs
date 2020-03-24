using System;
using System.Linq;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum TimeScale
    {
        Years, Months, Days,
        Hours
    }

    public class TimeConstraint : ComparableConstraint
    {
        private string buffer = "0";
        private TimeScale timeScale = TimeScale.Days;
        private int value;

        public override void Draw(Rect canvas)
        {
            var right = canvas.RightHalf().Rounded();
            var newWidth = (float) Math.Floor(right.width / 3);

            right = new Rect(canvas.width - newWidth, canvas.y, newWidth, canvas.height).Rounded();
            var left = new Rect(canvas.x, canvas.y, canvas.width - right.width, canvas.height).Rounded();
            var rightWidth = right.width * 0.3f;

            Widgets.Label(left, "TKUtils.Windows.Purge.Constraints.Time".Translate());
            DrawButton(new Rect(right.x - 20f, right.y, rightWidth, right.height));
            Widgets.TextFieldNumeric(
                new Rect(right.x + rightWidth - 10f, right.y, right.width - (rightWidth * 2), right.height),
                ref value,
                ref buffer
            );

            if (!Widgets.ButtonText(
                new Rect(canvas.width - rightWidth, right.y, rightWidth, right.height),
                $"TKUtils.Windows.Purge.TimeScales.{Enum.GetName(typeof(TimeScale), timeScale)}".Translate()
            ))
            {
                return;
            }

            var names = Enum.GetNames(typeof(TimeScale));
            var options = names.Select(
                    name => new FloatMenuOption(
                        $"TKUtils.Windows.Purge.TimeScales.{name}".Translate(),
                        () => timeScale = (TimeScale) Enum.Parse(typeof(TimeScale), name)
                    )
                )
                .ToList();

            Find.WindowStack.Add(new FloatMenu(options));
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            var seconds = value * ScaleToInt(timeScale);
            var span = DateTime.Now - viewer.last_seen;

            switch (Strategy)
            {
                case ComparisonTypes.Equal:
                    return (int) span.TotalSeconds == seconds;

                case ComparisonTypes.Greater:
                    return span.TotalSeconds > seconds;

                case ComparisonTypes.Less:
                    return span.TotalSeconds < seconds;

                case ComparisonTypes.GreaterEqual:
                    return span.TotalSeconds >= seconds;

                case ComparisonTypes.LessEqual:
                    return span.TotalSeconds <= seconds;

                default:
                    return false;
            }
        }

        private static int ScaleToInt(TimeScale scale)
        {
            switch (scale)
            {
                case TimeScale.Days:
                    return 60 * 24;

                case TimeScale.Hours:
                    return 60;

                case TimeScale.Months:
                    return 60 * 24 * 30;

                case TimeScale.Years:
                    return 60 * 24 * 30 * 12;

                default:
                    return 0;
            }
        }
    }
}

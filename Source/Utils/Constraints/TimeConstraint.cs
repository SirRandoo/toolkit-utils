using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum TimeScales { Years, Months, Days, Hours }

    [UsedImplicitly]
    public class TimeConstraint : ComparableConstraint
    {
        private readonly string labelText;
        private readonly List<FloatMenuOption> scaleOptions;
        private string buffer = "0";
        private TimeScales timeScale = TimeScales.Days;
        private string timeScaleButtonText;
        private string timeScaleText = nameof(TimeScales.Days);
        private int value;

        public TimeConstraint()
        {
            labelText = "TKUtils.Windows.Purge.Constraints.Time".Localize();
            timeScaleButtonText = $"TKUtils.Windows.Purge.TimeScales.{timeScaleText}".Localize();

            scaleOptions = Enum.GetNames(typeof(TimeScales))
               .Select(
                    s => new FloatMenuOption(
                        $"TKUtils.Windows.Purge.TimeScales.{s}".Localize(),
                        () => TimeScale = (TimeScales) Enum.Parse(typeof(TimeScales), s)
                    )
                )
               .ToList();
        }

        private TimeScales TimeScale
        {
            get => timeScale;
            set
            {
                if (timeScale != value)
                {
                    timeScaleText = Enum.GetName(typeof(NameStrategies), value);
                    timeScaleButtonText = $"TKUtils.Windows.Purge.TimeScales.{timeScaleText}".Localize();
                }

                timeScale = value;
            }
        }

        public override void Draw(Rect canvas)
        {
            (Rect labelRect, Rect fieldRect) = canvas.ToForm(0.7f);
            (Rect buttonRect, Rect inputRect) = fieldRect.ToForm(0.25f);
            var scaleButtonRect = new Rect(
                canvas.x + canvas.width - buttonRect.width,
                buttonRect.y,
                buttonRect.width,
                buttonRect.height
            );

            inputRect.width -= buttonRect.width - 5f;

            Widgets.Label(labelRect, labelText);
            DrawButton(buttonRect);
            Widgets.TextFieldNumeric(inputRect, ref value, ref buffer);

            if (!Widgets.ButtonText(scaleButtonRect, timeScaleButtonText))
            {
                return;
            }

            Find.WindowStack.Add(new FloatMenu(scaleOptions));
        }

        public override bool ShouldPurge(Viewer viewer)
        {
            int seconds = value * ScaleToInt(TimeScale);
            TimeSpan span = DateTime.Now - viewer.last_seen;

            switch (Comparison)
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

        private static int ScaleToInt(TimeScales scale)
        {
            switch (scale)
            {
                case TimeScales.Days:
                    return 60 * 24;

                case TimeScales.Hours:
                    return 60;

                case TimeScales.Months:
                    return 60 * 24 * 30;

                case TimeScales.Years:
                    return 60 * 24 * 30 * 12;

                default:
                    return 0;
            }
        }
    }
}

// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public enum TimeScales { Years, Months, Days, Hours }

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
            labelText = "TKUtils.PurgeMenu.Time".Localize().CapitalizeFirst();
            timeScaleButtonText = $"TKUtils.PurgeMenu.{timeScaleText}".Localize();

            scaleOptions = Enum.GetNames(typeof(TimeScales))
               .Select(
                    s => new FloatMenuOption(
                        $"TKUtils.PurgeMenu.{s}".Localize(),
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
                    timeScaleButtonText = $"TKUtils.PurgeMenu.{timeScaleText}".Localize();
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

            SettingsHelper.DrawLabel(labelRect, labelText);
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

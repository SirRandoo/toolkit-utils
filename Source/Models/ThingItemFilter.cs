using System;
using System.Collections.Generic;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum FilterTypes { Mod, Category, TechLevel, Stackable, Research }

    public class ThingItemFilter
    {
        private string label;
        private float labelWidth = -1;

        public FilterTypes FilterType { get; set; }

        public string Label
        {
            get => label;
            set
            {
                label = $" {value}";
                labelWidth = Text.CalcSize(label).x;
            }
        }

        public float LabelWidth
        {
            get
            {
                if (labelWidth <= -1)
                {
                    labelWidth = Text.CalcSize(Label).x;
                }

                return labelWidth;
            }
        }

        public Func<List<ThingItem>, List<ThingItem>> Filter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum FilterTypes { Mod, Category, TechLevel, Stackable, Research }

    public class ThingItemFilter
    {
        private bool active;
        private string label;
        private float labelWidth = -1;
        public ThingItemFilterCategory Category { get; set; }

        internal string Id { get; set; }

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

        public bool Active
        {
            get => active;
            set
            {
                active = value;
                Category?.MarkDirty();
            }
        }

        public Func<IEnumerable<ThingItem>, List<ThingItem>> Filter { get; set; }

        public static List<ThingItem> FilterByCategory(IEnumerable<ThingItem> subject, string category)
        {
            return subject.Where(t => t.Category.Equals(category)).ToList();
        }

        public static List<ThingItem> FilterByMod(IEnumerable<ThingItem> subject, string mod)
        {
            return subject.Where(t => t.Mod.Equals(mod)).ToList();
        }

        public static List<ThingItem> FilterByTechLevel(IEnumerable<ThingItem> subject, TechLevel techLevel)
        {
            return subject.Where(t => t.Thing.techLevel == techLevel).ToList();
        }

        public static List<ThingItem> FilterByStackable(IEnumerable<ThingItem> subject)
        {
            return subject.Where(t => t.Thing.stackLimit > 1).ToList();
        }

        public static List<ThingItem> FilterByNonStackable(IEnumerable<ThingItem> subject)
        {
            return subject.Where(t => t.Thing.stackLimit == 1).ToList();
        }

        public static List<ThingItem> FilterByResearched(IEnumerable<ThingItem> subject)
        {
            return Current.Game == null
                ? subject.ToList()
                : subject.Where(t => t.Thing.GetUnfinishedPrerequisites().NullOrEmpty()).ToList();
        }

        public static List<ThingItem> FilterByNotResearched(IEnumerable<ThingItem> subject)
        {
            return Current.Game == null
                ? subject.ToList()
                : subject.Where(t => !t.Thing.GetUnfinishedPrerequisites().NullOrEmpty()).ToList();
        }
    }
}

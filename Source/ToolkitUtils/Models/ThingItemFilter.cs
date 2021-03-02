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
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum FilterTypes
    {
        Mod,
        Category,
        TechLevel,
        Stackable,
        Research,
        State,
        Stuff
    }

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

        public Func<ItemTableItem, bool> IsUnfilteredFunc { get; set; }

        public static bool IsCategoryRelevant(ItemTableItem subject, string category)
        {
            return subject.Data.Category.Equals(category);
        }

        public static bool IsModRelevant(ItemTableItem subject, string mod)
        {
            return subject.Data.Mod.Equals(mod);
        }

        public static bool IsTechLevelRelevant(ItemTableItem subject, TechLevel techLevel)
        {
            return subject.Data.Thing != null && subject.Data.Thing.techLevel == techLevel;
        }

        public static bool FilterByStuff(ItemTableItem subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.IsStuff;
        }

        public static bool FilterByNotStuff(ItemTableItem subject)
        {
            return subject.Data.Thing != null && !subject.Data.Thing.IsStuff;
        }

        public static bool FilterByStackable(ItemTableItem subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.stackLimit > 1;
        }

        public static bool FilterByNonStackable(ItemTableItem subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.stackLimit == 1;
        }

        public static bool FilterByResearched(ItemTableItem subject)
        {
            return Current.Game != null
                   && subject.Data.Thing != null
                   && subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();
        }

        public static bool FilterByNotResearched(ItemTableItem subject)
        {
            return Current.Game != null
                   && subject.Data.Thing != null
                   && !subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();
        }

        public static bool FilterByEnabled(ItemTableItem subject)
        {
            return subject.Data.Price > 0;
        }

        public static bool FilterByDisabled(ItemTableItem subject)
        {
            return subject.Data.Price <= 0;
        }
    }
}

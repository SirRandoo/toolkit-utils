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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
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
        Stuff,
        Manufacturable
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

        public Func<TableSettingsItem<ThingItem>, bool> IsUnfilteredFunc { get; set; }

        public static bool IsCategoryRelevant([NotNull] TableSettingsItem<ThingItem> subject, string category)
        {
            return subject.Data.Category.Equals(category);
        }

        public static bool IsModRelevant([NotNull] TableSettingsItem<ThingItem> subject, string mod)
        {
            return subject.Data.Mod.Equals(mod);
        }

        public static bool IsTechLevelRelevant([NotNull] TableSettingsItem<ThingItem> subject, TechLevel techLevel)
        {
            return subject.Data.Thing != null && subject.Data.Thing.techLevel == techLevel;
        }

        public static bool FilterByStuff([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.IsStuff;
        }

        public static bool FilterByNotStuff([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Thing != null && !subject.Data.Thing.IsStuff;
        }

        public static bool FilterByStackable([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.stackLimit > 1;
        }

        public static bool FilterByNonStackable([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Thing != null && subject.Data.Thing.stackLimit == 1;
        }

        public static bool FilterByResearched(TableSettingsItem<ThingItem> subject)
        {
            return Current.Game != null
                   && subject.Data.Thing != null
                   && subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();
        }

        public static bool FilterByNotResearched(TableSettingsItem<ThingItem> subject)
        {
            return Current.Game != null
                   && subject.Data.Thing != null
                   && !subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();
        }

        public static bool FilterByEnabled([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Cost > 0;
        }

        public static bool FilterByDisabled([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.Cost <= 0;
        }

        public static bool FilterByManufactured([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.ProducedAt != null;
        }

        public static bool FilterByNonManufactured([NotNull] TableSettingsItem<ThingItem> subject)
        {
            return subject.Data.ProducedAt == null;
        }
    }
}

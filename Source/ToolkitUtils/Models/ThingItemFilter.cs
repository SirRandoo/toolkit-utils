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
using SirRandoo.ToolkitUtils.Models.Tables;
using Verse;

namespace SirRandoo.ToolkitUtils.Models;

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
    private bool _active;
    private string _label;
    private float _labelWidth = -1;
    public ThingItemFilterCategory Category { get; set; }

    internal string Id { get; set; }

    public string Label
    {
        get => _label;
        set
        {
            _label = $" {value}";
            _labelWidth = Text.CalcSize(_label).x;
        }
    }

    public float LabelWidth
    {
        get
        {
            if (_labelWidth <= -1)
            {
                _labelWidth = Text.CalcSize(Label).x;
            }

            return _labelWidth;
        }
    }

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            Category?.MarkDirty();
        }
    }

    public Func<TableSettingsItem<ThingItem>, bool> IsUnfilteredFunc { get; set; }

    public static bool IsCategoryRelevant(TableSettingsItem<ThingItem> subject, string category) => subject.Data.Category.Equals(category);

    public static bool IsModRelevant(TableSettingsItem<ThingItem> subject, string mod) => subject.Data.Mod.Equals(mod);

    public static bool IsTechLevelRelevant(TableSettingsItem<ThingItem> subject, TechLevel techLevel) =>
        subject.Data.Thing != null && subject.Data.Thing.techLevel == techLevel;

    public static bool FilterByStuff(TableSettingsItem<ThingItem> subject) => subject.Data.Thing != null && subject.Data.Thing.IsStuff;

    public static bool FilterByNotStuff(TableSettingsItem<ThingItem> subject) => subject.Data.Thing != null && !subject.Data.Thing.IsStuff;

    public static bool FilterByStackable(TableSettingsItem<ThingItem> subject) => subject.Data.Thing != null && subject.Data.Thing.stackLimit > 1;

    public static bool FilterByNonStackable(TableSettingsItem<ThingItem> subject) => subject.Data.Thing != null && subject.Data.Thing.stackLimit == 1;

    public static bool FilterByResearched(TableSettingsItem<ThingItem> subject) => Current.Game != null
        && subject.Data.Thing != null && subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();

    public static bool FilterByNotResearched(TableSettingsItem<ThingItem> subject) => Current.Game != null
        && subject.Data.Thing != null && !subject.Data.Thing.GetUnfinishedPrerequisites().NullOrEmpty();

    public static bool FilterByEnabled(TableSettingsItem<ThingItem> subject) => subject.Data.Cost > 0;

    public static bool FilterByDisabled(TableSettingsItem<ThingItem> subject) => subject.Data.Cost <= 0;

    public static bool FilterByManufactured(TableSettingsItem<ThingItem> subject) => subject.Data.ProducedAt != null;

    public static bool FilterByNonManufactured(TableSettingsItem<ThingItem> subject) => subject.Data.ProducedAt == null;
}
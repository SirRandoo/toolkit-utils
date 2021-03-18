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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItemFilterCategory
    {
        private MultiCheckboxState checkboxState = MultiCheckboxState.Off;
        public bool Expanded;

        public FilterTypes FilterType;

        public MultiCheckboxState CheckState
        {
            get => checkboxState;
            set
            {
                if (checkboxState == MultiCheckboxState.Partial && value != MultiCheckboxState.Partial)
                {
                    foreach (ThingItemFilter f in Filters)
                    {
                        f.Active = false;
                    }

                    checkboxState = MultiCheckboxState.Off;
                    return;
                }

                foreach (ThingItemFilter filter in Filters)
                {
                    switch (value)
                    {
                        case MultiCheckboxState.Off:
                            filter.Active = false;
                            break;
                        case MultiCheckboxState.On:
                            filter.Active = true;
                            break;
                    }
                }
            }
        }

        public List<ThingItemFilter> Filters { get; set; } = new List<ThingItemFilter>();
        [NotNull] public IEnumerable<ThingItemFilter> ActiveFilters => Filters.Where(i => i.Active);

        public float Height => (Expanded ? Filters.Count + 1 : 1) * Text.LineHeight;

        public void MarkDirty()
        {
            int totalFilters = Filters.Count;
            int activeFilters = Filters.Count(f => f.Active);

            if (activeFilters > 0 && activeFilters < totalFilters)
            {
                checkboxState = MultiCheckboxState.Partial;
            }
            else if (activeFilters <= 0)
            {
                checkboxState = MultiCheckboxState.Off;
            }
            else if (activeFilters >= totalFilters)
            {
                checkboxState = MultiCheckboxState.On;
            }
        }

        public bool IsFiltered(TableSettingsItem<ThingItem> item)
        {
            return ActiveFilters.Any() && ActiveFilters.All(i => !i.IsUnfilteredFunc(item));
        }
    }
}

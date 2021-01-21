using System.Collections.Generic;
using System.Linq;
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

        public float Height => (Expanded ? Filters.Count : 1) * Text.LineHeight;

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
    }
}

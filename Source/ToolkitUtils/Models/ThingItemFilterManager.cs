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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItemFilterManager
    {
        private readonly List<ThingItemFilterCategory> filters;
        private Vector2 scrollPos = Vector2.zero;

        public ThingItemFilterManager()
        {
            filters = new List<ThingItemFilterCategory>();
        }

        public List<FilterTypes> UniqueFilters { get; set; }

        public IEnumerable<ThingItemFilter> FiltersForType(FilterTypes type)
        {
            ThingItemFilterCategory result = filters.FirstOrDefault(f => f.FilterType == type);

            if (result == null)
            {
                yield break;
            }

            foreach (ThingItemFilter filter in result.Filters)
            {
                yield return filter;
            }
        }

        public IEnumerable<ThingItem> FilterItems(IEnumerable<ThingItem> input)
        {
            List<ThingItem> result = filters.Aggregate(
                    input,
                    (current, category) => FilterItemsByType(category.FilterType, current)
                )
               .ToList();

            return result.Distinct();
        }

        public IEnumerable<ThingItem> FilterItemsByType(FilterTypes type, IEnumerable<ThingItem> input)
        {
            ThingItemFilterCategory result = filters.FirstOrDefault(f => f.FilterType == type);

            if (result == null)
            {
                foreach (ThingItem item in input)
                {
                    yield return item;
                }

                yield break;
            }

            var container = new List<ThingItem>();
            List<ThingItem> workingList = input.ToList();

            foreach (ThingItemFilter filter in result.Filters.Where(f => f.Active))
            {
                container.AddRange(filter.Filter(workingList));
            }

            if (container.Count <= 0)
            {
                container = workingList;
            }

            foreach (ThingItem thingItem in container.Distinct())
            {
                yield return thingItem;
            }
        }

        public void RegisterFilter(FilterTypes type, ThingItemFilter filter)
        {
            ThingItemFilterCategory result = filters.FirstOrDefault(f => f.FilterType == type);

            if (result == null)
            {
                var category = new ThingItemFilterCategory {FilterType = type};
                category.Filters.Add(filter);
                filter.Category = category;

                filters.Add(category);
                return;
            }

            if (result.Filters.Count > 0 && UniqueFilters.Contains(type))
            {
                foreach (ThingItemFilter itemFilter in result.Filters)
                {
                    itemFilter.Active = false;
                }
            }

            ThingItemFilter storedFilter = result.Filters.FirstOrDefault(f => f.Id.Equals(filter.Id));

            if (storedFilter != null)
            {
                storedFilter.Active = true;
                return;
            }

            filter.Category = result;
            result.Filters.Add(filter);
        }

        public void UnregisterFilter(FilterTypes type, string filterId)
        {
            ThingItemFilterCategory result = filters.FirstOrDefault(f => f.FilterType == type);
            ThingItemFilter filter = result?.Filters.FirstOrDefault(f => f.Id.Equals(filterId));

            if (filter == null)
            {
                return;
            }

            filter.Active = false;
        }

        [SuppressMessage("ReSharper", "CognitiveComplexity")]
        public void DrawFilters(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var listing = new Listing_Standard();
            var viewPort = new Rect(canvas.x, canvas.y, canvas.width - 16f, filters.Sum(f => f.Height));

            listing.BeginScrollView(canvas, ref scrollPos, ref viewPort);

            foreach (ThingItemFilterCategory category in filters)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);

                if (!lineRect.IsRegionVisible(viewPort, scrollPos) && !category.Expanded)
                {
                    continue;
                }

                Rect arrowIconRect =
                    new Rect(lineRect.x, lineRect.y, lineRect.height, lineRect.height).ContractedBy(4f);
                Rect checkStateRect = new Rect(
                    arrowIconRect.x + arrowIconRect.width,
                    lineRect.y,
                    lineRect.height,
                    lineRect.height
                ).ContractedBy(4f);
                var categoryTextRect = new Rect(
                    checkStateRect.x + checkStateRect.width + 5f,
                    lineRect.y,
                    lineRect.width - 5f - arrowIconRect.width - checkStateRect.width,
                    lineRect.height
                );

                GUI.DrawTexture(
                    arrowIconRect.ContractedBy(2f),
                    category.Expanded ? Textures.ExpandedArrow : Textures.CollapsedArrow
                );

                MultiCheckboxState state = Widgets.CheckboxMulti(checkStateRect, category.CheckState, true);

                if (state != category.CheckState)
                {
                    category.CheckState = state;

                    foreach (ThingItemFilter filter in category.Filters)
                    {
                        switch (category.CheckState)
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

                Widgets.Label(categoryTextRect, $"TKUtils.FilterTypes.{category.FilterType}".Localize());

                if (Widgets.ButtonInvisible(arrowIconRect))
                {
                    category.Expanded = !category.Expanded;
                }

                if (category.Expanded)
                {
                    DrawFiltersFor(category, listing, viewPort);
                }
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
        }

        private void DrawFiltersFor(ThingItemFilterCategory category, Listing listing, Rect viewPort)
        {
            foreach (ThingItemFilter filter in category.Filters)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);

                if (!lineRect.IsRegionVisible(viewPort, scrollPos))
                {
                    continue;
                }

                var canvas = new Rect(lineRect.x + 16f, lineRect.y, lineRect.width - 16f, lineRect.height);
                var checkRect = new Rect(canvas.x + 4f, canvas.y + 4f, 12f, 12f);
                var textRect = new Rect(checkRect.x + 12f, canvas.y, canvas.width - 12f, canvas.height);

                GUI.DrawTexture(checkRect, filter.Active ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);

                if (textRect.width < filter.LabelWidth)
                {
                    SettingsHelper.DrawLabel(textRect, filter.Label, fontScale: GameFont.Tiny);
                }
                else
                {
                    Widgets.Label(textRect, filter.Label);
                }

                if (Widgets.ButtonInvisible(canvas))
                {
                    filter.Active = !filter.Active;
                }

                Widgets.DrawHighlightIfMouseover(canvas);
            }
        }
    }
}

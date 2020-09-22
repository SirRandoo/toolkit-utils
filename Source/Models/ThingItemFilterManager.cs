using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ThingItemFilterManager
    {
        private static readonly List<FilterTypes> KnownFilters = Enum.GetNames(typeof(FilterTypes))
           .Select(f => (FilterTypes) Enum.Parse(typeof(FilterTypes), f))
           .ToList();

        private readonly List<FilterTypes> expanded = new List<FilterTypes>();
        private readonly Dictionary<FilterTypes, List<ThingItemFilter>> filters;
        private Vector2 scrollPos = Vector2.zero;

        public ThingItemFilterManager()
        {
            filters = new Dictionary<FilterTypes, List<ThingItemFilter>>();
        }

        public List<FilterTypes> UniqueFilters { get; set; }

        public IEnumerable<ThingItemFilter> FiltersForType(FilterTypes type)
        {
            if (!filters.TryGetValue(type, out List<ThingItemFilter> activeFilters))
            {
                yield break;
            }

            foreach (ThingItemFilter filter in activeFilters)
            {
                yield return filter;
            }
        }

        public IEnumerable<ThingItem> FilterItems(IEnumerable<ThingItem> input)
        {
            List<ThingItem> result = KnownFilters.Aggregate(
                    input,
                    (current, category) => FilterItemsByType(category, current)
                )
               .ToList();

            return result.Distinct();
        }

        public IEnumerable<ThingItem> FilterItemsByType(FilterTypes type, IEnumerable<ThingItem> input)
        {
            if (!filters.TryGetValue(type, out List<ThingItemFilter> itemFilters))
            {
                foreach (ThingItem thingItem in input)
                {
                    yield return thingItem;
                }

                yield break;
            }

            var container = new List<ThingItem>();
            List<ThingItem> workingList = input.ToList();

            foreach (ThingItemFilter filter in itemFilters.Where(f => f.Active))
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
            if (!filters.TryGetValue(type, out List<ThingItemFilter> itemFilters))
            {
                filters[type] = new List<ThingItemFilter> {filter};
                return;
            }

            if (itemFilters.Count > 0 && UniqueFilters.Contains(type))
            {
                foreach (ThingItemFilter itemFilter in itemFilters)
                {
                    itemFilter.Active = false;
                }
            }

            ThingItemFilter storedFilter = itemFilters.FirstOrDefault(f => f.Id.Equals(filter.Id));

            if (storedFilter != null)
            {
                storedFilter.Active = true;
                return;
            }

            itemFilters.Add(filter);
        }

        public void UnregisterFilter(FilterTypes type, string filterId)
        {
            if (!filters.TryGetValue(type, out List<ThingItemFilter> itemFilters))
            {
                return;
            }

            ThingItemFilter filter = itemFilters.FirstOrDefault(f => f.Id.Equals(filterId));

            if (filter == null)
            {
                return;
            }

            filter.Active = false;
        }

        public void DrawFilters(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var listing = new Listing_Standard();
            var viewPort = new Rect(canvas.x, canvas.y, canvas.width - 16f, Text.LineHeight * KnownFilters.Count);

            foreach (FilterTypes type in expanded)
            {
                if (!filters.TryGetValue(type, out List<ThingItemFilter> itemFilters))
                {
                    continue;
                }

                viewPort.height += Text.LineHeight * itemFilters.Count;
            }

            listing.BeginScrollView(canvas, ref scrollPos, ref viewPort);

            foreach (FilterTypes filterType in KnownFilters)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);

                if (!lineRect.IsRegionVisible(viewPort, scrollPos) && !expanded.Contains(filterType))
                {
                    continue;
                }

                bool isExpanded = expanded.Contains(filterType);
                Widgets.Label(lineRect, (isExpanded ? " - " : " + ") + $"TKUtils.FilterTypes.{filterType}".Localize());

                if (Widgets.ButtonInvisible(lineRect))
                {
                    if (isExpanded)
                    {
                        expanded.Remove(filterType);
                    }
                    else
                    {
                        expanded.Add(filterType);
                    }
                }

                if (isExpanded)
                {
                    DrawFiltersFor(filterType, listing, viewPort);
                }

                Widgets.DrawHighlightIfMouseover(lineRect);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
        }

        private void DrawFiltersFor(FilterTypes type, Listing listing, Rect viewPort)
        {
            if (!filters.TryGetValue(type, out List<ThingItemFilter> itemFilters))
            {
                return;
            }

            foreach (ThingItemFilter filter in itemFilters)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);

                if (!lineRect.IsRegionVisible(viewPort, scrollPos))
                {
                    continue;
                }

                var canvas = new Rect(lineRect.x + 8f, lineRect.y, lineRect.width - 8f, lineRect.height);
                var checkRect = new Rect(canvas.x + 3f, canvas.y + 3f, 12f, 12f);
                var textRect = new Rect(checkRect.x + 16f, canvas.y, canvas.width - 16f, canvas.height);

                GUI.DrawTexture(checkRect, filter.Active ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);

                if (textRect.width < filter.LabelWidth)
                {
                    SettingsHelper.DrawSmallLabelAnchored(textRect, filter.Label, TextAnchor.MiddleLeft);
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

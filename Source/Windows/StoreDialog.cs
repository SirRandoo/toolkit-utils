using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public enum SortMode { Ascending, Descending }

    public enum Sorter { Name, Price, Category }

    [StaticConstructorOnStartup]
    public class StoreDialog : Window
    {
        private static readonly Texture2D _sortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
        private static readonly Texture2D _sortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        private readonly List<Item> cache = StoreInventory.items;
        private readonly List<Change> dirtyItems = new List<Change>();
        private readonly List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading;

        private string currentQuery = "";
        private string lastQuery = "";

        private List<Item> results;
        private Vector2 scrollPos = Vector2.zero;
        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Descending;

        public StoreDialog()
        {
            doCloseX = true;
            forcePause = true;

            optionalTitle = "TKUtils.Windows.Store.Title".Translate();
            cache?.SortBy(i => i.abr);

            if (cache == null)
            {
                TkLogger.Warn("Toolkit's item shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight / 9f);

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard {maxOneColumn = true};
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f, Text.LineHeight);
            var clearRect = new Rect(searchRect.x + searchRect.width + 5f, searchRect.y, 16f, searchRect.height);

            currentQuery = Widgets.TextEntryLabeled(
                searchRect,
                "TKUtils.Windows.Config.Buttons.Search.Label".Translate(),
                currentQuery
            );

            var old = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(clearRect))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                results = null;
                lastQuery = "";
            }

            Text.Anchor = old;
            var resetText = "TKUtils.Windows.Config.Buttons.ResetAll.Label".Translate();
            var enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            var disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            var resetSize = Text.CalcSize(resetText);
            var enableSize = Text.CalcSize(enableText);
            var disableSize = Text.CalcSize(disableText);
            var disableRect = new Rect(
                inRect.width - disableSize.x * 1.5f,
                inRect.y,
                disableSize.x * 1.5f,
                Text.LineHeight
            );
            var enableRect = new Rect(
                inRect.width - disableSize.x * 1.5f - enableSize.x * 1.5f,
                inRect.y,
                enableSize.x * 1.5f,
                Text.LineHeight
            );
            var resetRect = new Rect(
                inRect.width - disableSize.x * 1.5f - enableSize.x * 1.5f - resetSize.x * 1.5f,
                inRect.y,
                resetSize.x * 1.5f,
                Text.LineHeight
            );

            if (Widgets.ButtonText(resetRect, resetText))
            {
                foreach (var item in cache.Where(i => i.price < 0))
                {
                    var thing = things.FirstOrDefault(t => t.defName.Equals(item.defname));
                    var change = dirtyItems.FirstOrDefault(i => i.DefName.Equals(item.defname));
                    var price = CalculateToolkitPrice(thing?.BaseMarketValue ?? 0f);

                    if (change != null && change.Price != price)
                    {
                        change.Price = price;
                    }
                    else if (item.price != price)
                    {
                        dirtyItems.Add(new Change(item.defname) {Price = price});
                    }
                }
            }

            if (Widgets.ButtonText(enableRect, enableText))
            {
                foreach (var item in cache.Where(i => i.price < 0))
                {
                    var thing = things.FirstOrDefault(t => t.defName.Equals(item.defname));
                    var change = dirtyItems.FirstOrDefault(i => i.DefName.Equals(item.defname));
                    var price = thing?.BaseMarketValue ?? 0f;

                    if (change != null)
                    {
                        change.Price = price <= 0 ? -10 : CalculateToolkitPrice(price);
                    }
                    else
                    {
                        dirtyItems.Add(
                            new Change(item.defname) {Price = price <= 0 ? -10 : CalculateToolkitPrice(price)}
                        );
                    }
                }
            }

            if (Widgets.ButtonText(disableRect, disableText))
            {
                foreach (var item in cache.Where(i => i.price > -10))
                {
                    var change = dirtyItems.FirstOrDefault(i => i.DefName.Equals(item.defname));

                    if (change != null)
                    {
                        change.Price = -10;
                    }
                    else
                    {
                        dirtyItems.Add(new Change(item.defname) {Price = -10});
                    }
                }
            }

            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 3f
            );

            var total = results?.Count ?? cache.Count;
            var viewPort = new Rect(contentArea.x, 0f, contentArea.width * 0.9f, Text.LineHeight * total + 1);
            var wrapped = Text.WordWrap;

            Text.WordWrap = false;

            var infoHeaderRect = new Rect(
                inRect.x,
                Text.LineHeight * 3f,
                inRect.width * 0.3f,
                Text.LineHeight
            );
            var priceHeaderRect = new Rect(
                inRect.x + infoHeaderRect.width + 5f,
                infoHeaderRect.y,
                infoHeaderRect.width,
                Text.LineHeight
            );
            var categoryHeaderRect = new Rect(
                inRect.x + infoHeaderRect.width + priceHeaderRect.width + 5f,
                infoHeaderRect.y,
                infoHeaderRect.width,
                Text.LineHeight
            );

            Text.Anchor = TextAnchor.MiddleCenter;
            if (Widgets.ButtonText(infoHeaderRect, "TKUtils.Windows.Store.Headers.Name".Translate()))
            {
                sorter = Sorter.Name;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                switch (sortMode)
                {
                    case SortMode.Ascending:
                        (results ?? cache)?.SortBy(i => i.abr);
                        break;

                    case SortMode.Descending:
                        (results ?? cache)?.SortByDescending(i => i.abr);
                        break;
                }
            }

            if (Widgets.ButtonText(priceHeaderRect, "TKUtils.Windows.Store.Headers.Price".Translate()))
            {
                sorter = Sorter.Price;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                switch (sortMode)
                {
                    case SortMode.Ascending:
                        (results ?? cache)?.SortBy(i => i.price);
                        break;

                    case SortMode.Descending:
                        (results ?? cache)?.SortByDescending(i => i.price);
                        break;
                }
            }

            if (Widgets.ButtonText(categoryHeaderRect, "TKUtils.Windows.Store.Headers.Category".Translate()))
            {
                sorter = Sorter.Category;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                switch (sortMode)
                {
                    case SortMode.Ascending:
                        (results ?? cache)?.SortBy(
                            i =>
                            {
                                var thing = things.FirstOrDefault(t => t.defName.Equals(i.defname));

                                var category = thing?.FirstThingCategory?.LabelCap.RawText ?? string.Empty;

                                if (category.NullOrEmpty() && thing?.race != null)
                                {
                                    category = "Animal";
                                }

                                return category;
                            }
                        );
                        break;

                    case SortMode.Descending:
                        (results ?? cache)?.SortByDescending(
                            i =>
                            {
                                var thing = things.FirstOrDefault(t => t.defName.Equals(i.defname));

                                var category = thing?.FirstThingCategory?.LabelCap.RawText ?? string.Empty;

                                if (category.NullOrEmpty() && thing?.race != null)
                                {
                                    category = "Animal";
                                }

                                return category;
                            }
                        );
                        break;
                }
            }

            Text.Anchor = old;

            switch (sorter)
            {
                case Sorter.Name:
                    GUI.DrawTexture(
                        new Rect(infoHeaderRect.x, infoHeaderRect.y, 16f, 16f),
                        sortMode == SortMode.Descending ? _sortingDescend : _sortingAscend
                    );
                    break;

                case Sorter.Price:
                    GUI.DrawTexture(
                        new Rect(priceHeaderRect.x, priceHeaderRect.y, 16f, 16f),
                        sortMode == SortMode.Descending ? _sortingDescend : _sortingAscend
                    );
                    break;

                case Sorter.Category:
                    GUI.DrawTexture(
                        new Rect(categoryHeaderRect.x, categoryHeaderRect.y, 16f, 16f),
                        sortMode == SortMode.Descending ? _sortingDescend : _sortingAscend
                    );
                    break;
            }

            listing.BeginScrollView(contentArea, ref scrollPos, ref viewPort);
            foreach (var item in results ?? cache)
            {
                var thing = things.FirstOrDefault(t => t.defName.Equals(item.defname));
                var lineRect = listing.GetRect(Text.LineHeight);

                var iconRect = new Rect(contentArea.x, lineRect.y, 27f, lineRect.height);
                var labelRect = new Rect(
                    contentArea.x + iconRect.width + 5f,
                    lineRect.y,
                    lineRect.width * 0.25f,
                    lineRect.height
                );
                var infoRect = new Rect(
                    contentArea.x,
                    lineRect.y,
                    iconRect.width + labelRect.width + 5f,
                    lineRect.height
                );

                Widgets.Label(labelRect, thing?.LabelCap ?? item.abr);

                if (thing != null)
                {
                    Widgets.ThingIcon(iconRect, thing);

                    if (Widgets.ButtonInvisible(infoRect, false))
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(thing));
                    }

                    Widgets.DrawHighlightIfMouseover(infoRect);
                }
            }

            Text.WordWrap = wrapped;
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (lastQuery.Equals(currentQuery))
            {
                return;
            }

            if (Time.time % 2 < 1)
            {
                Notify__SearchRequested();
            }
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults();
        }

        private List<Item> GetSearchResults()
        {
            var serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            return cache
                .Where(i => i.abr.EqualsIgnoreCase(serialized) || i.defname.EqualsIgnoreCase(serialized))
                .ToList();
        }

        public override void PreClose()
        {
            foreach (var thing in things)
            {
                var change = dirtyItems.FirstOrDefault(i => i.DefName.Equals(thing.defName));

                Item item;

                if (change == null)
                {
                    item = new Item(
                        CalculateToolkitPrice(thing.BaseMarketValue),
                        thing.LabelCap.RawText.ToToolkit(),
                        thing.defName
                    );

                    StoreInventory.items.Add(item);
                }
                else
                {
                    item = StoreInventory.items.FirstOrDefault(i => i.defname.Equals(change.DefName));

                    if (item == null)
                    {
                        TkLogger.Warn(
                            $"Received change notice for item \"{change.DefName}\", but the item doesn't exist in the shop."
                        );
                        continue;
                    }

                    item.price = change.Price;
                }
            }

            Store_ItemEditor.UpdateStoreItemList();
        }

        public override void PreOpen()
        {
            Store_ItemEditor.FindItemsNotInList();
        }

        public static int CalculateToolkitPrice(float basePrice)
        {
            return Math.Max(1, Convert.ToInt32(basePrice * 10.0f / 6.0f));
        }

        private class Change
        {
            public Change(string defName)
            {
                DefName = defName;
            }

            public string DefName { get; }
            public int Price { get; set; }
        }
    }
}

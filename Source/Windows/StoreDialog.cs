using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly Texture2D SortingAscend;
        private static readonly Texture2D SortingDescend;
        private readonly List<Container> cache;
        private string categoryFilter = "";
        private TaggedString categoryHeader;
        private bool closeCalled;
        private bool ctrlKeyDown;
        private TaggedString ctxAscending;
        private TaggedString ctxDescending;
        private TaggedString ctxInfo;

        private string currentQuery = "";
        private TaggedString disableAllText;
        private Vector2 disableAllTextSize;
        private TaggedString enableAllText;
        private Vector2 enableAllTextSize;
        private string lastQuery = "";
        private TaggedString nameHeader;
        private TaggedString priceHeader;
        private TaggedString resetAllText;

        private Vector2 resetAllTextSize;

        private List<Container> results;
        private Vector2 scrollPos = Vector2.zero;
        private TaggedString searchText;

        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private bool shftKeyDown;

        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;

        private TaggedString title;

        static StoreDialog()
        {
            SortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
            SortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        }

        public StoreDialog()
        {
            doCloseX = true;

            GetTranslationStrings();
            cache = GenerateContainers();

            optionalTitle = title;
            cache?.SortBy(i => i.Item.abr);

            if (cache == null)
            {
                TkLogger.Warn("Toolkit's item shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(inRect);
            var listing = new Listing_Standard {maxOneColumn = true};
            var fontCache = Text.Font;
            Text.Font = GameFont.Small;

            DrawStoreHeader(inRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            var wrapped = Text.WordWrap;

            Text.WordWrap = false;

            var infoHeaderRect = new Rect(
                30f,
                0f,
                inRect.width * 0.4f - 15f,
                Text.LineHeight
            );
            var priceHeaderRect = new Rect(
                infoHeaderRect.width + 35f,
                infoHeaderRect.y,
                inRect.width - infoHeaderRect.width * 2f - 35f,
                Text.LineHeight
            );
            var categoryHeaderRect = new Rect(
                infoHeaderRect.width + priceHeaderRect.width + 35f,
                infoHeaderRect.y,
                infoHeaderRect.width,
                Text.LineHeight
            );


            GUI.BeginGroup(new Rect(inRect.x, Text.LineHeight * 3f, inRect.width, infoHeaderRect.height));
            Widgets.DrawHighlightIfMouseover(infoHeaderRect);
            Widgets.DrawHighlightIfMouseover(priceHeaderRect);
            Widgets.DrawHighlightIfMouseover(categoryHeaderRect);


            if (Widgets.ButtonText(infoHeaderRect, "   " + nameHeader, false))
            {
                if (sorter != Sorter.Name)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Name;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            if (Widgets.ButtonText(priceHeaderRect, "   " + priceHeader, false))
            {
                if (sorter != Sorter.Price)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Price;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            if (Widgets.ButtonText(categoryHeaderRect, "   " + categoryHeader, false))
            {
                if (sorter != Sorter.Category)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Category;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            switch (sorter)
            {
                case Sorter.Name:
                    GUI.DrawTexture(
                        new Rect(infoHeaderRect.x, infoHeaderRect.y + (Text.LineHeight / 2f) - 4f, 8f, 8f),
                        sortMode != SortMode.Descending ? SortingAscend : SortingDescend
                    );
                    break;

                case Sorter.Price:
                    GUI.DrawTexture(
                        new Rect(priceHeaderRect.x, priceHeaderRect.y + (Text.LineHeight / 2f) - 4f, 8f, 8f),
                        sortMode != SortMode.Descending ? SortingAscend : SortingDescend
                    );
                    break;

                case Sorter.Category:
                    GUI.DrawTexture(
                        new Rect(categoryHeaderRect.x, categoryHeaderRect.y + (Text.LineHeight / 2f) - 4f, 8f, 8f),
                        sortMode != SortMode.Descending ? SortingAscend : SortingDescend
                    );
                    break;
            }

            GUI.EndGroup();

            var effectiveWorkingList = results ?? cache;
            var total = effectiveWorkingList.Count;
            const float scale = 1.25f;

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 4f
            );
            var viewPort = new Rect(
                0f,
                0f,
                contentArea.width - 16f,
                (Text.LineHeight * scale) * total
            );
            var items = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(items, ref scrollPos, ref viewPort);
            for (var index = 0; index < effectiveWorkingList.Count; index++)
            {
                var anchor = Text.Anchor;
                var item = effectiveWorkingList[index];
                var lineRect = listing.GetRect(Text.LineHeight * scale);

                if (!lineRect.IsRegionVisible(items, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                var iconRect = new Rect(27f, lineRect.y, 27f, lineRect.height);
                var labelRect = new Rect(
                    iconRect.width + 5f + 27f,
                    lineRect.y,
                    infoHeaderRect.width - 30f,
                    lineRect.height
                );
                var infoRect = new Rect(
                    27f,
                    lineRect.y,
                    infoHeaderRect.width,
                    lineRect.height
                );

                Widgets.Checkbox(0f, lineRect.y, ref item.Enabled, paintable: true);

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, item.Thing?.LabelCap ?? item.Item.abr);
                Text.Anchor = anchor;

                if (item.Thing != null)
                {
                    Widgets.ThingIcon(iconRect, item.Thing);
                }

                if (Widgets.ButtonInvisible(infoRect, false))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(item.Thing));
                }

                Widgets.DrawHighlightIfMouseover(infoRect);

                if (infoRect.WasRightClicked())
                {
                    var infoOptions = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption(
                            ctxInfo,
                            () => Find.WindowStack.Add(new Dialog_InfoCard(item.Thing))
                        ),
                        new FloatMenuOption(
                            (item.Enabled
                                ? "TKUtils.Windows.Store.Context.Disable"
                                : "TKUtils.Windows.Store.Context.Enable"
                            ).Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            ctxAscending,
                            () =>
                            {
                                sorter = Sorter.Name;
                                sortMode = SortMode.Ascending;
                                SortCurrentWorkingList();
                            }
                        ),
                        new FloatMenuOption(
                            ctxDescending,
                            () =>
                            {
                                sorter = Sorter.Name;
                                sortMode = SortMode.Descending;
                                SortCurrentWorkingList();
                            }
                        )
                    };

                    Find.WindowStack.Add(new FloatMenu(infoOptions));
                }


                var priceRect = new Rect(
                    infoRect.x + infoRect.width + 5f,
                    lineRect.y,
                    priceHeaderRect.width,
                    lineRect.height
                );

                if (item.Item.price > 0)
                {
                    SettingsHelper.DrawPriceField(priceRect, ref item.Item.price, ref ctrlKeyDown, ref shftKeyDown);
                }

                if (priceRect.WasRightClicked())
                {
                    var priceOptions = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption(
                            (item.Enabled
                                ? "TKUtils.Windows.Store.Context.Disable"
                                : "TKUtils.Windows.Store.Context.Enable"
                            ).Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            ctxAscending,
                            () =>
                            {
                                sorter = Sorter.Price;
                                sortMode = SortMode.Ascending;
                                SortCurrentWorkingList();
                            }
                        ),
                        new FloatMenuOption(
                            ctxDescending,
                            () =>
                            {
                                sorter = Sorter.Price;
                                sortMode = SortMode.Descending;
                                SortCurrentWorkingList();
                            }
                        )
                    };

                    Find.WindowStack.Add(new FloatMenu(priceOptions));
                }

                var categoryRect = new Rect(
                    priceRect.x + priceRect.width + 5f,
                    lineRect.y,
                    categoryHeaderRect.width,
                    lineRect.height
                );

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(categoryRect, item.Category);
                Text.Anchor = anchor;

                if (categoryRect.WasRightClicked())
                {
                    var categoryOptions = new List<FloatMenuOption>()
                    {
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Category".Translate(item.Category),
                            () =>
                            {
                                categoryFilter = item.Category;
                                Notify__SearchRequested();
                            }
                        ),
                        new FloatMenuOption(
                            (item.Enabled
                                ? "TKUtils.Windows.Store.Context.Disable"
                                : "TKUtils.Windows.Store.Context.Enable"
                            ).Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.EnableAll".Translate(item.Category),
                            () =>
                            {
                                foreach (var i in cache.Where(
                                    i => i.Category.RawText.EqualsIgnoreCase(item.Category.RawText)
                                ))
                                {
                                    i.Enabled = true;
                                    i.Update();
                                }
                            }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.DisableAll".Translate(item.Category),
                            () =>
                            {
                                foreach (var i in cache.Where(
                                    i => i.Category.RawText.EqualsIgnoreCase(item.Category.RawText)
                                ))
                                {
                                    i.Enabled = false;
                                    i.Update();
                                }
                            }
                        ),
                        new FloatMenuOption(
                            ctxAscending,
                            () =>
                            {
                                sorter = Sorter.Category;
                                sortMode = SortMode.Ascending;
                                SortCurrentWorkingList();
                            }
                        ),
                        new FloatMenuOption(
                            ctxDescending,
                            () =>
                            {
                                sorter = Sorter.Category;
                                sortMode = SortMode.Descending;
                                SortCurrentWorkingList();
                            }
                        )
                    };

                    Find.WindowStack.Add(new FloatMenu(categoryOptions));
                }

                if (!closeCalled)
                {
                    item.Update();
                }
            }

            GUI.EndGroup();

            listing.EndScrollView(ref viewPort);
            Text.WordWrap = wrapped;
            Text.Font = fontCache;

            GUI.EndGroup();
        }

        private void DrawStoreHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var line = new Rect(canvas.x, canvas.y, canvas.width, Text.LineHeight);
            var searchRect = new Rect(line.x, line.y, line.width * 0.25f, line.height);
            var workingList = results ?? cache;

            currentQuery = Widgets.TextEntryLabeled(searchRect, searchText, currentQuery);

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(searchRect))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                if (categoryFilter.NullOrEmpty())
                {
                    results = null;
                }
                else if (!lastQuery.Equals(currentQuery))
                {
                    Notify__SearchRequested();
                }

                lastQuery = "";
            }

            var buttonWidth = Mathf.Max(resetAllTextSize.x, enableAllTextSize.x, disableAllTextSize.x) * 1.5f;
            var offset = buttonWidth;

            if (Widgets.ButtonText(
                new Rect(line.x + line.width - offset, line.y, buttonWidth, line.height),
                disableAllText
            ))
            {
                foreach (var item in workingList.Where(i => i.Item.price > 0))
                {
                    item.Enabled = false;
                    item.Update();
                }
            }

            offset += buttonWidth + 5f;

            if (Widgets.ButtonText(
                new Rect(line.x + line.width - offset, line.y, buttonWidth, line.height),
                enableAllText
            ))
            {
                foreach (var item in workingList.Where(i => i.Item.price < 0))
                {
                    item.Enabled = true;
                    item.Update();
                }
            }

            offset += buttonWidth + 5f;

            if (Widgets.ButtonText(
                new Rect(line.x + line.width - offset, line.y, buttonWidth, line.height),
                resetAllText
            ))
            {
                foreach (var item in workingList)
                {
                    item.Item.price = CalculateToolkitPrice(item.Thing.BaseMarketValue);
                }
            }

            offset += buttonWidth + 5f;

            var filterSection = new Rect(
                searchRect.x + searchRect.width + 5f,
                line.y,
                line.width - offset - searchRect.width - 10f,
                line.height
            );

            var filterOffset = 0f;

            if (!categoryFilter.NullOrEmpty())
            {
                var categoryFilterWidth = Text.CalcSize(categoryFilter).x * 1.5f;
                var categoryFilterRect = new Rect(
                    filterSection.x + filterOffset,
                    filterSection.y,
                    categoryFilterWidth + 20f,
                    filterSection.height
                );

                Widgets.DrawHighlight(categoryFilterRect);
                Widgets.Label(categoryFilterRect, " " + categoryFilter);

                if (SettingsHelper.DrawClearButton(categoryFilterRect))
                {
                    categoryFilter = null;

                    if (!currentQuery.NullOrEmpty())
                    {
                        Notify__SearchRequested();
                    }
                }
            }

            GUI.EndGroup();
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
            SortCurrentWorkingList();
        }

        private List<Container> GetSearchResults()
        {
            var workingList = cache;

            if (!categoryFilter.NullOrEmpty())
            {
                workingList = workingList
                    .Where(i => i.Category.RawText.EqualsIgnoreCase(categoryFilter))
                    .ToList();
            }

            var serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return workingList;
            }

            return workingList
                .Where(
                    i =>
                    {
                        if (i.Item.abr.ToToolkit().Contains(serialized)
                            || i.Item.abr.ToToolkit().EqualsIgnoreCase(serialized))
                        {
                            return true;
                        }

                        return i.Item.defname.ToToolkit().Contains(serialized)
                               || i.Item.defname.ToToolkit().EqualsIgnoreCase(serialized);
                    }
                )
                .ToList();
        }

        public override void PreClose()
        {
            foreach (var c in cache.Where(c => c.Item == null))
            {
                c.Item = new Item(
                    CalculateToolkitPrice(c.Thing.BaseMarketValue),
                    c.Thing.LabelCap.RawText.ToToolkit(),
                    c.Thing.defName
                );

                StoreInventory.items.Add(c.Item);
            }

            base.PreClose();
        }

        public override void PostClose()
        {
            closeCalled = true;
            Store_ItemEditor.UpdateStoreItemList();

            base.PostClose();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            Store_ItemEditor.FindItemsNotInList();
        }

        public static int CalculateToolkitPrice(float basePrice)
        {
            return Math.Max(1, Convert.ToInt32(basePrice * 10.0f / 6.0f));
        }

        public static IEnumerable<ThingDef> GetTradeables()
        {
            return Store_ItemEditor.GetDefaultItems();
        }

        private void SortCurrentWorkingList()
        {
            var workingList = results ?? cache;

            switch (sorter)
            {
                case Sorter.Name:
                    switch (sortMode)
                    {
                        case SortMode.Ascending:
                            workingList.SortBy(i => i.Item.abr);
                            results = workingList;
                            return;
                        case SortMode.Descending:
                            workingList.SortByDescending(i => i.Item.abr);
                            results = workingList;
                            return;
                        default:
                            return;
                    }
                case Sorter.Price:
                    switch (sortMode)
                    {
                        case SortMode.Ascending:
                            workingList.SortBy(i => i.Item.price);
                            results = workingList;
                            return;
                        case SortMode.Descending:
                            workingList.SortByDescending(i => i.Item.price);
                            results = workingList;
                            return;
                        default:
                            return;
                    }
                case Sorter.Category when sortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Category.RawText);
                    results = workingList;
                    return;
                case Sorter.Category when sortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Category.RawText);
                    results = workingList;
                    return;
                default:
                    return;
            }
        }

        private void GetTranslationStrings()
        {
            title = "TKUtils.Windows.Store.Title".Translate();
            nameHeader = "TKUtils.Windows.Store.Headers.Name".Translate();
            priceHeader = "TKUtils.Windows.Store.Headers.Price".Translate();
            categoryHeader = "TKUtils.Windows.Store.Headers.Category".Translate();
            searchText = "TKUtils.Windows.Config.Buttons.Search.Label".Translate();
            resetAllText = "TKUtils.Windows.Config.Buttons.ResetAll.Label".Translate();
            enableAllText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            disableAllText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            ctxAscending = "TKUtils.Windows.Store.Context.Ascending".Translate();
            ctxDescending = "TKUtils.Windows.Store.Context.Descending".Translate();
            ctxInfo = "TKUtils.Windows.Store.Context.Info".Translate();

            resetAllTextSize = Text.CalcSize(resetAllText);
            enableAllTextSize = Text.CalcSize(enableAllText);
            disableAllTextSize = Text.CalcSize(disableAllText);
        }

        private static List<Container> GenerateContainers()
        {
            var container = new List<Container>();
            var tradeables = GetTradeables();

            try
            {
                foreach (var thing in tradeables)
                {
                    if (thing?.defName == null)
                    {
                        continue;
                    }

                    var item = StoreInventory.items
                        .FirstOrDefault(i => i != null && (i.defname?.Equals(thing.defName) ?? false));

                    if (item == null)
                    {
                        item = new Item(
                            CalculateToolkitPrice(thing.BaseMarketValue),
                            thing.label?.ToToolkit() ?? thing.defName,
                            thing.defName
                        );

                        TkLogger.Info($@"Added product ""{item.abr}"" to Toolkit's inventory.");
                        StoreInventory.items.Add(item);
                    }
                    else
                    {
                        item.abr ??= thing.label?.ToToolkit() ?? thing.defName;
                    }

                    container.Add(new Container {Item = item, Thing = thing, Enabled = item.price > 0});
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not generate containers!", e);
            }

            return container;
        }

        private class Container
        {
            private TaggedString categoryCached;
            public bool Enabled;

            public Item Item;
            public ThingDef Thing;

            public TaggedString Category
            {
                get
                {
                    if (!categoryCached.NullOrEmpty())
                    {
                        return categoryCached;
                    }

                    var category = Thing?.FirstThingCategory?.LabelCap.RawText ?? string.Empty;

                    if (category.NullOrEmpty() && Thing?.race != null)
                    {
                        category = "TechLevel_Animal".Translate().CapitalizeFirst();
                    }

                    categoryCached = category;
                    return categoryCached;
                }
            }

            public void Update()
            {
                if (Item.price == 0)
                {
                    Enabled = false;
                    Item.price = -10;
                    return;
                }

                if (Enabled && Item.price < 0)
                {
                    Item.price = CalculateToolkitPrice(Thing.BaseMarketValue);
                }
                else if (!Enabled && Item.price > 0)
                {
                    Item.price = -10;
                }

                Enabled = Item.price > 0;
            }
        }
    }
}

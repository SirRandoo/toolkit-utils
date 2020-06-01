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

    public class StoreDialog : Window
    {
        private static readonly List<Container> Containers = new List<Container>();
        private string categoryFilter = "";
        private TaggedString categoryHeader;
        private bool closeCalled;
        private IEnumerator<int> containerGenerator;
        private bool ctrlKeyDown;
        private TaggedString ctxAscending;
        private TaggedString ctxDescending;
        private TaggedString ctxInfo;

        private string currentQuery = "";
        private TaggedString disableAllText;
        private Vector2 disableAllTextSize;
        private TaggedString enableAllText;
        private Vector2 enableAllTextSize;
        private TaggedString fetchingText;
        private Vector2 fetchingTextSize;
        private string lastQuery = "";
        private string modFilter = "";
        private TaggedString nameHeader;
        private TaggedString priceHeader;
        private TaggedString refreshText;
        private Vector2 refreshTextSize;
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

        public StoreDialog()
        {
            doCloseX = true;

            GetTranslationStrings();
            optionalTitle = title;
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);

        public override void PostOpen()
        {
            // Realistically, no one should be generating new ThingDefs past
            // post init, so the store will cache the containers for use later.
            if (Containers.NullOrEmpty())
            {
                containerGenerator = GenerateContainers(Containers).GetEnumerator();
            }

            base.PostOpen();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(inRect);
            var listing = new Listing_Standard {maxOneColumn = true};
            GameFont fontCache = Text.Font;
            TextAnchor anchorCache = Text.Anchor;
            Text.Font = GameFont.Small;

            DrawStoreHeader(inRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            bool wrapped = Text.WordWrap;

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

            var position = new Rect(0f, infoHeaderRect.y + Text.LineHeight / 2f - 4f, 8f, 8f);
            switch (sorter)
            {
                case Sorter.Name:
                    position.x = infoHeaderRect.x;
                    break;
                case Sorter.Price:
                    position.x = priceHeaderRect.x;
                    break;
                case Sorter.Category:
                    position.x = categoryHeaderRect.x;
                    break;
            }

            GUI.DrawTexture(
                position,
                sortMode != SortMode.Descending ? Textures.SortingAscend : Textures.SortingDescend
            );
            GUI.EndGroup();

            List<Container> effectiveWorkingList = results ?? Containers;
            int total = effectiveWorkingList.Count;
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
                Text.LineHeight * scale * total
            );
            var items = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);

            if (containerGenerator != null && TkSettings.StoreLoading)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;

                var fetchingRect = new Rect(
                    0f,
                    contentArea.height / 2f - Text.LineHeight,
                    contentArea.width,
                    Text.LineHeight
                );

                Widgets.Label(fetchingRect, fetchingText);

                Text.Font = fontCache;
                Text.Anchor = anchorCache;

                var progressRect = new Rect(
                    0f,
                    fetchingRect.y + Text.LineHeight * 1.25f,
                    contentArea.width * 0.8f,
                    Text.LineHeight
                );
                progressRect.x = (contentArea.width - progressRect.width) / 2f;

                Widgets.FillableBar(progressRect, (float) containerGenerator.Current / StoreInventory.items.Count);
                GUI.EndGroup();

                Text.WordWrap = wrapped;
                Text.Font = fontCache;
                GUI.EndGroup();
                return;
            }

            listing.BeginScrollView(items, ref scrollPos, ref viewPort);
            for (var index = 0; index < effectiveWorkingList.Count; index++)
            {
                Container item = effectiveWorkingList[index];
                Rect lineRect = listing.GetRect(Text.LineHeight * scale);

                if (!lineRect.IsRegionVisible(items, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                var infoRect = new Rect(
                    27f,
                    lineRect.y,
                    infoHeaderRect.width,
                    lineRect.height
                );

                item.DrawItemInfo(infoRect);

                if (infoRect.WasRightClicked())
                {
                    item.InfoContextOptions ??= new List<FloatMenuOption>
                    {
                        new FloatMenuOption(
                            ctxInfo,
                            () => Find.WindowStack.Add(new Dialog_InfoCard(item.Thing))
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Toggle".Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Mod".Translate(item.Mod),
                            () =>
                            {
                                modFilter = item.Mod;
                                Notify__SearchRequested();
                            }
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

                    Find.WindowStack.Add(new FloatMenu(item.InfoContextOptions));
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
                    item.PriceContextOptions ??= new List<FloatMenuOption>
                    {
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Toggle".Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Mod".Translate(item.Mod),
                            () =>
                            {
                                modFilter = item.Mod;
                                Notify__SearchRequested();
                            }
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

                    Find.WindowStack.Add(new FloatMenu(item.PriceContextOptions));
                }

                var categoryRect = new Rect(
                    priceRect.x + priceRect.width + 5f,
                    lineRect.y,
                    categoryHeaderRect.width,
                    lineRect.height
                );

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(categoryRect, item.Category);
                Text.Anchor = anchorCache;

                if (categoryRect.WasRightClicked())
                {
                    item.CategoryContextOptions ??= new List<FloatMenuOption>
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
                            "TKUtils.Windows.Store.Context.Mod".Translate(item.Mod),
                            () =>
                            {
                                modFilter = item.Mod;
                                Notify__SearchRequested();
                            }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.Toggle".Translate(item.Thing?.LabelCap ?? item.Item.abr),
                            () => { item.Enabled = !item.Enabled; }
                        ),
                        new FloatMenuOption(
                            "TKUtils.Windows.Store.Context.EnableAll".Translate(item.Category),
                            () =>
                            {
                                foreach (Container i in Containers.Where(
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
                                foreach (Container i in Containers.Where(
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

                    Find.WindowStack.Add(new FloatMenu(item.CategoryContextOptions));
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
            List<Container> workingList = results ?? Containers;

            currentQuery = Widgets.TextEntryLabeled(searchRect, searchText, currentQuery);

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(searchRect))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                if (categoryFilter.NullOrEmpty() && modFilter.NullOrEmpty())
                {
                    results = null;
                }
                else if (!lastQuery.Equals(currentQuery))
                {
                    Notify__SearchRequested();
                }

                lastQuery = "";
            }

            float buttonWidth = Mathf.Max(resetAllTextSize.x, enableAllTextSize.x, disableAllTextSize.x) * 1.5f;
            float offset = buttonWidth;

            if (Widgets.ButtonText(
                new Rect(line.x + line.width - offset, line.y, buttonWidth, line.height),
                disableAllText
            ))
            {
                foreach (Container item in workingList.Where(i => i.Item.price > 0))
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
                foreach (Container item in workingList.Where(i => i.Item.price < 0))
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
                foreach (Container item in workingList)
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

            if (!modFilter.NullOrEmpty())
            {
                float modFilterWidth = Text.CalcSize(modFilter).x + 2f;
                var modFilterRect = new Rect(
                    filterSection.x + filterOffset,
                    filterSection.y,
                    modFilterWidth + 22f,
                    filterSection.height
                );

                Widgets.DrawHighlight(modFilterRect);
                Widgets.Label(modFilterRect, " " + modFilter);

                if (SettingsHelper.DrawClearButton(modFilterRect))
                {
                    modFilter = null;

                    if (!currentQuery.NullOrEmpty() || !categoryFilter.NullOrEmpty())
                    {
                        Notify__SearchRequested();
                    }
                }

                filterOffset += modFilterRect.width + 5f;
            }

            if (!categoryFilter.NullOrEmpty())
            {
                float categoryFilterWidth = Text.CalcSize(categoryFilter).x + 16f;
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

                    if (!currentQuery.NullOrEmpty() || !modFilter.NullOrEmpty())
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

            if (containerGenerator != null)
            {
                for (var _ = 0; _ < TkSettings.StoreBuildRate; _++)
                {
                    if (!containerGenerator.MoveNext())
                    {
                        containerGenerator = null;
                        break;
                    }

                    Notify__SearchRequested();
                }
            }

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
            List<Container> workingList = Containers;

            if (!modFilter.NullOrEmpty())
            {
                workingList = workingList
                    .Where(i => i.Mod.EqualsIgnoreCase(modFilter))
                    .ToList();
            }

            if (!categoryFilter.NullOrEmpty())
            {
                workingList = workingList
                    .Where(i => i.Category.RawText.EqualsIgnoreCase(categoryFilter))
                    .ToList();
            }

            string serialized = currentQuery?.ToToolkit();

            if (serialized.NullOrEmpty())
            {
                return workingList;
            }

            return workingList
                .Where(
                    i =>
                    {
                        if (i.Item.abr.ToToolkit().Contains(serialized!)
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
            foreach (Container c in Containers.Where(c => c.Item == null))
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
            List<Container> workingList = results ?? Containers;

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
            refreshText = "TKUtils.Windows.Config.Buttons.Refresh.Label".Translate();
            fetchingText = "TKUtils.Windows.Store.Screen.Text".Translate();

            refreshTextSize = Text.CalcSize(refreshText);
            fetchingTextSize = Text.CalcSize(fetchingText);
            resetAllTextSize = Text.CalcSize(resetAllText);
            enableAllTextSize = Text.CalcSize(enableAllText);
            disableAllTextSize = Text.CalcSize(disableAllText);
        }

        private static IEnumerable<int> GenerateContainers(ICollection<Container> receiver)
        {
            IEnumerable<ThingDef> things = GetTradeables();
            var tracker = 0;

            foreach (ThingDef thing in things)
            {
                try
                {
                    if (thing?.defName == null)
                    {
                        continue;
                    }

                    Item item = StoreInventory.items
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

                    receiver.Add(
                        new Container
                        {
                            Item = item,
                            Thing = thing,
                            Enabled = item.price > 0,
                            Mod = thing.modContentPack?.Name ?? "ModNameMissing"
                        }
                    );
                }
                catch (Exception e)
                {
                    TkLogger.Error($@"Could not generate a container for ""{thing?.defName}""!", e);

                    TkLogger.Info(
                        $@"Errored thing data is as followed:\nDefName={thing?.defName.ToStringSafe()}\nBaseMarketValue={thing?.BaseMarketValue:N0}\nLabel={thing?.label?.ToStringSafe()}\nModName={thing?.modContentPack?.Name?.ToStringSafe()}"
                    );
                }

                yield return tracker++;
            }

            yield return tracker;
        }

        private class Container
        {
            private TaggedString categoryCached;
            public List<FloatMenuOption> CategoryContextOptions;
            public bool Enabled;

            public List<FloatMenuOption> InfoContextOptions;

            public Item Item;
            public string Mod;
            public List<FloatMenuOption> PriceContextOptions;
            public ThingDef Thing;

            public TaggedString Category
            {
                get
                {
                    if (!categoryCached.NullOrEmpty())
                    {
                        return categoryCached;
                    }

                    string category = Thing?.FirstThingCategory?.LabelCap.RawText ?? string.Empty;

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

            public void DrawItemInfo(Rect canvas)
            {
                var iconRegion = new Rect(27f, canvas.y, 27f, canvas.height);
                var labelRegion = new Rect(iconRegion.width + 5f + 27f, canvas.y, canvas.width - 30f, canvas.height);

                Widgets.Checkbox(0f, canvas.y, ref Enabled, paintable: true);

                TextAnchor cache = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRegion, Thing?.LabelCap ?? Item.abr);
                Text.Anchor = cache;

                if (Thing != null)
                {
                    Widgets.ThingIcon(iconRegion, Thing);
                }

                if (Widgets.ButtonInvisible(canvas, false))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(Thing));
                }

                Widgets.DrawHighlightIfMouseover(canvas);
            }
        }
    }
}

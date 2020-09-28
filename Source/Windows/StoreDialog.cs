using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public enum SortMode { Ascending, Descending }

    public enum Sorter { Name, Cost, Category, AddCost, RemoveCost }


    [StaticConstructorOnStartup]
    public class StoreDialog : Window
    {
        private const float LineScale = 1.25f;

        internal static readonly List<KarmaType> KarmaTypes = Enum.GetNames(typeof(KarmaType))
           .Select(t => (KarmaType) Enum.Parse(typeof(KarmaType), t))
           .ToList();

        private static readonly List<TechLevel> TechLevels = Enum.GetNames(typeof(TechLevel))
           .Select(t => (TechLevel) Enum.Parse(typeof(TechLevel), t))
           .ToList();

        private static IEnumerator<ThingItem> _validator;
        private static readonly Color OverlayBackgroundColor = new Color(0.13f, 0.16f, 0.17f);

        private readonly Dictionary<ThingItem, List<FloatMenuOption>> categoryOptionsCache =
            new Dictionary<ThingItem, List<FloatMenuOption>>();

        private readonly ThingItemFilterManager filterManager =
            new ThingItemFilterManager() {UniqueFilters = new List<FilterTypes> {FilterTypes.Stackable}};

        private readonly KarmaType thingKarmaType;
        private string categoryHeader;
        private Vector2 categoryHeaderSize;
        private bool categorySearch;
        private bool closeCalled;
        private bool ctrlKeyDown;

        private string currentQuery = "";
        private string customNameText;
        private string disableAllText;
        private Vector2 disableAllTextSize;
        private string enableAllText;
        private Vector2 enableAllTextSize;
        private ThingItem expanded;
        private string expandedName;
        private bool filterMenuActive;
        private string karmaTypeText;
        private string lastQuery = "";
        private string localize;
        private string nameHeader;
        private string noCustomKarmaText;
        private string priceHeader;
        private string quantityLimitText;
        private string resetAllText;

        private Vector2 resetAllTextSize;
        private string resetText;
        private List<ThingItem> results;
        private Vector2 scrollPos = Vector2.zero;
        private string searchText;
        private Vector2 searchTextSize;
        private bool shftKeyDown;
        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;
        private string stuffText;
        private string title;

        static StoreDialog()
        {
            Data.Items = GenerateContainers().Where(c => c != null).ToList();
            _validator = GenerateContainers().GetEnumerator();
        }

        public StoreDialog()
        {
            doCloseX = true;

            GetTranslationStrings();
            optionalTitle = title;
            thingKarmaType = DefDatabase<StoreIncidentVariables>.GetNamedSilentFail("Item").karmaType;
        }

        public bool FilterMenuActive
        {
            get => filterMenuActive;
            private set
            {
                filterMenuActive = value;

                if (!filterMenuActive)
                {
                    Notify__SearchRequested();
                }
            }
        }

        private List<ThingItem> CurrentWorkingList => results ?? (Data.Items ??= new List<ThingItem>());

        private Sorter Sorter
        {
            get => sorter;
            set
            {
                if (sorter != value)
                {
                    SortMode = SortMode.Descending;
                }

                sorter = value;
                SortCurrentWorkingList();
            }
        }

        private SortMode SortMode
        {
            get => sortMode;
            set
            {
                sortMode = value;
                SortCurrentWorkingList();
            }
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);

        public override void PreOpen()
        {
            base.PreOpen();
            GenerateFilters();
        }

        private void GenerateFilters()
        {
            filterManager.RegisterFilter(
                FilterTypes.Research,
                new ThingItemFilter
                {
                    Id = "Researched",
                    Filter = ThingItemFilter.FilterByResearched,
                    Label = "TKUtils.StoreFilters.Researched".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Research,
                new ThingItemFilter
                {
                    Id = "NotResearched",
                    Filter = ThingItemFilter.FilterByNotResearched,
                    Label = "TKUtils.StoreFilters.NotResearched".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Stackable,
                new ThingItemFilter
                {
                    Id = "Stackable",
                    Filter = ThingItemFilter.FilterByStackable,
                    Label = "TKUtils.StoreFilters.Stackable".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Stackable,
                new ThingItemFilter
                {
                    Id = "NonStackable",
                    Filter = ThingItemFilter.FilterByNonStackable,
                    Label = "TKUtils.StoreFilters.NonStackable".Localize().CapitalizeFirst()
                }
            );

            foreach (TechLevel techLevel in TechLevels)
            {
                filterManager.RegisterFilter(
                    FilterTypes.TechLevel,
                    new ThingItemFilter
                    {
                        Id = $"TechLevel.{techLevel}",
                        Filter = t => ThingItemFilter.FilterByTechLevel(t, techLevel),
                        Label = techLevel.ToString().CapitalizeFirst()
                    }
                );
            }

            if (Data.Items.NullOrEmpty())
            {
                return;
            }

            foreach (string modName in Data.Items.Select(i => i.Mod).Distinct())
            {
                filterManager.RegisterFilter(
                    FilterTypes.Mod,
                    new ThingItemFilter
                    {
                        Id = $"Mod.{modName}",
                        Filter = t => ThingItemFilter.FilterByMod(t, modName),
                        Label = modName
                    }
                );
            }

            foreach (string category in Data.Items.Select(i => i.Category).Distinct())
            {
                filterManager.RegisterFilter(
                    FilterTypes.Category,
                    new ThingItemFilter
                    {
                        Id = $"Categories.{category}",
                        Filter = t => ThingItemFilter.FilterByCategory(t, category),
                        Label = category
                    }
                );
            }
        }

        private void DrawThingSettings(Rect inRect)
        {
            expanded.Data ??= new ItemData {KarmaType = thingKarmaType};
            string removeKarma = expanded.Data.KarmaType == null
                ? noCustomKarmaText
                : Enum.GetName(typeof(KarmaType), expanded.Data.KarmaType);


            var listing = new Listing_Standard();
            listing.Begin(inRect);

            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
            expandedName = Widgets.TextField(nameField, expandedName).ToToolkit();

            if (expandedName.Length > 0 && SettingsHelper.DrawClearButton(nameField))
            {
                expandedName = "";
                expanded.Data.CustomName = null;
            }

            SettingsHelper.DrawLabelAnchored(nameLabel, customNameText, TextAnchor.MiddleLeft);

            listing.Gap(4f);
            (Rect karmaTypeLabel, Rect karmaTypeField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
            SettingsHelper.DrawLabelAnchored(karmaTypeLabel, karmaTypeText, TextAnchor.MiddleLeft);
            if (Widgets.ButtonText(karmaTypeField, removeKarma))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        KarmaTypes.Select(
                                k => new FloatMenuOption(
                                    Enum.GetName(typeof(KarmaType), k),
                                    () => expanded.Data.KarmaType = k
                                )
                            )
                           .ToList()
                    )
                );
            }


            listing.GapLine(4f);
            listing.CheckboxLabeled(quantityLimitText, ref expanded.Data.HasQuantityLimit);

            if (expanded.Data.HasQuantityLimit)
            {
                (Rect _, Rect quantityLimitField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
                var quantityLimitBuffer = expanded.Data.QuantityLimit.ToString();
                Widgets.TextFieldNumeric(
                    quantityLimitField,
                    ref expanded.Data.QuantityLimit,
                    ref quantityLimitBuffer,
                    1f
                );
            }


            if (expanded.Thing.IsStuff)
            {
                listing.Gap(2f);
                listing.GapLine();
                Rect stuffLineRect = listing.GetRect(Text.LineHeight * LineScale);
                var stuffLabelRect = new Rect(
                    stuffLineRect.x,
                    stuffLineRect.y,
                    stuffLineRect.width - stuffLineRect.height - 5f,
                    stuffLineRect.height
                );
                var stuffCheckRect = new Rect(
                    stuffLabelRect.x + stuffLabelRect.width + 5f,
                    stuffLabelRect.y,
                    stuffLineRect.height,
                    stuffLineRect.height
                );

                SettingsHelper.DrawLabelAnchored(stuffLineRect, stuffText, TextAnchor.MiddleLeft);
                Widgets.Checkbox(
                    stuffCheckRect.position,
                    ref expanded.Data.IsStuffAllowed,
                    stuffCheckRect.height,
                    paintable: true
                );
            }

            if (Widgets.ButtonText(
                new Rect(
                    10f,
                    inRect.height - Text.LineHeight * LineScale,
                    inRect.width - 20f,
                    Text.LineHeight * LineScale
                ),
                resetText
            ))
            {
                expanded.Data.CustomName = expanded.GetDefaultName();
                expanded.Data.KarmaType = null;
                expanded.Data.HasQuantityLimit = false;
                expanded.Data.QuantityLimit = -1;
            }

            listing.End();
        }

        private void DoExpandedDialog(Rect inRect)
        {
            float expandedWidth = inRect.width * 0.284f;
            Vector2 center = inRect.center;

            Rect expandedDialog = new Rect(
                center.x - expandedWidth / 2f,
                center.y - Text.LineHeight * LineScale * 5f,
                expandedWidth,
                Text.LineHeight * LineScale * 10f
            ).ExpandedBy(StandardMargin * 2f);

            Widgets.DrawBoxSolid(expandedDialog, OverlayBackgroundColor);
            Widgets.Label(
                new Rect(
                    expandedDialog.x + 8f,
                    expandedDialog.y + 5f,
                    expandedDialog.width - 30f,
                    Text.LineHeight * LineScale
                ),
                "TKUtils.Headers.DataDialog".Localize(expanded.Name)
            );

            Widgets.DrawHighlight(
                new Rect(expandedDialog.position, new Vector2(expandedDialog.width, Text.LineHeight * LineScale))
            );

            GUI.BeginGroup(expandedDialog.ContractedBy(StandardMargin * 2f));
            DrawThingSettings(
                new Rect(
                    0f,
                    0f,
                    expandedDialog.width - StandardMargin * 4f,
                    expandedDialog.height - StandardMargin * 4f
                )
            );
            GUI.EndGroup();

            if (Widgets.CloseButtonFor(expandedDialog))
            {
                CloseExpandedMenu();
            }
        }

        private (Rect, Rect, Rect) DoStoreHeaders(Rect inRect)
        {
            var infoHeaderRect = new Rect(30f, 0f, inRect.width * 0.4f - 15f, Text.LineHeight);
            var priceHeaderRect = new Rect(
                infoHeaderRect.width + 35f,
                infoHeaderRect.y,
                inRect.width - infoHeaderRect.width * 2f - 35f,
                Text.LineHeight
            );
            var categoryHeaderRect = new Rect(
                infoHeaderRect.width + priceHeaderRect.width + 35f,
                infoHeaderRect.y,
                infoHeaderRect.width - 16f,
                Text.LineHeight
            );


            GUI.BeginGroup(new Rect(inRect.x, Text.LineHeight * 3f, inRect.width, infoHeaderRect.height));
            Widgets.DrawHighlightIfMouseover(infoHeaderRect);
            Widgets.DrawHighlightIfMouseover(priceHeaderRect);
            Widgets.DrawHighlightIfMouseover(categoryHeaderRect);


            if (Widgets.ButtonText(infoHeaderRect, "   " + nameHeader, false))
            {
                Sorter = Sorter.Name;
                SortMode = SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            }

            if (Widgets.ButtonText(priceHeaderRect, "   " + priceHeader, false))
            {
                Sorter = Sorter.Cost;
                SortMode = SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            }

            if (Widgets.ButtonText(categoryHeaderRect, "   " + categoryHeader, false))
            {
                Sorter = Sorter.Category;
                SortMode = SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            }

            DrawSortIcon(infoHeaderRect.y, infoHeaderRect.x, priceHeaderRect.x, categoryHeaderRect.x);

            return (infoHeaderRect, priceHeaderRect, categoryHeaderRect);
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(inRect);
            var listing = new Listing_Standard {maxOneColumn = true};
            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 4f
            );
            GameFont fontCache = Text.Font;
            Text.Font = GameFont.Small;

            DrawStoreHeader(inRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            if (FilterMenuActive)
            {
                DoFilterMenu(contentArea);
                GUI.EndGroup();
                return;
            }

            if (expanded != null)
            {
                DoExpandedDialog(contentArea);
                GUI.EndGroup();
                return;
            }

            bool wrapped = Text.WordWrap;

            Text.WordWrap = false;

            (Rect infoHeaderRect, Rect priceHeaderRect, Rect categoryHeaderRect) = DoStoreHeaders(
                new Rect(0f, Text.LineHeight * 3f, inRect.width, Text.LineHeight)
            );

            GUI.EndGroup();

            List<ThingItem> effectiveWorkingList = CurrentWorkingList;
            int total = effectiveWorkingList.Count;

            var viewPort = new Rect(0f, 0f, contentArea.width - 16f, Text.LineHeight * LineScale * total);
            var items = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(items, ref scrollPos, ref viewPort);
            for (var index = 0; index < total; index++)
            {
                ThingItem item = effectiveWorkingList[index];
                Rect lineRect = listing.GetRect(Text.LineHeight * LineScale);

                if (!lineRect.IsRegionVisible(items, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                DrawThingItem(lineRect, infoHeaderRect, item, priceHeaderRect, categoryHeaderRect);

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

        private void DoFilterMenu(Rect canvas)
        {
            float filterWidth = canvas.width * 0.284f;
            Vector2 center = canvas.center;

            Rect filterDialog = new Rect(
                center.x - filterWidth / 2f,
                center.y - canvas.height * 0.75f / 2f,
                filterWidth,
                canvas.height * 0.75f
            ).ExpandedBy(StandardMargin * 2f);

            Widgets.DrawBoxSolid(filterDialog, OverlayBackgroundColor);
            Widgets.Label(
                new Rect(
                    filterDialog.x + 8f,
                    filterDialog.y + 5f,
                    filterDialog.width - 30f,
                    Text.LineHeight * LineScale
                ),
                localize
            );

            Widgets.DrawHighlight(
                new Rect(filterDialog.x, filterDialog.y, filterDialog.width, Text.LineHeight * LineScale)
            );

            GUI.BeginGroup(filterDialog.ContractedBy(StandardMargin * 2f));
            filterManager.DrawFilters(
                new Rect(0f, 0f, filterDialog.width - StandardMargin * 4f, filterDialog.height - StandardMargin * 4f)
            );
            GUI.EndGroup();

            if (Widgets.CloseButtonFor(filterDialog))
            {
                FilterMenuActive = false;
            }
        }

        private void DrawThingItem(
            Rect lineRect,
            Rect infoHeaderRect,
            ThingItem item,
            Rect priceHeaderRect,
            Rect categoryHeaderRect
        )
        {
            var infoRect = new Rect(27f, lineRect.y, infoHeaderRect.width, lineRect.height);
            DrawInfoFor(infoRect, item);

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

            var categoryRect = new Rect(
                priceRect.x + priceRect.width + 5f,
                lineRect.y,
                categoryHeaderRect.width - 27f - 16f,
                lineRect.height
            );

            SettingsHelper.DrawLabelAnchored(categoryRect, item.Category, TextAnchor.MiddleLeft);
            DrawCategoryCtxFor(categoryRect, item);

            var settingsRect = new Rect(categoryRect.x + categoryRect.width + 5f, lineRect.y, 27f, lineRect.height);
            GUI.DrawTexture(settingsRect, Textures.Gear);

            if (Widgets.ButtonInvisible(settingsRect))
            {
                expanded = item;

                if (expanded.Data?.CustomName != null)
                {
                    expandedName = expanded.Data.CustomName;
                }
            }
        }

        private void DrawSortIcon(float y, float infoX, float priceX, float categoryX)
        {
            var position = new Rect(0f, y + Text.LineHeight / 2f - 4f, 8f, 8f);

            switch (Sorter)
            {
                case Sorter.Name:
                    position.x = infoX;
                    break;
                case Sorter.Cost:
                    position.x = priceX;
                    break;
                case Sorter.Category:
                    position.x = categoryX;
                    break;
            }

            GUI.DrawTexture(
                position,
                SortMode != SortMode.Descending ? Textures.SortingAscend : Textures.SortingDescend
            );
        }

        private void DrawCategoryCtxFor(Rect categoryRect, ThingItem item)
        {
            if (!categoryRect.WasRightClicked())
            {
                return;
            }

            if (!categoryOptionsCache.TryGetValue(item, out List<FloatMenuOption> optionCache))
            {
                optionCache = new List<FloatMenuOption>
                {
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.EnableCategory".Localize(item.Category),
                        () =>
                        {
                            foreach (ThingItem i in Data.Items.Where(i => i.Category.EqualsIgnoreCase(item.Category)))
                            {
                                i.IsEnabled = true;
                                i.Update();
                            }
                        }
                    ),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.DisableCategory".Localize(item.Category),
                        () =>
                        {
                            foreach (ThingItem i in Data.Items.Where(i => i.Category.EqualsIgnoreCase(item.Category)))
                            {
                                i.IsEnabled = false;
                                i.Update();
                            }
                        }
                    )
                };

                categoryOptionsCache[item] = optionCache;
            }

            Find.WindowStack.Add(new FloatMenu(optionCache));
        }

        private void DrawStoreHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var line = new Rect(canvas.x, canvas.y, canvas.width, Text.LineHeight);
            var searchRect = new Rect(line.x, line.y, line.width * 0.18f, line.height);
            Rect searchTextRect = searchRect.WithWidth(searchTextSize.x);
            var searchFieldRect = new Rect(
                searchTextRect.x + searchTextRect.width + 5f,
                searchTextRect.y,
                searchRect.width - searchTextRect.width - 5f,
                searchTextRect.height
            );
            List<ThingItem> workingList = results ?? Data.Items;

            Widgets.Label(searchTextRect, searchText);
            currentQuery = Widgets.TextField(searchFieldRect, currentQuery);

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(searchRect))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                if (!lastQuery.Equals(currentQuery))
                {
                    Notify__SearchRequested();
                }

                lastQuery = "";
            }

            var filterButtonRect = new Rect(
                searchRect.x + searchRect.width + 2f,
                searchRect.y,
                searchRect.height,
                searchRect.height
            );
            DrawFilterButton(filterButtonRect);
            DrawCategorySearchModifier(searchFieldRect, line);

            float buttonWidth = Mathf.Max(resetAllTextSize.x, enableAllTextSize.x, disableAllTextSize.x) + 16f;
            var buttonRect = new Rect(line.x + line.width - buttonWidth, line.y, buttonWidth, line.height);

            DrawGlobalDisableButton(buttonRect, workingList);
            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalEnableButton(buttonRect, workingList);
            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalResetButton(buttonRect, workingList);

            GUI.EndGroup();
        }

        private void DrawCategorySearchModifier(Rect searchFieldRect, Rect line)
        {
            var categoryLine = new Rect(
                searchFieldRect.x,
                Text.LineHeight + 1f,
                categoryHeaderSize.x + 16f,
                line.height
            );
            var categoryCheck = new Rect(categoryLine.x + 2f, categoryLine.y + 2f, 12f, 12f);
            var categoryText = new Rect(
                categoryCheck.x + 16f,
                categoryLine.y,
                categoryHeaderSize.x,
                categoryLine.height
            );

            GUI.DrawTexture(categoryCheck, categorySearch ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            SettingsHelper.DrawSmallLabelAnchored(categoryText, categoryHeader, TextAnchor.UpperLeft);

            if (Widgets.ButtonInvisible(categoryLine))
            {
                categorySearch = !categorySearch;

                if (!currentQuery.NullOrEmpty())
                {
                    Notify__SearchRequested();
                }
            }
        }

        private void DrawGlobalDisableButton(Rect buttonRect, IEnumerable<ThingItem> workingList)
        {
            if (Widgets.ButtonText(buttonRect, disableAllText))
            {
                foreach (ThingItem item in workingList.Where(i => i.Item.price > 0))
                {
                    item.IsEnabled = false;
                    item.Update();
                }
            }
        }

        private void DrawFilterButton(Rect region)
        {
            if (Widgets.ButtonImage(region, Textures.Filter))
            {
                FilterMenuActive = true;
            }
        }

        private void DrawGlobalEnableButton(Rect buttonRect, IEnumerable<ThingItem> workingList)
        {
            if (Widgets.ButtonText(buttonRect, enableAllText))
            {
                foreach (ThingItem item in workingList.Where(i => i.Item.price < 0))
                {
                    item.IsEnabled = true;
                    item.Update();
                }
            }
        }

        private void DrawGlobalResetButton(Rect buttonRect, IEnumerable<ThingItem> workingList)
        {
            if (Widgets.ButtonText(buttonRect, resetAllText))
            {
                foreach (ThingItem item in workingList)
                {
                    item.Item.abr = item.Thing.label.ToToolkit();
                    item.Item.price = item.Thing.CalculateStorePrice();
                    item.Data.KarmaType = null;
                    item.Data.CustomName = null;
                    item.Data.QuantityLimit = -1;
                }
            }
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (_validator != null)
            {
                AdvanceValidator();
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

        private void AdvanceValidator()
        {
            for (var _ = 0; _ < TkSettings.StoreBuildRate; _++)
            {
                if (!_validator.MoveNext())
                {
                    _validator = null;
                    break;
                }

                ThingItem latest = _validator.Current;

                if (latest == null)
                {
                    continue;
                }

                Data.Items ??= new List<ThingItem>();

                if (Data.Items.Any(c => c.Thing.defName.Equals(latest.Thing.defName)))
                {
                    continue;
                }

                Data.Items.Add(latest);
            }

            Notify__SearchRequested();
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults().ToList();
            SortCurrentWorkingList();
        }

        private IEnumerable<ThingItem> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();
            IEnumerable<ThingItem> workingList = filterManager.FilterItems(Data.Items);

            if (serialized.NullOrEmpty())
            {
                return workingList;
            }

            return (categorySearch
                ? GetCategorySearchResults(serialized, workingList)
                : GetNameSearchResults(serialized, workingList)).ToList();
        }

        private static IEnumerable<ThingItem> GetCategorySearchResults(string input, IEnumerable<ThingItem> items)
        {
            return items.Where(
                i => i.Category.ToToolkit().Contains(input) || i.Category.ToToolkit().EqualsIgnoreCase(input)
            );
        }

        private static IEnumerable<ThingItem> GetNameSearchResults(string input, IEnumerable<ThingItem> items)
        {
            return items.Where(i => i.Name.ToToolkit().Contains(input) || i.Name.ToToolkit().EqualsIgnoreCase(input));
        }

        public override void PreClose()
        {
            foreach (ThingItem c in Data.Items.Where(c => c.Item == null))
            {
                c.Item = new Item(c.Thing.CalculateStorePrice(), c.Thing.LabelCap.RawText.ToToolkit(), c.Thing.defName);

                StoreInventory.items.Add(c.Item);
            }

            base.PreClose();
        }

        private static void DrawInfoFor(Rect canvas, ThingItem item)
        {
            var iconRegion = new Rect(27f, canvas.y, 27f, canvas.height);
            var labelRegion = new Rect(iconRegion.width + 5f + 27f, canvas.y, canvas.width - 30f, canvas.height);

            Widgets.Checkbox(0f, canvas.y, ref item.IsEnabled, paintable: true);

            if (item.Thing != null)
            {
                SettingsHelper.DrawLabelAnchored(labelRegion, item.Name, TextAnchor.MiddleLeft);
                Widgets.ThingIcon(iconRegion, item.Thing);

                if (Current.Game == null)
                {
                    return;
                }

                Widgets.DrawHighlightIfMouseover(canvas);

                if (Widgets.ButtonInvisible(canvas, false))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(item.Thing));
                }

                return;
            }

            GUI.color = Color.yellow;
            SettingsHelper.DrawLabelAnchored(labelRegion, item.Name, TextAnchor.MiddleLeft);
            GUI.color = Color.white;
            GUI.DrawTexture(iconRegion, Textures.QuestionMark);
        }

        public override void PostClose()
        {
            closeCalled = true;
            Store_ItemEditor.UpdateStoreItemList();

            base.PostClose();
        }

        public static IEnumerable<ThingDef> GetTradeables()
        {
            return Store_ItemEditor.GetDefaultItems();
        }

        private void SortCurrentWorkingList()
        {
            List<ThingItem> workingList = CurrentWorkingList;

            switch (Sorter)
            {
                case Sorter.Name when SortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Name);
                    results = workingList;
                    return;
                case Sorter.Name when SortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Name);
                    results = workingList;
                    return;
                case Sorter.Cost when SortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Price);
                    results = workingList;
                    return;
                case Sorter.Cost when SortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Price);
                    results = workingList;
                    return;
                case Sorter.Category when SortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Category);
                    results = workingList;
                    return;
                case Sorter.Category when SortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Category);
                    results = workingList;
                    return;
                default:
                    return;
            }
        }

        public override void OnCancelKeyPressed()
        {
            if (FilterMenuActive)
            {
                FilterMenuActive = false;
                Event.current.Use();
                return;
            }

            if (expanded != null)
            {
                CloseExpandedMenu();
                Event.current.Use();
                return;
            }

            base.OnCancelKeyPressed();
        }

        public override void OnAcceptKeyPressed()
        {
            if (FilterMenuActive)
            {
                FilterMenuActive = false;
                Event.current.Use();
                return;
            }

            if (expanded != null)
            {
                CloseExpandedMenu();
                Event.current.Use();
                return;
            }

            base.OnAcceptKeyPressed();
        }

        private void CloseExpandedMenu()
        {
            if (expanded.Data != null)
            {
                expanded.Data.CustomName = expandedName.NullOrEmpty() ? null : expandedName;
            }

            if (!expandedName.NullOrEmpty())
            {
                SortCurrentWorkingList();
            }

            expanded = null;
            expandedName = null;
        }

        private void GetTranslationStrings()
        {
            title = "TKUtils.ItemStore.Title".Localize();
            nameHeader = "TKUtils.Headers.Name".Localize();
            priceHeader = "TKUtils.Headers.Price".Localize();
            categoryHeader = "TKUtils.ItemStore.Category".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            resetAllText = "TKUtils.Buttons.ResetAll".Localize();
            enableAllText = "TKUtils.Buttons.EnableAll".Localize();
            disableAllText = "TKUtils.Buttons.DisableAll".Localize();
            noCustomKarmaText = "TKUtils.TraitStore.NoCustomKarmaType".Localize();
            resetText = "TKUtils.Buttons.ResetAll".Localize();
            customNameText = "TKUtils.Inputs.CustomName".Localize();
            karmaTypeText = "TKUtils.IncidentEditor.Karma".Localize();
            localize = "TKUtils.Headers.FilterDialog".Localize();
            stuffText = "TKUtils.ItemStore.Stuff".Localize();
            quantityLimitText = "TKUtils.ItemStore.QuantityLimit".Localize();

            resetAllTextSize = Text.CalcSize(resetAllText);
            enableAllTextSize = Text.CalcSize(enableAllText);
            disableAllTextSize = Text.CalcSize(disableAllText);
            searchTextSize = Text.CalcSize(searchText);
            categoryHeaderSize = Text.CalcSize(categoryHeader);
        }

        internal static IEnumerable<ThingItem> GenerateContainers()
        {
            IEnumerable<ThingDef> things = GetTradeables();
            var builder = new StringBuilder();

            foreach (ThingDef thing in things)
            {
                if (thing?.defName == null)
                {
                    continue;
                }

                ThingItem thingItem = null;

                try
                {
                    Item item = StoreInventory.items.FirstOrDefault(i => i?.defname?.Equals(thing.defName) ?? false);

                    if (item == null)
                    {
                        item = new Item(
                            thing.CalculateStorePrice(),
                            thing.label?.ToToolkit() ?? thing.defName,
                            thing.defName
                        );

                        StoreInventory.items.Add(item);
                    }
                    else
                    {
                        item.abr ??= thing.label?.ToToolkit() ?? thing.defName;
                    }

                    thingItem = ThingItem.FromData(item, thing);
                }
                catch (Exception e)
                {
                    builder.AppendInNewLine($@"- {thing.defName} | ERROR: {e.GetType().Name}({e.Message})");
                }

                yield return thingItem;
            }

            if (builder.Length <= 0)
            {
                yield break;
            }

            builder.Insert(0, "The following containers failed to generate:\n");
            TkLogger.Warn(builder.ToString());
        }
    }
}

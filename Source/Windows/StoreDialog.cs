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

    public enum FilterTypes { Mod, Category, TechLevel }


    [StaticConstructorOnStartup]
    public class StoreDialog : Window
    {
        private const float LineScale = 1.25f;

        internal static readonly List<KarmaType> KarmaTypes = Enum.GetNames(typeof(KarmaType))
           .Select(t => (KarmaType) Enum.Parse(typeof(KarmaType), t))
           .ToList();

        private static IEnumerator<ThingItem> _validator;

        private readonly Dictionary<ThingItem, List<FloatMenuOption>> catCtxCache =
            new Dictionary<ThingItem, List<FloatMenuOption>>();

        private readonly List<StoreItemFilter> filters = new List<StoreItemFilter>();

        private readonly Dictionary<ThingItem, List<FloatMenuOption>> infoCtxCache =
            new Dictionary<ThingItem, List<FloatMenuOption>>();

        private readonly Dictionary<ThingItem, List<FloatMenuOption>> priceCtxCache =
            new Dictionary<ThingItem, List<FloatMenuOption>>();

        private readonly KarmaType thingKarmaType;
        private string categoryHeader;
        private bool closeCalled;
        private bool ctrlKeyDown;
        private string ctxAscending;
        private string ctxDescending;
        private string ctxInfo;

        private string currentQuery = "";
        private string customNameText;
        private string disableAllText;
        private Vector2 disableAllTextSize;
        private string enableAllText;
        private Vector2 enableAllTextSize;
        private ThingItem expanded;
        private string expandedName;
        private string karmaTypeText;
        private string lastQuery = "";
        private string nameHeader;
        private string noCustomKarmaText;
        private string priceHeader;
        private string resetAllText;

        private Vector2 resetAllTextSize;
        private string resetText;
        private List<ThingItem> results;
        private Vector2 scrollPos = Vector2.zero;
        private string searchText;
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

        private List<ThingItem> CurrentWorkingList => results ?? Data.Items;

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
            }

            listing.End();
        }

        private void DoExpandedDialog(Rect inRect)
        {
            float expandedWidth = inRect.width * 0.284f;
            Vector2 center = inRect.center;

            Rect expandedDialog = new Rect(
                center.x - expandedWidth / 2f,
                center.y - Text.LineHeight * LineScale * 4f,
                expandedWidth,
                Text.LineHeight * LineScale * 8f
            ).ExpandedBy(StandardMargin * 2f);

            Widgets.DrawBoxSolid(expandedDialog, new Color(0.13f, 0.16f, 0.17f));
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

        private void DrawThingItem(Rect lineRect,
                                   Rect infoHeaderRect,
                                   ThingItem item,
                                   Rect priceHeaderRect,
                                   Rect categoryHeaderRect)
        {
            var infoRect = new Rect(27f, lineRect.y, infoHeaderRect.width, lineRect.height);
            DrawInfoFor(infoRect, item);
            DrawInfoContextFor(infoRect, item);

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

            DrawPriceContextFor(priceRect, item);

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

        private void DrawInfoContextFor(Rect infoRect, ThingItem item)
        {
            if (!infoRect.WasRightClicked())
            {
                return;
            }

            if (!infoCtxCache.TryGetValue(item, out List<FloatMenuOption> optionCache))
            {
                optionCache = new List<FloatMenuOption>
                {
                    new FloatMenuOption(ctxInfo, () => Find.WindowStack.Add(new Dialog_InfoCard(item.Thing))),
                    new FloatMenuOption("TKUtils.StoreMenu.Filters".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Mod".Localize(item.Mod),
                        () =>
                        {
                            InjectModFilter(item.Mod);
                            Notify__SearchRequested();
                        }
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.State".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Toggle".Localize(item.Name),
                        () => item.IsEnabled = !item.IsEnabled
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.Sorting".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        ctxAscending,
                        () =>
                        {
                            Sorter = Sorter.Name;
                            SortMode = SortMode.Ascending;
                        }
                    ),
                    new FloatMenuOption(
                        ctxDescending,
                        () =>
                        {
                            Sorter = Sorter.Name;
                            SortMode = SortMode.Descending;
                        }
                    )
                };

                if (item.Thing.techLevel != TechLevel.Undefined)
                {
                    optionCache.Insert(
                        1,
                        new FloatMenuOption(
                            "TKUtils.StoreMenu.Technology".Localize(item.Thing.techLevel.ToString()),
                            () =>
                            {
                                InjectTechLevelFilter(item.Thing.techLevel);
                                Notify__SearchRequested();
                            }
                        )
                    );
                }

                infoCtxCache[item] = optionCache;
            }

            Find.WindowStack.Add(new FloatMenu(optionCache));
        }

        private void DrawPriceContextFor(Rect priceRect, ThingItem item)
        {
            if (!priceRect.WasRightClicked())
            {
                return;
            }

            if (!priceCtxCache.TryGetValue(item, out List<FloatMenuOption> optionCache))
            {
                optionCache = new List<FloatMenuOption>
                {
                    new FloatMenuOption("TKUtils.StoreMenu.State".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Toggle".Localize(item.Name),
                        () => item.IsEnabled = !item.IsEnabled
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.Filters".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Mod".Localize(item.Mod),
                        () =>
                        {
                            InjectModFilter(item.Mod);
                            Notify__SearchRequested();
                        }
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.Sorting".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        ctxAscending,
                        () =>
                        {
                            Sorter = Sorter.Cost;
                            SortMode = SortMode.Ascending;
                        }
                    ),
                    new FloatMenuOption(
                        ctxDescending,
                        () =>
                        {
                            Sorter = Sorter.Cost;
                            SortMode = SortMode.Descending;
                        }
                    )
                };

                if (item.Thing.techLevel != TechLevel.Undefined)
                {
                    optionCache.Insert(
                        4,
                        new FloatMenuOption(
                            "TKUtils.StoreMenu.Technology".Localize(item.Thing.techLevel.ToString()),
                            () =>
                            {
                                InjectTechLevelFilter(item.Thing.techLevel);
                                Notify__SearchRequested();
                            }
                        )
                    );
                }

                priceCtxCache[item] = optionCache;
            }

            Find.WindowStack.Add(new FloatMenu(optionCache));
        }

        private void DrawCategoryCtxFor(Rect categoryRect, ThingItem item)
        {
            if (!categoryRect.WasRightClicked())
            {
                return;
            }

            if (!catCtxCache.TryGetValue(item, out List<FloatMenuOption> optionCache))
            {
                optionCache = new List<FloatMenuOption>
                {
                    new FloatMenuOption("TKUtils.StoreMenu.Filters".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Category".Localize(item.Category),
                        () =>
                        {
                            InjectCategoryFilter(item.Category);
                            Notify__SearchRequested();
                        }
                    ),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Mod".Localize(item.Mod),
                        () =>
                        {
                            InjectModFilter(item.Mod);
                            Notify__SearchRequested();
                        }
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.State".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        "TKUtils.StoreMenu.Toggle".Localize(item.Name),
                        () => item.IsEnabled = !item.IsEnabled
                    ),
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
                    ),
                    new FloatMenuOption("TKUtils.StoreMenu.Sorting".Localize().Tagged("i").Tagged("b"), () => { }),
                    new FloatMenuOption(
                        ctxAscending,
                        () =>
                        {
                            Sorter = Sorter.Category;
                            SortMode = SortMode.Ascending;
                        }
                    ),
                    new FloatMenuOption(
                        ctxDescending,
                        () =>
                        {
                            Sorter = Sorter.Category;
                            SortMode = SortMode.Descending;
                        }
                    )
                };

                if (item.Thing.techLevel != TechLevel.Undefined)
                {
                    optionCache.Insert(
                        2,
                        new FloatMenuOption(
                            "TKUtils.StoreMenu.Technology".Localize(item.Thing.techLevel.ToString()),
                            () =>
                            {
                                InjectTechLevelFilter(item.Thing.techLevel);
                                Notify__SearchRequested();
                            }
                        )
                    );
                }

                catCtxCache[item] = optionCache;
            }

            Find.WindowStack.Add(new FloatMenu(optionCache));
        }

        private void DrawStoreHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var line = new Rect(canvas.x, canvas.y, canvas.width, Text.LineHeight);
            var searchRect = new Rect(line.x, line.y, line.width * 0.25f, line.height);
            List<ThingItem> workingList = results ?? Data.Items;

            currentQuery = Widgets.TextEntryLabeled(searchRect, searchText, currentQuery);

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(searchRect))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                if (filters.NullOrEmpty())
                {
                    results = null;
                }
                else if (!lastQuery.Equals(currentQuery))
                {
                    Notify__SearchRequested();
                }

                lastQuery = "";
            }

            float buttonWidth = Mathf.Max(resetAllTextSize.x, enableAllTextSize.x, disableAllTextSize.x) + 16f;
            var buttonRect = new Rect(line.x + line.width - buttonWidth, line.y, buttonWidth, line.height);

            if (Widgets.ButtonText(buttonRect, disableAllText))
            {
                foreach (ThingItem item in workingList.Where(i => i.Item.price > 0))
                {
                    item.IsEnabled = false;
                    item.Update();
                }
            }

            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalEnableButton(buttonRect, workingList);

            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalResetButton(buttonRect, workingList);

            float buttonGroupWidth = line.x + line.width - buttonRect.x;
            DrawFilters(searchRect, line, buttonGroupWidth);

            GUI.EndGroup();
        }

        private void DrawFilters(Rect searchRect, Rect line, float buttonGroupWidth)
        {
            var filterSection = new Rect(
                searchRect.x + searchRect.width + 5f,
                line.y,
                line.width - buttonGroupWidth - searchRect.width - 10f,
                line.height
            );

            var filterOffset = 0f;
            StoreItemFilter toCull = null;
            foreach (StoreItemFilter filter in filters)
            {
                var filterRect = new Rect(
                    filterSection.x + filterOffset,
                    filterSection.y,
                    filter.LabelWidth + 22f,
                    filterSection.height
                );

                Widgets.DrawHighlight(filterRect);
                Widgets.Label(filterRect, filter.Label);

                if (SettingsHelper.DrawClearButton(filterRect))
                {
                    toCull = filter;
                }

                filterOffset += filterRect.width + 2f;
            }

            if (toCull != null)
            {
                filters.Remove(toCull);
                Notify__SearchRequested();
            }
        }

        private void DrawGlobalEnableButton(Rect buttonRect, List<ThingItem> workingList)
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

        private void DrawGlobalResetButton(Rect buttonRect, List<ThingItem> workingList)
        {
            if (Widgets.ButtonText(buttonRect, resetAllText))
            {
                foreach (ThingItem item in workingList)
                {
                    item.Item.abr = item.Thing.label.ToToolkit().Replace(@"\", "");
                    item.Item.price = item.Thing.CalculateStorePrice();
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

            results = GetSearchResults();
            SortCurrentWorkingList();
        }

        private List<ThingItem> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();
            List<ThingItem> workingList = filters.Aggregate(Data.Items, (current, filter) => filter.Filter(current));

            if (serialized.NullOrEmpty())
            {
                return workingList;
            }

            return workingList.Where(
                    i => i.Item.abr.ToToolkit().Contains(serialized!)
                         || i.Item.abr.ToToolkit().EqualsIgnoreCase(serialized)
                )
               .ToList();
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
            SettingsHelper.DrawLabelAnchored(labelRegion, item.Name, TextAnchor.MiddleLeft);

            if (item.Thing != null)
            {
                Widgets.ThingIcon(iconRegion, item.Thing);
            }

            if (Widgets.ButtonInvisible(canvas, false))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(item.Thing));
            }

            Widgets.DrawHighlightIfMouseover(canvas);
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
            List<ThingItem> workingList = results ?? Data.Items;

            switch (Sorter)
            {
                case Sorter.Name when SortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Item.abr);
                    results = workingList;
                    return;
                case Sorter.Name when SortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Item.abr);
                    results = workingList;
                    return;
                case Sorter.Cost when SortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Item.price);
                    results = workingList;
                    return;
                case Sorter.Cost when SortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Item.price);
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
            if (expanded == null)
            {
                base.OnCancelKeyPressed();
            }
            else
            {
                CloseExpandedMenu();
                Event.current.Use();
            }
        }

        public override void OnAcceptKeyPressed()
        {
            if (expanded == null)
            {
                base.OnAcceptKeyPressed();
            }
            else
            {
                CloseExpandedMenu();
                Event.current.Use();
            }
        }

        private void CloseExpandedMenu()
        {
            if (expanded.Data != null && (expanded.Data.CustomName?.Equals("") ?? false))
            {
                expanded.Data.CustomName = null;
            }

            expanded = null;
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
            ctxAscending = "TKUtils.StoreMenu.Ascending".Localize();
            ctxDescending = "TKUtils.StoreMenu.Descending".Localize();
            ctxInfo = "TKUtils.StoreMenu.Info".Localize();
            noCustomKarmaText = "TKUtils.TraitStore.NoCustomKarmaType".Localize();
            resetText = "TKUtils.Buttons.ResetAll".Localize();
            customNameText = "TKUtils.Inputs.CustomName".Localize();
            karmaTypeText = "TKUtils.IncidentEditor.Karma".Localize();
            stuffText = "TKUtils.ItemStore.Stuff".Localize();

            resetAllTextSize = Text.CalcSize(resetAllText);
            enableAllTextSize = Text.CalcSize(enableAllText);
            disableAllTextSize = Text.CalcSize(disableAllText);
        }

        private static IEnumerable<ThingItem> GenerateContainers()
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

        private void InjectCategoryFilter(string category)
        {
            StoreItemFilter filter = filters.FirstOrDefault(f => f.FilterType == FilterTypes.Category);

            if (filter == null)
            {
                filters.Add(
                    new StoreItemFilter
                    {
                        FilterType = FilterTypes.Category,
                        Filter = t => FilterByCategory(t, category),
                        Label = category
                    }
                );
                return;
            }

            filter.Label = category;
            filter.Filter = t => FilterByCategory(t, category);
        }

        private void InjectModFilter(string mod)
        {
            StoreItemFilter filter = filters.FirstOrDefault(f => f.FilterType == FilterTypes.Mod);

            if (filter == null)
            {
                filters.Add(
                    new StoreItemFilter {FilterType = FilterTypes.Mod, Filter = t => FilterByMod(t, mod), Label = mod}
                );
                return;
            }

            filter.Label = mod;
            filter.Filter = t => FilterByMod(t, mod);
        }

        private void InjectTechLevelFilter(TechLevel techLevel)
        {
            StoreItemFilter filter = filters.FirstOrDefault(f => f.FilterType == FilterTypes.TechLevel);

            if (filter == null)
            {
                filters.Add(
                    new StoreItemFilter
                    {
                        FilterType = FilterTypes.TechLevel,
                        Filter = t => FilterByTechLevel(t, techLevel),
                        Label = techLevel.ToString()
                    }
                );
                return;
            }

            filter.Label = techLevel.ToString();
            filter.Filter = t => FilterByTechLevel(t, techLevel);
        }

        private static List<ThingItem> FilterByCategory(IEnumerable<ThingItem> subject, string category)
        {
            return subject.Where(t => t.Category.Equals(category)).ToList();
        }

        private static List<ThingItem> FilterByMod(IEnumerable<ThingItem> subject, string mod)
        {
            return subject.Where(t => t.Mod.Equals(mod)).ToList();
        }

        private static List<ThingItem> FilterByTechLevel(IEnumerable<ThingItem> subject, TechLevel techLevel)
        {
            return subject.Where(t => t.Thing.techLevel == techLevel).ToList();
        }
    }

    internal class StoreItemFilter
    {
        private string label;
        private float labelWidth = -1;

        public FilterTypes FilterType { get; set; }

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

        public Func<List<ThingItem>, List<ThingItem>> Filter { get; set; }
    }
}

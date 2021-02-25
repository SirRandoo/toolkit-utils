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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit;
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
        private static IEnumerator<ThingItem> _validator;
        private static readonly Color OverlayBackgroundColor = new Color(0.13f, 0.16f, 0.17f);

        private readonly ThingItemFilterManager filterManager = new ThingItemFilterManager();
        private readonly ItemTableWorker worker;

        private bool categorySearch;
        private string categorySearchText;
        private Vector2 categorySearchTextSize;
        private string disableAllText;
        private Vector2 disableAllTextSize;
        private string enableAllText;
        private Vector2 enableAllTextSize;
        private bool filterMenuActive;
        private float lastSearchTick;
        private string localize;

        private string query = "";
        private string resetAllText;

        private Vector2 resetAllTextSize;
        private string searchText;
        private Vector2 searchTextSize;
        private bool shouldResizeTable = true;
        private string title;

        static StoreDialog()
        {
            _validator = ValidateContainers().GetEnumerator();
        }

        public StoreDialog()
        {
            doCloseX = true;
            optionalTitle = title;

            worker = new ItemTableWorker();
        }

        private bool FilterMenuActive
        {
            get => filterMenuActive;
            set
            {
                filterMenuActive = value;

                if (!filterMenuActive)
                {
                    NotifySearchRequested();
                }
            }
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);

        private void NotifySearchRequested()
        {
            lastSearchTick = 10f;
        }

        private void UpdateItemView()
        {
            foreach (ItemTableItem item in worker.Data)
            {
                item.IsHidden = false;
            }

            filterManager.FilterItems(worker.Data);

            if (query.NullOrEmpty())
            {
                return;
            }

            foreach (ItemTableItem item in worker.Data.Where(i => !i.IsHidden))
            {
                item.IsHidden = categorySearch
                    ? !item.Data.Category.ToLower().Contains(query.ToLower())
                    : !item.Data.Name.ToLower().Contains(query.ToLower());
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
            GenerateFilters();
            GetTranslationStrings();
            worker.Prepare();
        }

        private void GenerateFilters()
        {
            filterManager.RegisterFilter(
                FilterTypes.Research,
                new ThingItemFilter
                {
                    Id = "Researched",
                    IsUnfilteredFunc = ThingItemFilter.FilterByResearched,
                    Label = "TKUtils.StoreFilters.Researched".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Research,
                new ThingItemFilter
                {
                    Id = "NotResearched",
                    IsUnfilteredFunc = ThingItemFilter.FilterByNotResearched,
                    Label = "TKUtils.StoreFilters.NotResearched".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Stackable,
                new ThingItemFilter
                {
                    Id = "Stackable",
                    IsUnfilteredFunc = ThingItemFilter.FilterByStackable,
                    Label = "TKUtils.StoreFilters.Stackable".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.Stackable,
                new ThingItemFilter
                {
                    Id = "NonStackable",
                    IsUnfilteredFunc = ThingItemFilter.FilterByNonStackable,
                    Label = "TKUtils.StoreFilters.NonStackable".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.State,
                new ThingItemFilter
                {
                    Id = "Disabled",
                    IsUnfilteredFunc = ThingItemFilter.FilterByDisabled,
                    Label = "TKUtils.StoreFilters.Disabled".Localize().CapitalizeFirst()
                }
            );

            filterManager.RegisterFilter(
                FilterTypes.State,
                new ThingItemFilter
                {
                    Id = "Enabled",
                    IsUnfilteredFunc = ThingItemFilter.FilterByEnabled,
                    Label = "TKUtils.StoreFilters.Enabled".Localize().CapitalizeFirst()
                }
            );

            foreach (TechLevel techLevel in Data.TechLevels)
            {
                filterManager.RegisterFilter(
                    FilterTypes.TechLevel,
                    new ThingItemFilter
                    {
                        Id = $"TechLevel.{techLevel}",
                        IsUnfilteredFunc = t => ThingItemFilter.IsTechLevelRelevant(t, techLevel),
                        Label = techLevel.ToString().CapitalizeFirst()
                    }
                );
            }

            if (Data.Items.NullOrEmpty())
            {
                return;
            }

            foreach (string modName in Data.Items.OrderBy(i => i.Mod).Select(i => i.Mod).Distinct())
            {
                filterManager.RegisterFilter(
                    FilterTypes.Mod,
                    new ThingItemFilter
                    {
                        Id = $"Mod.{modName}",
                        IsUnfilteredFunc = t => ThingItemFilter.IsModRelevant(t, modName),
                        Label = modName
                    }
                );
            }

            foreach (string category in Data.Items.OrderBy(i => i.Category).Select(i => i.Category).Distinct())
            {
                filterManager.RegisterFilter(
                    FilterTypes.Category,
                    new ThingItemFilter
                    {
                        Id = $"Categories.{category}",
                        IsUnfilteredFunc = t => ThingItemFilter.IsCategoryRelevant(t, category),
                        Label = category
                    }
                );
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(inRect);
            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 3f,
                inRect.width,
                inRect.height - Text.LineHeight * 3f
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

            bool wrapped = Text.WordWrap;
            Text.WordWrap = false;
            GUI.EndGroup();

            GUI.BeginGroup(contentArea);

            if (shouldResizeTable)
            {
                worker.NotifyResolutionChanged(contentArea.AtZero());
                shouldResizeTable = true;
            }

            worker.Draw(contentArea.AtZero());
            GUI.EndGroup();

            Text.WordWrap = wrapped;
            Text.Font = fontCache;
            GUI.EndGroup();
        }

        private void DoFilterMenu(Rect canvas)
        {
            float filterWidth = canvas.width * 0.5f;
            Vector2 center = canvas.center;

            Rect filterDialog = new Rect(
                    center.x - filterWidth / 2f,
                    center.y - canvas.height * 0.75f / 2f,
                    filterWidth,
                    canvas.height * 0.75f
                ).ExpandedBy(StandardMargin * 2f)
               .Rounded();

            Widgets.DrawBoxSolid(filterDialog, OverlayBackgroundColor);
            Widgets.Label(
                new Rect(
                    filterDialog.x + 8f,
                    filterDialog.y + 5f,
                    filterDialog.width - 30f,
                    Text.LineHeight * LineScale
                ).Rounded(),
                localize
            );

            Widgets.DrawHighlight(
                new Rect(filterDialog.x, filterDialog.y, filterDialog.width, Text.LineHeight * LineScale).Rounded()
            );

            GUI.BeginGroup(filterDialog.ContractedBy(StandardMargin * 2f));
            filterManager.DrawFilters(
                new Rect(0f, 0f, filterDialog.width - StandardMargin * 4f, filterDialog.height - StandardMargin * 4f)
            );
            GUI.EndGroup();

            if (Widgets.CloseButtonFor(filterDialog))
            {
                FilterMenuActive = false;
                UpdateItemView();
            }
        }

        private void DrawStoreHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            var line = new Rect(canvas.x, canvas.y, canvas.width, Text.LineHeight);
            Rect searchRect = new Rect(line.x, line.y, line.width * 0.18f, line.height).Rounded();
            Rect searchTextRect = searchRect.WithWidth(searchTextSize.x);
            var searchFieldRect = new Rect(
                searchTextRect.x + searchTextRect.width + 5f,
                searchTextRect.y,
                searchRect.width - searchTextRect.width - 5f,
                searchTextRect.height
            );

            Widgets.Label(searchTextRect, searchText);

            if (SettingsHelper.DrawTextField(searchFieldRect, query, out string input))
            {
                query = input;
                NotifySearchRequested();
            }

            if (query.Length > 0 && SettingsHelper.DrawClearButton(searchRect))
            {
                query = "";
                NotifySearchRequested();
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

            DrawGlobalDisableButton(buttonRect, worker.Data);
            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalEnableButton(buttonRect, worker.Data);
            buttonRect = buttonRect.ShiftLeft(1f);
            DrawGlobalResetButton(buttonRect, worker.Data);

            GUI.EndGroup();
        }

        private void DrawCategorySearchModifier(Rect searchFieldRect, Rect line)
        {
            var categoryLine = new Rect(
                searchFieldRect.x,
                Text.LineHeight + 1f,
                categorySearchTextSize.x + 16f,
                line.height
            );
            var categoryCheck = new Rect(categoryLine.x + 2f, categoryLine.y + 2f, 12f, 12f);
            var categoryText = new Rect(
                categoryCheck.x + 16f,
                categoryLine.y,
                categorySearchTextSize.x,
                categoryLine.height
            );

            GUI.DrawTexture(categoryCheck, categorySearch ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            SettingsHelper.DrawLabel(categoryText, categorySearchText, TextAnchor.UpperLeft, GameFont.Tiny);

            if (!Widgets.ButtonInvisible(categoryLine))
            {
                return;
            }

            categorySearch = !categorySearch;

            if (!query.NullOrEmpty())
            {
                NotifySearchRequested();
            }
        }

        private void DrawGlobalDisableButton(Rect buttonRect, IEnumerable<ItemTableItem> workingList)
        {
            if (!Widgets.ButtonText(buttonRect, disableAllText))
            {
                return;
            }

            foreach (ItemTableItem item in workingList.Where(i => i.Data.Price > 0))
            {
                item.Data.IsEnabled = false;
                item.Data.Update();
            }
        }

        private void DrawFilterButton(Rect region)
        {
            if (Widgets.ButtonImage(region, Textures.Filter))
            {
                FilterMenuActive = true;
            }
        }

        private void DrawGlobalEnableButton(Rect buttonRect, IEnumerable<ItemTableItem> workingList)
        {
            if (!Widgets.ButtonText(buttonRect, enableAllText))
            {
                return;
            }

            foreach (ItemTableItem item in workingList.Where(i => i.Data.Price < 0))
            {
                item.Data.IsEnabled = true;
                item.Data.Update();
            }
        }

        private void DrawGlobalResetButton(Rect buttonRect, IEnumerable<ItemTableItem> workingList)
        {
            if (!Widgets.ButtonText(buttonRect, resetAllText))
            {
                return;
            }

            foreach (ItemTableItem item in workingList)
            {
                item.Data.Item.abr = item.Data.Thing.label.ToToolkit();
                item.Data.Item.price = item.Data.Thing.CalculateStorePrice();
                item.Data.Data.KarmaType = null;
                item.Data.Data.CustomName = null;
                item.Data.Data.HasQuantityLimit = false;
                item.Data.Data.Weight = 1f;
                item.Data.Data.IsStuffAllowed = true;
                item.Data.Data.QuantityLimit = 1;
            }
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (_validator != null)
            {
                try
                {
                    AdvanceValidator();
                }
                catch (Exception e)
                {
                    LogHelper.Warn(
                        $"Validator encountered an error! | Exception: {e.GetType().Name}({e.Message})\n{e.StackTrace}"
                    );
                    _validator = null;
                }
            }

            if (lastSearchTick <= 0)
            {
                UpdateItemView();
            }
            else
            {
                lastSearchTick -= Time.unscaledTime - lastSearchTick;
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
                worker.EnsureExists(new ItemTableItem {Data = latest});
            }

            worker.NotifySortRequested();
        }

        public override void Notify_ResolutionChanged()
        {
            base.Notify_ResolutionChanged();
            shouldResizeTable = true;
        }

        public override void PreClose()
        {
            foreach (ThingItem c in Data.Items.Where(c => c.Item == null && c.Thing != null))
            {
                c.Item = new Item(c.Thing.CalculateStorePrice(), c.Thing.label.ToToolkit(), c.Thing.defName);

                StoreInventory.items.Add(c.Item);
            }

            base.PreClose();
        }

        public override void PostClose()
        {
            Store_ItemEditor.UpdateStoreItemList();

            base.PostClose();
        }

        public static IEnumerable<ThingDef> GetTradeables()
        {
            return DefDatabase<ThingDef>.AllDefs.Where(t => t != null).Where(IsTradeable);
        }

        private static bool IsTradeable(ThingDef t)
        {
            if (!t.tradeability.TraderCanSell() && !ThingSetMakerUtility.CanGenerate(t))
            {
                return false;
            }

            if (t.building != null && !t.Minifiable && !ToolkitSettings.MinifiableBuildings)
            {
                return false;
            }

            if (t.FirstThingCategory == null && t.race == null)
            {
                return false;
            }

            try
            {
                if (!(t.BaseMarketValue > 0.0))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return t.defName != "Human";
        }

        public override void OnCancelKeyPressed()
        {
            if (FilterMenuActive)
            {
                FilterMenuActive = false;
                Event.current.Use();
                UpdateItemView();
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
                UpdateItemView();
                return;
            }

            base.OnAcceptKeyPressed();
        }

        private void GetTranslationStrings()
        {
            title = "TKUtils.ItemStore.Title".Localize();
            categorySearchText = "TKUtils.Fields.CategorySearch".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            resetAllText = "TKUtils.Buttons.ResetAll".Localize();
            enableAllText = "TKUtils.Buttons.EnableAll".Localize();
            disableAllText = "TKUtils.Buttons.DisableAll".Localize();
            localize = "TKUtils.Headers.FilterDialog".Localize();

            resetAllTextSize = Text.CalcSize(resetAllText);
            enableAllTextSize = Text.CalcSize(enableAllText);
            disableAllTextSize = Text.CalcSize(disableAllText);
            searchTextSize = Text.CalcSize(searchText);
            categorySearchTextSize = Text.CalcSize(categorySearchText);
        }

        internal static IEnumerable<ThingItem> ValidateContainers()
        {
            var builder = new StringBuilder();

            foreach (ThingDef thing in GetTradeables())
            {
                if (thing?.defName == null)
                {
                    continue;
                }

                ThingItem thingItem = Data.Items.Find(i => i.DefName.Equals(thing.defName))
                                      ?? new ThingItem {Thing = thing};

                try
                {
                    Item item = StoreInventory.items.Find(i => i?.defname?.Equals(thing.defName) ?? false);

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

                    thingItem.IsEnabled = item.price > 0;
                    thingItem.Item = item;
                    thingItem.Update();
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

            builder.Insert(0, "The following containers couldn't be validated:\n");
            LogHelper.Warn(builder.ToString());
        }
    }
}

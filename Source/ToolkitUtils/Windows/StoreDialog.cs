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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows;

/// <summary>
///     A dialog that allows users to configure their item store data.
/// </summary>
[StaticConstructorOnStartup]
public class StoreDialog : Window
{
    private const float LineScale = 1.25f;
    private static IEnumerator<ThingItem> _validator = ValidateContainers().GetEnumerator();
    private static readonly Color OverlayBackgroundColor = new Color(0.13f, 0.16f, 0.17f);

    private readonly ThingItemFilterManager _filterManager = new ThingItemFilterManager();
    private readonly ItemTableWorker _worker;

    private bool _categorySearch;
    private string _categorySearchText;
    private Vector2 _categorySearchTextSize;
    private bool _filterMenuActive;
    private float _lastSearchTick;
    private string _localize;

    private string _query = "";
    private string _resetAllText;

    private Vector2 _resetAllTextSize;
    private string _searchText;
    private Vector2 _searchTextSize;
    private bool _shouldResizeTable = true;
    private string _title;

    public StoreDialog()
    {
        doCloseX = true;
        _worker = new ItemTableWorker();
    }

    private bool FilterMenuActive
    {
        get => _filterMenuActive;
        set
        {
            _filterMenuActive = value;

            if (!_filterMenuActive)
            {
                NotifySearchRequested();
            }
        }
    }

    /// <inheritdoc cref="Window.InitialSize"/>
    public override Vector2 InitialSize => new Vector2(900f, Mathf.FloorToInt(UI.screenHeight * 0.9f));

    /// <inheritdoc cref="Window.Margin"/>
    protected override float Margin => 22f;

    private void NotifySearchRequested()
    {
        _lastSearchTick = 10f;
    }

    private void UpdateItemView()
    {
        foreach (TableSettingsItem<ThingItem> item in _worker.Data)
        {
            item.IsHidden = false;
        }

        _filterManager.FilterItems(_worker.Data);

        if (_query.NullOrEmpty())
        {
            return;
        }

        foreach (TableSettingsItem<ThingItem> item in _worker.Data.Where(i => !i.IsHidden))
        {
            item.IsHidden = _categorySearch ? !item.Data.Category.ToLower().Contains(_query.ToLower()) : !item.Data.Name.ToLower().Contains(_query.ToLower());
        }
    }

    /// <inheritdoc cref="Window.PreOpen"/>
    public override void PreOpen()
    {
        base.PreOpen();
        GenerateFilters();
        GetTranslationStrings();
        _worker.Prepare();
        optionalTitle = _title;
    }

    private void GenerateFilters()
    {
        _filterManager.RegisterFilter(
            FilterTypes.Research,
            new ThingItemFilter
            {
                Id = "Researched", IsUnfilteredFunc = ThingItemFilter.FilterByResearched, Label = "TKUtils.StoreFilters.Researched".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Research,
            new ThingItemFilter
            {
                Id = "NotResearched",
                IsUnfilteredFunc = ThingItemFilter.FilterByNotResearched,
                Label = "TKUtils.StoreFilters.NotResearched".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Manufacturable,
            new ThingItemFilter
            {
                Id = "Manufacturable",
                IsUnfilteredFunc = ThingItemFilter.FilterByManufactured,
                Label = "TKUtils.StoreFilters.Manufactured".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Manufacturable,
            new ThingItemFilter
            {
                Id = "NonManufacturable",
                IsUnfilteredFunc = ThingItemFilter.FilterByNonManufactured,
                Label = "TKUtils.StoreFilters.NonManufactured".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Stackable,
            new ThingItemFilter
            {
                Id = "Stackable", IsUnfilteredFunc = ThingItemFilter.FilterByStackable, Label = "TKUtils.StoreFilters.Stackable".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Stuff,
            new ThingItemFilter { Id = "Stuff", IsUnfilteredFunc = ThingItemFilter.FilterByStuff, Label = "TKUtils.StoreFilters.Stuff".Localize().CapitalizeFirst() }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Stuff,
            new ThingItemFilter { Id = "NotStuff", IsUnfilteredFunc = ThingItemFilter.FilterByNotStuff, Label = "TKUtils.StoreFilters.NotStuff".Localize() }
        );

        _filterManager.RegisterFilter(
            FilterTypes.Stackable,
            new ThingItemFilter
            {
                Id = "NonStackable",
                IsUnfilteredFunc = ThingItemFilter.FilterByNonStackable,
                Label = "TKUtils.StoreFilters.NonStackable".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.State,
            new ThingItemFilter
            {
                Id = "Disabled", IsUnfilteredFunc = ThingItemFilter.FilterByDisabled, Label = "TKUtils.StoreFilters.Disabled".Localize().CapitalizeFirst()
            }
        );

        _filterManager.RegisterFilter(
            FilterTypes.State,
            new ThingItemFilter
            {
                Id = "Enabled", IsUnfilteredFunc = ThingItemFilter.FilterByEnabled, Label = "TKUtils.StoreFilters.Enabled".Localize().CapitalizeFirst()
            }
        );

        foreach (TechLevel techLevel in Data.TechLevels)
        {
            _filterManager.RegisterFilter(
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
            _filterManager.RegisterFilter(
                FilterTypes.Mod,
                new ThingItemFilter { Id = $"Mod.{modName}", IsUnfilteredFunc = t => ThingItemFilter.IsModRelevant(t, modName), Label = modName }
            );
        }

        foreach (string category in Data.Items.OrderBy(i => i.Category).Select(i => i.Category).Distinct())
        {
            _filterManager.RegisterFilter(
                FilterTypes.Category,
                new ThingItemFilter { Id = $"Categories.{category}", IsUnfilteredFunc = t => ThingItemFilter.IsCategoryRelevant(t, category), Label = category }
            );
        }
    }

    /// <inheritdoc cref="Window.DoWindowContents"/>
    public override void DoWindowContents(Rect inRect)
    {
        if (Event.current.type == EventType.Layout)
        {
            return;
        }

        GUI.BeginGroup(inRect);
        var headerRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
        var contentArea = new Rect(inRect.x, Text.LineHeight * 3f, inRect.width, inRect.height - Text.LineHeight * 3f);
        GameFont fontCache = Text.Font;
        Text.Font = GameFont.Small;

        DrawStoreHeader(headerRect);
        Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

        if (FilterMenuActive)
        {
            DoFilterMenu(contentArea);
            GUI.EndGroup();

            return;
        }

        bool wrapped = Text.WordWrap;
        Text.WordWrap = false;

        GUI.BeginGroup(contentArea);

        if (_shouldResizeTable)
        {
            _worker.NotifyResolutionChanged(contentArea.AtZero());
            _shouldResizeTable = false;
        }

        _worker.Draw(contentArea.AtZero());
        GUI.EndGroup();

        Text.WordWrap = wrapped;
        Text.Font = fontCache;
        GUI.EndGroup();
    }

    private void DoFilterMenu(Rect canvas)
    {
        float filterWidth = canvas.width * 0.5f;
        Vector2 center = canvas.center;

        Rect filterDialog = new Rect(center.x - filterWidth / 2f, center.y - canvas.height * 0.75f / 2f, filterWidth, canvas.height * 0.75f)
           .ExpandedBy(StandardMargin * 2f)
           .Rounded();

        Widgets.DrawBoxSolid(filterDialog, OverlayBackgroundColor);
        Widgets.Label(new Rect(filterDialog.x + 8f, filterDialog.y + 5f, filterDialog.width - 30f, Text.LineHeight * LineScale).Rounded(), _localize);

        Widgets.DrawHighlight(new Rect(filterDialog.x, filterDialog.y, filterDialog.width, Text.LineHeight * LineScale).Rounded());

        GUI.BeginGroup(filterDialog.ContractedBy(StandardMargin * 2f));
        _filterManager.DrawFilters(new Rect(0f, 0f, filterDialog.width - StandardMargin * 4f, filterDialog.height - StandardMargin * 4f));
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

        Rect searchTextRect = searchRect.Trim(Direction8Way.East, searchRect.width - _searchTextSize.x);

        var searchFieldRect = new Rect(
            searchTextRect.x + searchTextRect.width + 5f,
            searchTextRect.y,
            searchRect.width - searchTextRect.width - 5f,
            searchTextRect.height
        );

        Widgets.Label(searchTextRect, _searchText);

        if (UiHelper.TextField(searchFieldRect, _query, out string input))
        {
            _query = input;
            NotifySearchRequested();
        }

        if (_query.Length > 0 && UiHelper.ClearButton(searchRect))
        {
            _query = "";
            NotifySearchRequested();
        }

        var filterButtonRect = new Rect(searchRect.x + searchRect.width + 2f, searchRect.y, searchRect.height, searchRect.height);
        DrawFilterButton(filterButtonRect);
        DrawCategorySearchModifier(searchFieldRect, line);

        float buttonWidth = _resetAllTextSize.x + 16f;
        var buttonRect = new Rect(line.x + line.width - buttonWidth, line.y, buttonWidth, line.height);

        DrawGlobalResetButton(buttonRect);

        GUI.EndGroup();
    }

    private void DrawCategorySearchModifier(Rect searchFieldRect, Rect line)
    {
        var categoryLine = new Rect(searchFieldRect.x, Text.LineHeight + 1f, _categorySearchTextSize.x + 16f, line.height);
        var categoryCheck = new Rect(categoryLine.x + 2f, categoryLine.y + 2f, 12f, 12f);
        var categoryText = new Rect(categoryCheck.x + 16f, categoryLine.y, _categorySearchTextSize.x, categoryLine.height);

        GUI.DrawTexture(categoryCheck, _categorySearch ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
        UiHelper.Label(categoryText, _categorySearchText, TextAnchor.UpperLeft, GameFont.Tiny);

        if (!Widgets.ButtonInvisible(categoryLine))
        {
            return;
        }

        _categorySearch = !_categorySearch;

        if (!_query.NullOrEmpty())
        {
            NotifySearchRequested();
        }
    }

    private void DrawFilterButton(Rect region)
    {
        if (Widgets.ButtonImage(region, Textures.Filter))
        {
            FilterMenuActive = true;
        }
    }

    private void DrawGlobalResetButton(Rect canvas)
    {
        if (Widgets.ButtonText(canvas, _resetAllText))
        {
            ConfirmationDialog.Open("TKUtils.ItemStore.ConfirmReset".TranslateSimple(), PerformGlobalReset);
        }
    }

    private void PerformGlobalReset()
    {
        foreach (TableSettingsItem<ThingItem> item in _worker.Data.Where(i => !i.IsHidden))
        {
            item.Data.Item!.abr = item.Data.Thing.label.ToToolkit();
            item.Data.Item.price = item.Data.Thing.CalculateStorePrice();
            item.Data.Data!.KarmaType = null;
            item.Data.ItemData!.CustomName = null;
            item.Data.ItemData.HasQuantityLimit = false;
            item.Data.ItemData.Weight = 1f;
            item.Data.ItemData.IsStuffAllowed = true;
            item.Data.ItemData.QuantityLimit = 1;
        }
    }

    /// <inheritdoc cref="Window.WindowUpdate"/>
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
                TkUtils.Logger.Warn($"Validator encountered an error! | Exception: {e.GetType().Name}({e.Message})\n{e.StackTrace}");
                _validator = null;
            }
        }

        if (_lastSearchTick <= 0)
        {
            UpdateItemView();
        }
        else
        {
            _lastSearchTick -= Time.unscaledTime - _lastSearchTick;
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
            _worker.EnsureExists(new TableSettingsItem<ThingItem> { Data = latest });
        }

        _worker.NotifySortRequested();
    }

    /// <inheritdoc cref="Window.Notify_ResolutionChanged"/>
    public override void Notify_ResolutionChanged()
    {
        base.Notify_ResolutionChanged();
        _shouldResizeTable = true;
    }

    /// <inheritdoc cref="Window.PreClose"/>
    public override void PreClose()
    {
        foreach (ThingItem c in Data.Items.Where(c => c.Item == null && c.Thing != null))
        {
            c.Item = new Item(c.Thing.CalculateStorePrice(), c.Thing.label.ToToolkit(), c.Thing.defName);

            StoreInventory.items.Add(c.Item);
        }

        base.PreClose();
    }

    /// <inheritdoc cref="Window.PostClose"/>
    public override void PostClose()
    {
        Store_ItemEditor.UpdateStoreItemList();

        base.PostClose();
    }

    /// <summary>
    ///     Returns a set of <see cref="ThingDef"/>s traders may have in
    ///     their inventory.
    /// </summary>
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
            if (t.BaseMarketValue <= 0.0)
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

    /// <inheritdoc cref="Window.OnCancelKeyPressed"/>
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

    /// <inheritdoc cref="Window.OnAcceptKeyPressed"/>
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
        _title = "TKUtils.ItemStore.Title".TranslateSimple();
        _categorySearchText = "TKUtils.Fields.CategorySearch".TranslateSimple();
        _searchText = "TKUtils.Buttons.Search".TranslateSimple();
        _resetAllText = "TKUtils.Buttons.ResetAll".TranslateSimple();
        _localize = "TKUtils.Headers.FilterDialog".TranslateSimple();

        _resetAllTextSize = Text.CalcSize(_resetAllText);
        _searchTextSize = Text.CalcSize(_searchText);
        _categorySearchTextSize = Text.CalcSize(_categorySearchText);
    }

    /// <summary>
    ///     Polyfills the <see cref="ThingItem"/> containers to ensure they
    ///     encompass all of Twitch Toolkit's <see cref="Item"/> dataclasses,
    ///     as well as any <see cref="ThingDef"/>s that may not have been
    ///     caught by Twitch Toolkit, i.e. mods that dynamically create items
    ///     at runtime.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ThingItem> ValidateContainers()
    {
        var builder = new StringBuilder();

        foreach (ThingDef thing in GetTradeables())
        {
            if (thing?.defName == null)
            {
                continue;
            }

            ThingItem thingItem = Data.Items.Find(i => i.DefName!.Equals(thing.defName)) ?? new ThingItem { Thing = thing };

            try
            {
                Item item = StoreInventory.items.Find(i => i?.defname?.Equals(thing.defName) ?? false);

                if (item == null)
                {
                    item = new Item(thing.CalculateStorePrice(), thing.label?.ToToolkit() ?? thing.defName, thing.defName);

                    StoreInventory.items.Add(item);
                }
                else
                {
                    item.abr ??= thing.label?.ToToolkit() ?? thing.defName;
                }

                thingItem.Enabled = item.price > 0;
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
        TkUtils.Logger.Warn(builder.ToString());
    }
}

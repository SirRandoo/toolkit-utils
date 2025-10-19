// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation.Extensions;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>A serialized class for drawing tabbed content on screen.</summary>
/// <remarks>
///     Developers wanting tabbed content should use the <see cref="TabularDrawer.Builder" /> class to instantiate a
///     new tabular drawer.
/// </remarks>
[PublicAPI]
public class TabularDrawer
{
    /// <summary>A delegate for rendering content within a specified screen region.</summary>
    /// <param name="region">Defines the rectangular area where the content should be drawn.</param>
    public delegate void ContentDrawer(Rect region);

    private const int IconSize = 16;
    private const int IconTextPadding = 5;
    private static readonly Color BackgroundColor = new(r: 0.46f, g: 0.49f, b: 0.5f);
    private readonly Dictionary<Tab, Vector2> _labelCache = [];
    private int _currentPage = 1;
    private Tab? _currentTab;
    private float _foregroundProgress;
    private float _highlightProgress;
    private Tab? _lastTab;
    private float _maxHeight;
    private float _maxWidth;
    private Rect _previousRegion = Rect.zero;

    private IReadOnlyList<Tab> _tabs = [];
    private int _tabsPerView;
    private int _totalPages;

    private TabularDrawer() {}

    /// <summary>Returns the id of the current tab, or null if there isn't an active tab.</summary>
    public string? CurrentTabId => _currentTab?.Id;

    /// <summary>Draws the tabular content on screen.</summary>
    /// <param name="region">The region of the screen to draw the tabular content in.</param>
    public void Draw(Rect region)
    {
        if (_previousRegion != region)
        {
            _previousRegion = region;
            RecalculateLayout();
        }

        var barRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.TabHeight);
        var contentRegion = new Rect(x: 0f, barRegion.height, region.width, region.height - barRegion.height);

        GUI.BeginGroup(region);

        GUI.color = BackgroundColor;
        Widgets.DrawLightHighlight(RectExtensions.AtZero(ref region));
        GUI.color = Color.white;

        GUI.BeginGroup(barRegion);
        DrawTabs(barRegion);
        GUI.EndGroup();

        if (_currentTab != null) Widgets.DrawLightHighlight(contentRegion);

        RectExtensions.ContractedBy(ref contentRegion, margin: 16f);

        GUI.BeginGroup(contentRegion);

        if (_currentTab == null)
        {
            GUI.EndGroup();

            return;
        }

        _currentTab.ContentRenderAction?.Invoke(RectExtensions.AtZero(ref contentRegion));
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawTabs(Rect region)
    {
        var tabBarRegion = new Rect(x: 0f, y: 0f, region.width, region.height);

        if (_totalPages > 1)
        {
            var previousPageRegion = new Rect(x: 0f, y: 0f, region.height, region.height);
            var nextPageRegion = new Rect(region.width - region.height, y: 0f, region.height, region.height);

            DrawHorizontalNavigation(previousPageRegion, nextPageRegion);

            tabBarRegion.SetX(previousPageRegion.width);
            tabBarRegion.SetWidth(tabBarRegion.width - (previousPageRegion.width + nextPageRegion.width));
        }

        GUI.BeginGroup(tabBarRegion);

        int pageIndex = (_currentPage - 1) * _tabsPerView;
        int viewCount = Mathf.Min(_tabs.Count, _tabsPerView);
        float width = Mathf.FloorToInt(tabBarRegion.width / _tabsPerView);

        for (var i = 0; i < viewCount; i++)
        {
            int tabIndex = pageIndex + i;

            if (tabIndex >= _tabs.Count) break;

            Tab tab = _tabs[tabIndex];

            var tabRegion = new Rect(i * width, tabBarRegion.y, width, region.height);

            Widgets.DrawHighlightIfMouseover(tabRegion);
            TooltipHandler.TipRegion(tabRegion, tab.Tooltip);

            if (tab == _currentTab) DrawTabHighlight(tabRegion);

            if (Widgets.ButtonInvisible(tabRegion)) _currentTab = tab;

            GUI.BeginGroup(RectExtensions.ContractedBy(ref tabRegion, margin: 5f));
            DrawTabContent(RectExtensions.AtZero(ref tabRegion), tab);
            GUI.EndGroup();
        }

        GUI.EndGroup();
    }

    private void DrawHorizontalNavigation(Rect leftRegion, Rect rightRegion)
    {
        Vector2 leftRegionCenter = leftRegion.center;
        Vector2 rightRegionCenter = rightRegion.center;

        const float halvedIconSize = IconSize * 0.5f;

        var leftIconRegion = new Rect(leftRegionCenter.x - halvedIconSize, leftRegionCenter.y - halvedIconSize, IconSize, IconSize);

        var rightIconRegion = new Rect(rightRegionCenter.x - halvedIconSize, rightRegionCenter.y - halvedIconSize, IconSize, IconSize);

        IconDrawer.DrawIcon(leftIconRegion, _currentPage == 1 ? Icons.AnglesLeft.Value : Icons.AngleLeft.Value, Color.white);

        IconDrawer.DrawIcon(rightIconRegion, _currentPage == _totalPages ? Icons.AnglesRight.Value : Icons.AngleRight.Value, Color.white);

        if (Widgets.ButtonInvisible(leftRegion)) _currentPage = _currentPage <= 1 ? _totalPages : _currentPage - 1;

        if (Widgets.ButtonInvisible(rightRegion)) _currentPage = _currentPage >= _totalPages ? 1 : _currentPage + 1;

        Widgets.DrawHighlightIfMouseover(leftRegion);
        Widgets.DrawHighlightIfMouseover(rightRegion);
    }

    private void DrawTabHighlight(Rect region)
    {
        if (_lastTab != _currentTab)
        {
            _highlightProgress = 0f;
            _foregroundProgress = 0f;
            _lastTab = _currentTab;
        }

        Color cache = GUI.color;

        GUI.color = Color.cyan;
        Vector2 center = region.center;
        float accentDistance = Mathf.FloorToInt((region.x - center.x) * 0.75f);
        _highlightProgress = Mathf.SmoothStep(_highlightProgress, accentDistance, t: 0.15f);

        Widgets.DrawLineHorizontal(center.x - _highlightProgress, region.y + region.height - 1f, _highlightProgress * 2f);

        GUI.color = cache;

        _foregroundProgress = Mathf.SmoothStep(_foregroundProgress, center.x - region.x, t: 0.15f);
        var animationRegion = new Rect(center.x, center.y, width: 0f, height: 0f);

        Widgets.DrawLightHighlight(RectExtensions.ExpandedBy(ref animationRegion, _foregroundProgress));
    }

    private void DrawTabContent(Rect region, Tab tab)
    {
        Rect iconRegion;

        switch (tab.Layout)
        {
            case IconLayout.IconAndText:
                Vector2 center = region.center;
                Vector2 tabSize = _labelCache[tab];

                float contentWidth = tabSize.x - 10f;
                float halvedContentWidth = contentWidth * 0.5f;
                iconRegion = new Rect(center.x - halvedContentWidth, center.y - IconSize * 0.5f, IconSize, IconSize);

                var textRegion = new Rect(
                    iconRegion.x + IconSize + IconTextPadding,
                    center.y - UiConstants.LineHeight * 0.5f,
                    contentWidth - IconSize - IconTextPadding,
                    UiConstants.LineHeight
                );


                IconDrawer.DrawIcon(iconRegion, tab.Icon!, Color.white);
                LabelDrawer.DrawLabel(textRegion, tab.Label!);

                return;
            case IconLayout.Text:
                LabelDrawer.DrawLabel(region, tab.Label!, TextAnchor.MiddleCenter);

                return;
            case IconLayout.Icon:
                iconRegion = LayoutHelper.IconRect(x: 0f, y: 0f, IconSize, region.height);
                IconDrawer.DrawIcon(iconRegion, tab.Icon!, Color.white);

                return;
            default:
                LabelDrawer.DrawLabel(region, tab.Label!, TextAnchor.MiddleCenter);

                return;
        }
    }

    private void RecalculateLayout()
    {
        GameFont cache = Text.Font;
        Text.Font = GameFont.Small;

        for (var index = 0; index < _tabs.Count; index++)
        {
            Tab tab = _tabs[index];

            if (!_labelCache.TryGetValue(tab, out Vector2 vector))
            {
                Vector2 size = !string.IsNullOrEmpty(tab.Label) ? Text.CalcSize(tab.Label) : new Vector2();

                if (tab.Icon != null) size.IncrementX(IconSize + IconTextPadding);

                size.IncrementX(10f); // Padding

                _labelCache[tab] = vector = size;
            }

            if (vector.x > _maxWidth) _maxWidth = vector.x;

            if (vector.y > _maxHeight) _maxHeight = vector.y;
        }

        Text.Font = cache;

        int tabBarWidth = Mathf.CeilToInt(_previousRegion.width - UiConstants.TabHeight * 2f);
        _tabsPerView = Mathf.FloorToInt(tabBarWidth / _maxWidth);
        _totalPages = Mathf.CeilToInt(_tabs.Count / (float)_tabsPerView);
    }

    /// <summary>Represents a specialized class for building tabbed content.</summary>
    [PublicAPI]
    public sealed class Builder
    {
        /// <summary>Represents a delegate used to configure the options of a tab.</summary>
        /// <param name="options">An instance of <see cref="TabularDrawer.TabOptions" /> used to define tab customization.</param>
        public delegate void ConfigurationAction(TabOptions options);

        private readonly List<Tab> _tabs = [];

        /// <summary>Adds a tab to the tabular drawer.</summary>
        /// <param name="id">The unique id of the tab.</param>
        /// <param name="configurator">
        ///     A callable that takes an instance of <see cref="TabOptions" /> that's used to customize the
        ///     tab being added. Each tab must have an icon and/or a label.
        /// </param>
        /// <returns>The builder itself.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The tab's layout requires an icon or label be set within the
        ///     <see cref="TabOptions" /> instance.
        /// </exception>
        public Builder WithTab(string id, ConfigurationAction configurator)
        {
            var options = new TabOptions();

            configurator(options);

            bool isLabelInvalid = string.IsNullOrEmpty(options.Label);

            if (isLabelInvalid && options.Icon == null) throw new InvalidOperationException("An icon or label must be when adding a new tab.");
            if (isLabelInvalid && options.Layout == IconLayout.Text) throw new InvalidOperationException("Tab layout requires a label to be specified.");
            if (options.Icon == null && options.Layout == IconLayout.Icon) throw new InvalidOperationException("Tab layout requires an icon to be specified.");

            _tabs.Add(new Tab(id, options.Label, options.Drawer, options.Tooltip, options.Icon, options.Layout));

            return this;
        }

        /// <summary>Returns an instance of <see cref="TabularDrawer" /> prepped with each tab previously specified.</summary>
        public TabularDrawer Build()
        {
            var drawer = new TabularDrawer
            {
                _tabs = _tabs.AsReadOnly(),
            };

            if (_tabs.Count > 0) drawer._currentTab = _tabs[0];

            return drawer;
        }
    }

    /// <summary>Represents the various options each tab can have.</summary>
    [PublicAPI]
    public sealed class TabOptions
    {
        /// <summary>A string that's displayed to the user to indicate the type of content they can expect within the tab.</summary>
        public string? Label { get; set; }

        /// <summary>A string that's displayed when a user hovers over the tab with their mouse.</summary>
        public string? Tooltip { get; set; }

        /// <summary>
        ///     A callable that takes a <see cref="Rect" /> as its sole parameter. This callable is invoked while its
        ///     associated tab is active, with the <see cref="Rect" /> passed to it being the region developers should draw the
        ///     tab's content in.
        /// </summary>
        public ContentDrawer? Drawer { get; set; }

        /// <summary>The icon to display in the tab.</summary>
        public Texture2D? Icon { get; set; }

        /// <summary>Whether the display only the tab's icon, the tab's text, or both.</summary>
        public IconLayout Layout { get; set; } = IconLayout.Text;
    }

    private sealed record Tab(
        string Id,
        string? Label = null,
        ContentDrawer? ContentRenderAction = null,
        string? Tooltip = null,
        Texture2D? Icon = null,
        IconLayout Layout = IconLayout.IconAndText
    );
}

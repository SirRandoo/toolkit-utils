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
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation.Extensions;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>
///     A generic base class used to create a table view with customizable content filtering, resolution handling, and
///     drawing.
/// </summary>
/// <typeparam name="T">
///     The type of data each row in the table will represent. Must implement the
///     <see cref="IIdentifiable" /> interface.
/// </typeparam>
[PublicAPI]
public abstract partial class TableDrawer<T> where T : IIdentifiable
{
    private Rect[] _columnRegions = [];
    private IReadOnlyList<TableColumn> _columns = [];

    private List<TableEntry> _contents = [];
    private Rect _previousRegion = Rect.zero;
    private IReadOnlyList<Rect> _readOnlyColumnRegions = [];
    private Vector2 _scrollPos = Vector2.zero;
    private float _viewportHeight;

    /// <summary>Called once when the table's contents need to be filtered by the query specified.</summary>
    /// <param name="query">The query to filter the table's contents by.</param>
    public void NotifySearchRequested(string query)
    {
        _viewportHeight = 0f;
        bool queryGiven = !string.IsNullOrEmpty(query);

        for (var i = 0; i < _contents.Count; i++)
        {
            TableEntry row = _contents[i];

            row.Visible = !queryGiven || row.Data.Name.Contains(query);

            if (row.Visible) _viewportHeight += UiConstants.LineHeight;
        }
    }

    /// <summary>Called once when the table's contents need to be filtered by a set of filters.</summary>
    /// <param name="filters">A collection of filters to filter the table's contents by.</param>
    public void NotifyFilterRequested(Func<T, bool>[] filters)
    {
        for (var i = 0; i < _contents.Count; i++)
        {
            TableEntry row = _contents[i];

            if (!row.Visible) continue;

            for (var i1 = 0; i1 < filters.Length; i1++)
            {
                row.Visible = row.Visible && filters[i1](row.Data);

                if (!row.Visible) break;
            }
        }
    }

    /// <summary>Called once when the table's draw region has been resized.</summary>
    /// <param name="region">The new region of the table.</param>
    public void NotifyResolutionChanged(Rect region)
    {
        _previousRegion = region;

        var xPosition = 0f;
        float adjustedWidth = region.width - 16f;

        for (var i = 0; i < _columns.Count; i++)
        {
            var columnRegion = new Rect(xPosition, y: 0f, adjustedWidth * _columns[i].RelativeWidth, UiConstants.LineHeight);

            _columnRegions[i] = columnRegion;
            xPosition += columnRegion.width;
        }

        _readOnlyColumnRegions = new ReadOnlyCollection<Rect>(_columnRegions);
    }

    /// <summary>Draws the table as-is in the region given.</summary>
    /// <param name="region">The region to draw the table in.</param>
    public void Draw(Rect region)
    {
        if (region != _previousRegion)
        {
            NotifyResolutionChanged(region);

            _previousRegion = region;
        }

        var headerRegion = new Rect(x: 0f, y: 0f, region.width, UiConstants.LineHeight);
        var contentRegion = new Rect(x: 0f, headerRegion.height, region.width, region.height - headerRegion.height);

        GUI.BeginGroup(region);

        GUI.BeginGroup(headerRegion);
        DrawHeaders(headerRegion);
        GUI.EndGroup();

        GUI.BeginGroup(contentRegion);
        DrawContent(RectExtensions.AtZero(ref contentRegion));
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawHeaders(Rect region)
    {
        GUI.BeginGroup(region);

        for (var i = 0; i < _columns.Count; i++)
        {
            var offset = 0f;
            TableColumn column = _columns[i];
            Rect headerRegion = _readOnlyColumnRegions[i];
            Rect headerDisplayRegion = RectExtensions.ContractedBy(ref headerRegion, margin: 4f);

            Widgets.DrawLightHighlight(headerRegion);
            Widgets.DrawLightHighlight(headerRegion);

            if (i % 2 == 1)
            {
                GUI.color = new Color(Color.grey.r, Color.grey.g, Color.grey.b, a: 0.5f);
                Widgets.DrawLineVertical(headerRegion.x, headerRegion.y + 2f, headerRegion.height - 4f);
                Widgets.DrawLineVertical(headerRegion.x + headerRegion.width, headerRegion.y + 2f, headerRegion.height - 4f);

                GUI.color = Color.white;
            }

            if (column.Icon != null)
            {
                offset = region.height;

                Rect iconRegion = LayoutHelper.IconRect(headerDisplayRegion.x, headerDisplayRegion.y, region.height, region.height);

                GUI.DrawTexture(iconRegion, column.Icon);
            }

            if (!string.IsNullOrEmpty(column.Header))
            {
                var labelRegion = new Rect(headerDisplayRegion.x + offset, headerDisplayRegion.y, headerDisplayRegion.width - offset, region.height);

                LabelDrawer.DrawLabel(labelRegion, column.Header!);
            }

            if (column.SortAction == null) continue;

            Texture2D sortIcon = column.SortOrder is SortOrder.Descending ? Icons.SortUp.Value : Icons.SortDown.Value;

            Rect sortRegion = LayoutHelper.IconRect(
                headerDisplayRegion.x + headerDisplayRegion.width - headerDisplayRegion.height,
                headerDisplayRegion.y,
                headerDisplayRegion.height,
                headerDisplayRegion.height
            );

            GUI.DrawTexture(sortRegion, sortIcon);

            if (!Widgets.ButtonInvisible(headerRegion)) continue;

            column.SortOrder = column.SortOrder.Invert();
            column.SortAction(column.SortOrder, _contents);
        }

        GUI.EndGroup();
    }

    private void DrawContent(Rect region)
    {
        var viewport = new Rect(x: 0f, y: 0f, region.width - 16f, _viewportHeight);

        GUI.BeginGroup(region);

        _scrollPos = GUI.BeginScrollView(region, _scrollPos, viewport, alwaysShowHorizontal: false, alwaysShowVertical: true);

        var visibleRowCount = 0;

        for (var i = 0; i < _contents.Count; i++)
        {
            TableEntry rowEntry = _contents[i];

            if (!rowEntry.Visible) continue;

            var rowRegion = new Rect(x: 0f, visibleRowCount++ * UiConstants.LineHeight, region.width, UiConstants.LineHeight);

            if (!rowRegion.IsVisible(region, _scrollPos)) continue;
            if (visibleRowCount % 2 == 1) Widgets.DrawLightHighlight(rowRegion);

            DrawRowEntry(rowRegion, rowEntry.Data, _readOnlyColumnRegions);
        }

        GUI.EndScrollView();
        GUI.EndGroup();
    }

    /// <summary>Called once for every table row that's visible on screen.</summary>
    /// <param name="region">The region the row is being drawn in.</param>
    /// <param name="data">The data being drawn at the given row.</param>
    /// <param name="columnRegions">A collection of regions implementations can directly use to draw data within the row.</param>
    protected abstract void DrawRowEntry(Rect region, T data, IReadOnlyList<Rect> columnRegions);
}

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
using ToolkitUtils.Mod.Presentation.Drawers;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Dialogs;

// TODO: Currently "DropdownDialog" is a modified version of DropdownDrawer's DropdownDialog internal class.

public abstract class DropdownDialog<T> : Window
{
    private const int TotalDisplayableOptions = 9;
    private readonly Action<T> _setter;

    private readonly int _totalOptions;
    protected readonly T CurrentOption;
    protected readonly Rect DropdownRegion;
    private Vector2 _scrollPos = new(x: 0f, y: 0f);
    protected bool IsReversed;
    protected float ViewHeight;

    protected DropdownDialog(Rect parentRegion, T current, IReadOnlyList<T> allOptions, Action<T> setter)
    {
        _setter = setter;
        Options = allOptions;
        CurrentOption = current;
        DropdownRegion = parentRegion;
        _totalOptions = allOptions.Count;

        doCloseX = false;
        drawShadow = false;
        doCloseButton = false;
        layer = WindowLayer.Dialog;

        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
        forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
    }

    /// <inheritdoc />
    protected override float Margin => 5f;

    /// <summary>Represents the collection of selectable options in the dropdown dialog.</summary>
    public IReadOnlyList<T> Options { get; set; }

    /// <inheritdoc />
    protected override void SetInitialSizeAndPosition()
    {
        float yPosition = IsReversed ? DropdownRegion.y - ViewHeight - 10f : DropdownRegion.y + DropdownRegion.height + 5f;

        windowRect = new Rect(DropdownRegion.x, yPosition, DropdownRegion.width, ViewHeight);
        windowRect = windowRect.ExpandedBy(5f);
    }

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var dropdownOptionsRegion = new Rect(x: 0f, y: 0f, inRect.width, inRect.height);

        GUI.BeginGroup(inRect);

        Widgets.DrawLightHighlight(dropdownOptionsRegion);
        Widgets.DrawLightHighlight(dropdownOptionsRegion);
        Widgets.DrawLightHighlight(dropdownOptionsRegion);

        GUI.BeginGroup(dropdownOptionsRegion);
        DrawDropdownOptions(dropdownOptionsRegion.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    /// <summary>Renders the options within the dropdown UI.</summary>
    /// <param name="region">The screen region where the options will be drawn.</param>
    protected void DrawDropdownOptions(Rect region)
    {
        float viewportWidth = region.width - (_totalOptions > TotalDisplayableOptions ? 16f : 0f);
        var viewport = new Rect(x: 0f, y: 0f, viewportWidth, UiConstants.LineHeight * _totalOptions);

        GUI.BeginGroup(region);
        _scrollPos = GUI.BeginScrollView(region.AtZero(), _scrollPos, viewport);

        for (var i = 0; i < _totalOptions; i++)
        {
            var lineRegion = new Rect(x: 0f, UiConstants.LineHeight * i, viewportWidth, UiConstants.LineHeight);

            if (!lineRegion.IsVisible(viewport, _scrollPos)) continue;

            T item = Options[i];
            var textRegion = new Rect(x: 5f, lineRegion.y, lineRegion.width - 10f, lineRegion.height);

            DrawItemLabel(textRegion, item);

            Widgets.DrawHighlightIfMouseover(lineRegion);

            if (AreItemsEqual(item, CurrentOption)) Widgets.DrawHighlightSelected(lineRegion);

            if (i % 2 == 0)
            {
                GUI.color = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 0.44f);

                Widgets.DrawLineHorizontal(lineRegion.x, lineRegion.y, lineRegion.width);
                Widgets.DrawLineHorizontal(lineRegion.x, lineRegion.y + lineRegion.height, lineRegion.width);

                GUI.color = Color.white;
            }

            if (Widgets.ButtonInvisible(lineRegion, doMouseoverSound: false))
            {
                _setter(item);

                Close();
            }
        }

        GUI.EndScrollView();
        GUI.EndGroup();
    }

    /// <inheritdoc />
    public override void PreOpen()
    {
        ViewHeight = UiConstants.LineHeight * (_totalOptions > TotalDisplayableOptions ? TotalDisplayableOptions : _totalOptions);

        IsReversed = DropdownRegion.y + ViewHeight > UI.screenHeight;

        base.PreOpen();
    }

    /// <inheritdoc />
    public override void PostOpen()
    {
        if (Options.Count <= 0)
        {
            Log.Error("[SirRandoo.UX] Attempted to create a dropdown menu with no options.");

            Close(false);

            return;
        }

        if (CurrentOption != null) ScrollToItem(CurrentOption);
    }

    private void ScrollToItem(T item)
    {
        float totalViewHeight = _totalOptions * UiConstants.LineHeight;

        for (var i = 0; i < _totalOptions; i++)
        {
            if (!AreItemsEqual(Options[i], item)) continue;

            int startingPageItem = i - Mathf.FloorToInt(TotalDisplayableOptions / 2f) - 1;

            _scrollPos = new Vector2(x: 0f, Mathf.Clamp(startingPageItem * UiConstants.LineHeight, min: 0f, totalViewHeight));
        }
    }

    /// <summary>Retrieves the label for a given dropdown item.</summary>
    /// <param name="item">The item for which to retrieve the label.</param>
    /// <returns>A string representing the label of the specified item.</returns>
    protected abstract string GetItemLabel(T item);

    /// <summary>Renders the label for a specified item within a given region.</summary>
    /// <param name="region">The rectangular area where the label will be drawn.</param>
    /// <param name="item">The item for which the label is rendered.</param>
    protected abstract void DrawItemLabel(Rect region, T item);

    /// <summary>Determines whether two items are considered equal.</summary>
    /// <param name="item1">The first item to compare.</param>
    /// <param name="item2">The second item to compare.</param>
    /// <returns>True if the items are equal; otherwise, false.</returns>
    protected abstract bool AreItemsEqual(T item1, T item2);
}

/// <summary>Represents a dropdown dialog designed specifically to handle string-based options.</summary>
/// <remarks>
///     Inherits functionality from the generic <c>DropdownDialog</c> class to create a concrete implementation for
///     scenarios where options are represented as string values. Manages tasks such as displaying options, handling user
///     input, and interacting with a setter action for selection updates.
/// </remarks>
public sealed class StringDropdownDialog : DropdownDialog<string>
{
    /// <inheritdoc />
    public StringDropdownDialog(Rect parentRegion, string current, IReadOnlyList<string> allOptions, Action<string> setter) : base(parentRegion, current, allOptions, setter) {}

    /// <inheritdoc />
    protected override string GetItemLabel(string item) => item;

    /// <inheritdoc />
    protected override void DrawItemLabel(Rect region, string item)
    {
        LabelDrawer.DrawLabel(region, item);
    }

    /// <inheritdoc />
    protected override bool AreItemsEqual(string item1, string item2) => string.Equals(item1, item2, StringComparison.InvariantCultureIgnoreCase);
}

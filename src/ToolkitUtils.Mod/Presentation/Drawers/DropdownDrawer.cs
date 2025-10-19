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
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

/// <summary>A specialized class for drawing dropdown menus.</summary>
[PublicAPI]
public static class DropdownDrawer
{
    /// <summary>Draws a dropdown at the given region.</summary>
    /// <param name="region">The region to draw the dropdown in.</param>
    /// <param name="current">The current item being displayed in the dropdown display.</param>
    /// <param name="allOptions">A list containing all the available options a user can select.</param>
    /// <param name="setterFunc">An action that's called when the user selects an option in the dropdown menu.</param>
    public static void Draw(Rect region, string current, IReadOnlyList<string> allOptions, Action<string> setterFunc)
    {
        if (DrawButton(region, current)) Find.WindowStack.Add(new StringDropdownDialog(GetDialogPosition(region), current, allOptions, setterFunc));
    }

    /// <summary>Draws a dropdown at the given region.</summary>
    /// <param name="region">The region to draw the dropdown in.</param>
    /// <param name="current">The current item being displayed in the dropdown display.</param>
    /// <param name="allOptions">A list containing all the available options a user can select.</param>
    /// <param name="setterFunc">An action that's called when the user selects an option in the dropdown menu.</param>
    public static void Draw<T>(Rect region, T current, IReadOnlyList<IIdentifiable> allOptions, Action<IIdentifiable> setterFunc) where T : IIdentifiable
    {
        if (DrawButton(region, current.Name)) Find.WindowStack.Add(new IdentifiableDropdownDialog(GetDialogPosition(region), current, allOptions, setterFunc));
    }

    private static Rect GetDialogPosition(Rect parentRegion) => UI.GUIToScreenRect(parentRegion);

    /// <summary>Draws a button within the specified region with the provided label.</summary>
    /// <param name="region">The region on the screen where the button will be drawn.</param>
    /// <param name="label">The text label displayed on the button.</param>
    /// <returns>True if the button is clicked, otherwise false.</returns>
    public static bool DrawButton(Rect region, string label)
    {
        var labelRegion = new Rect(region.x + 5f, region.y, region.width - region.height, region.height);
        Rect iconRegion = LayoutHelper.IconRect(region.x + region.width - region.height, region.y, region.height, region.height, UiConstants.LineHeight * 0.3f);

        Widgets.DrawLightHighlight(region);

        GUI.color = Color.grey;
        Widgets.DrawBox(region);
        GUI.color = Color.white;

        LabelDrawer.DrawLabel(labelRegion, label);

        GUI.DrawTexture(iconRegion, Icons.AngleDown.Value);

        Widgets.DrawHighlightIfMouseover(region);

        return Widgets.ButtonInvisible(region);
    }

    internal abstract class DropdownDialog<T>(Rect parentRegion, T current, IReadOnlyList<T> allOptions, Action<T> setter)
        : Dialogs.DropdownDialog<T>(parentRegion, current, allOptions, setter)
    {
        /// <inheritdoc />
        protected override float Margin => 5f;

        /// <inheritdoc />
        protected override void SetInitialSizeAndPosition()
        {
            base.SetInitialSizeAndPosition();

            float yPosition = IsReversed ? DropdownRegion.y - ViewHeight : DropdownRegion.y;

            windowRect = new Rect(DropdownRegion.x, yPosition, DropdownRegion.width, ViewHeight + DropdownRegion.height);
            windowRect = windowRect.ExpandedBy(5f);
        }

        /// <inheritdoc />
        public override void DoWindowContents(Rect inRect)
        {
            var dropdownRegion = new Rect(x: 0f, IsReversed ? inRect.height - UiConstants.LineHeight : 0f, inRect.width, UiConstants.LineHeight);
            var dropdownOptionsRegion = new Rect(x: 0f, IsReversed ? 0f : DropdownRegion.height, inRect.width, inRect.height - UiConstants.LineHeight);

            GUI.BeginGroup(inRect);

            GUI.BeginGroup(dropdownRegion);
            DrawDropdown(dropdownRegion.AtZero());
            GUI.EndGroup();

            Widgets.DrawLightHighlight(dropdownOptionsRegion);
            Widgets.DrawLightHighlight(dropdownOptionsRegion);
            Widgets.DrawLightHighlight(dropdownOptionsRegion);

            GUI.BeginGroup(dropdownOptionsRegion);
            DrawDropdownOptions(dropdownOptionsRegion.AtZero());
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawDropdown(Rect region)
        {
            if (DrawButton(region, GetItemLabel(CurrentOption))) Close();
        }
    }

    private sealed class StringDropdownDialog : DropdownDialog<string>
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

    private sealed class IdentifiableDropdownDialog : DropdownDialog<IIdentifiable>
    {
        /// <inheritdoc />
        public IdentifiableDropdownDialog(Rect parentRegion, IIdentifiable current, IReadOnlyList<IIdentifiable> allOptions, Action<IIdentifiable> setter) : base(
            parentRegion,
            current,
            allOptions,
            setter
        ) {}

        /// <inheritdoc />
        protected override string GetItemLabel(IIdentifiable item) => item.Name;

        /// <inheritdoc />
        protected override void DrawItemLabel(Rect region, IIdentifiable item)
        {
            LabelDrawer.DrawLabel(region, item.Name);
        }

        /// <inheritdoc />
        protected override bool AreItemsEqual(IIdentifiable item1, IIdentifiable item2) => string.Equals(item1.Id, item2.Id, StringComparison.Ordinal);
    }
}

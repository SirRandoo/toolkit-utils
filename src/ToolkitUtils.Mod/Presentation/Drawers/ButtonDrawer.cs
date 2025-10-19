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
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

[PublicAPI]
public static class ButtonDrawer
{
    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, string icon) => DrawFieldButton(parentRegion, icon, tooltip: null, offset: 0, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button.</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, string icon, string? tooltip) => DrawFieldButton(parentRegion, icon, tooltip, offset: 0, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button.</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, string icon, string? tooltip, int offset) => DrawFieldButton(parentRegion, icon, tooltip, offset, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field.is in</param>
    /// <param name="icon">The icon to display on the button.</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <param name="removeControl">Whether the button will clear the keyboard control when the button is clicked by the user.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, string icon, string? tooltip, int offset, bool removeControl)
    {
        Rect region = IconDrawer.GetFieldIconRect(parentRegion, offset);
        LabelDrawer.DrawLabel(region, icon, TextAnchor.MiddleCenter, GameFont.Tiny);
        TooltipHandler.TipRegion(region, tooltip);

        return HandleInputFieldButton(region, removeControl);
    }

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon) => DrawFieldButton(parentRegion, icon, tooltip: null, offset: 0, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon, string? tooltip) => DrawFieldButton(parentRegion, icon, tooltip, offset: 0, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon, string? tooltip, int offset) => DrawFieldButton(parentRegion, icon, tooltip, offset, removeControl: true);

    /// <summary>Draws an interactable button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <param name="removeControl">Whether the button will clear the keyboard control when the button is clicked by the user.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon, string? tooltip, int offset, bool removeControl)
    {
        Rect region = LayoutHelper.IconRect(
            parentRegion.x + parentRegion.width - parentRegion.height * (offset + 1),
            parentRegion.y,
            parentRegion.height,
            parentRegion.height,
            Mathf.FloorToInt(parentRegion.height * 0.25f)
        );

        GUI.DrawTexture(region, icon);
        TooltipHandler.TipRegion(region, tooltip);

        return HandleInputFieldButton(region, removeControl);
    }

    /// <summary>Draws an interactable "clear" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool ClearButton(Rect parentRegion) => ClearButton(parentRegion, tooltip: null, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "clear" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in.</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button.</param>
    /// <returns>Whether the button was clicked by the user.</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip) => ClearButton(parentRegion, tooltip, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "clear" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip, int offset) => ClearButton(parentRegion, tooltip, offset, removeControl: true);

    /// <summary>Draws an interactable "clear" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <param name="removeControl">Whether the button will clear the keyboard control when the button is clicked by the user</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) => DrawFieldButton(parentRegion, icon: "×", tooltip, offset, removeControl);

    /// <summary>Draws an interactable "clear" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion) => DoneButton(parentRegion, tooltip: null, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "done" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip) => DoneButton(parentRegion, tooltip, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "done" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip, int offset) => DoneButton(parentRegion, tooltip, offset, removeControl: true);

    /// <summary>Draws an interactable "done" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <param name="removeControl">Whether the button will clear the keyboard control when the button is clicked by the user</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) => DrawFieldButton(parentRegion, icon: "✔", tooltip, offset, removeControl);

    /// <summary>Draws an interactable "reset" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion) => ResetButton(parentRegion, tooltip: null, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "reset" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip) => ResetButton(parentRegion, tooltip, offset: 0, removeControl: true);

    /// <summary>Draws an interactable "reset" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip, int offset) => ResetButton(parentRegion, tooltip, offset, removeControl: true);

    /// <summary>Draws an interactable "reset" button over an input field.</summary>
    /// <param name="parentRegion">The region of the screen the input field is in</param>
    /// <param name="tooltip">The tooltip to display to the user when they hover their mouse over the button</param>
    /// <param name="offset">
    ///     The offset to use when positioning the button. An offset is typically used to place multiple field
    ///     buttons on a given field.
    /// </param>
    /// <param name="removeControl">Whether the button will clear the keyboard control when the button is clicked by the user</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) =>
        DrawFieldButton(parentRegion, TexButton.CurveResetTex, tooltip, offset, removeControl);

    /// <summary>
    ///     An internal method for handling the "click" of a field button, as well as clearing keyboard control when
    ///     requested.
    /// </summary>
    /// <param name="region">The region of the field button this method is handling input for</param>
    /// <param name="removeControl">Whether the button will clear keyboard control when clicked by the user.</param>
    /// <returns>Whether the button was clicked</returns>
    private static bool HandleInputFieldButton(Rect region, bool removeControl)
    {
        Widgets.ButtonInvisible(region);
        bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

        if (!clicked || !removeControl) return clicked;

        GUIUtility.keyboardControl = 0;

        return true;
    }
}

// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using Verse;

namespace ToolkitUtils.UX;

public static class ButtonDrawer
{
    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, string? icon) => DrawFieldButton(parentRegion, icon, null, 0, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, string? icon, string? tooltip) => DrawFieldButton(parentRegion, icon, tooltip, 0, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, string? icon, string? tooltip, int offset) => DrawFieldButton(parentRegion, icon, tooltip, offset, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear the
    ///     keyboard control when the button is clicked by the user
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, string? icon, string? tooltip, int offset, bool removeControl)
    {
        Rect region = IconDrawer.GetFieldIconRect(parentRegion, offset);
        LabelDrawer.Draw(region, icon, TextAnchor.MiddleCenter, GameFont.Tiny);
        TooltipHandler.TipRegion(region, tooltip);

        return HandleInputFieldButton(region, removeControl);
    }

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon) => DrawFieldButton(parentRegion, icon, null, 0, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon, string? tooltip) => DrawFieldButton(parentRegion, icon, tooltip, 0, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DrawFieldButton(Rect parentRegion, Texture2D icon, string? tooltip, int offset) => DrawFieldButton(parentRegion, icon, tooltip, offset, true);

    /// <summary>
    ///     Draws an interactable button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="icon">The icon to display on the button</param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear the
    ///     keyboard control when the button is clicked by the user
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
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

    /// <summary>
    ///     Draws an interactable "clear" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion) => ClearButton(parentRegion, null, 0, true);

    /// <summary>
    ///     Draws an interactable "clear" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip) => ClearButton(parentRegion, tooltip, 0, true);

    /// <summary>
    ///     Draws an interactable "clear" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip, int offset) => ClearButton(parentRegion, tooltip, offset, true);

    /// <summary>
    ///     Draws an interactable "clear" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear the
    ///     keyboard control when the button is clicked by the user
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ClearButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) =>
        DrawFieldButton(parentRegion, "×", tooltip, offset, removeControl);

    /// <summary>
    ///     Draws an interactable "clear" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion) => DoneButton(parentRegion, null, 0, true);

    /// <summary>
    ///     Draws an interactable "done" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip) => DoneButton(parentRegion, tooltip, 0, true);

    /// <summary>
    ///     Draws an interactable "done" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip, int offset) => DoneButton(parentRegion, tooltip, offset, true);

    /// <summary>
    ///     Draws an interactable "done" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear the
    ///     keyboard control when the button is clicked by the user
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool DoneButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) => DrawFieldButton(parentRegion, "✔", tooltip, offset, removeControl);

    /// <summary>
    ///     Draws an interactable "reset" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion) => ResetButton(parentRegion, null, 0, true);

    /// <summary>
    ///     Draws an interactable "reset" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip) => ResetButton(parentRegion, tooltip, 0, true);

    /// <summary>
    ///     Draws an interactable "reset" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip, int offset) => ResetButton(parentRegion, tooltip, offset, true);

    /// <summary>
    ///     Draws an interactable "reset" button over an input field.
    /// </summary>
    /// <param name="parentRegion">
    ///     The region of the screen the input field
    ///     is in
    /// </param>
    /// <param name="tooltip">
    ///     The tooltip to display to the user when they
    ///     hover their mouse over the button
    /// </param>
    /// <param name="offset">
    ///     The offset to use when positioning the button.
    ///     An offset is typically used to place multiple field buttons on a
    ///     given field.
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear the
    ///     keyboard control when the button is clicked by the user
    /// </param>
    /// <returns>Whether the button was clicked by the user</returns>
    public static bool ResetButton(Rect parentRegion, string? tooltip, int offset, bool removeControl) =>
        DrawFieldButton(parentRegion, TexButton.CurveResetTex, tooltip, offset, removeControl);

    /// <summary>
    ///     An internal method for handling the "click" of a field button, as
    ///     well as clearing keyboard control when requested.
    /// </summary>
    /// <param name="region">
    ///     The region of the field button this method is
    ///     handling input for
    /// </param>
    /// <param name="removeControl">
    ///     Whether the button will clear keyboard control when clicked by
    ///     the user.
    /// </param>
    /// <returns>Whether the button was clicked</returns>
    private static bool HandleInputFieldButton(Rect region, bool removeControl)
    {
        Widgets.ButtonInvisible(region);
        bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

        if (!clicked || !removeControl)
        {
            return clicked;
        }

        GUIUtility.keyboardControl = 0;

        return true;
    }
}

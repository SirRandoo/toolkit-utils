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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Presentation.Extensions;
using ToolkitUtils.Mod.Presentation.Helpers;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Mod.Presentation.Drawers;

// TODO: Number fields should be decorated with an error icon to better indicate to the user that the text in the field isn't a valid number.

[PublicAPI]
public static class FieldDrawer
{
    private const NumberStyles BaseNumberStyles = NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite;
    private const NumberStyles FloatNumberStyles = BaseNumberStyles | NumberStyles.AllowDecimalPoint;

    /// <summary>Draws a text field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the text field in.</param>
    /// <param name="value">The text to draw within the text field.</param>
    /// <param name="newValue">
    ///     The new text if the user modified the text field, or <see langword="null" /> if the user hasn't
    ///     modified the field's contents.
    /// </param>
    /// <returns>Whether the text within the text field was modified by the user.</returns>
    public static bool DrawTextField(Rect region, string? value, [NotNullWhen(true)] out string? newValue)
    {
        string content = Widgets.TextField(region, value);
        newValue = string.Equals(value, content, StringComparison.InvariantCulture) ? null : content;

        return newValue != null;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The integer that was parsed from the user's input, or <c>0</c> if the value within the field could
    ///     not be parsed into a valid integer.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string stored at the reference
    ///     will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text field is a valid
    ///     integer. If the contents aren't a valid integer, the text field will be decorated with information to inform the
    ///     user the text field's content isn't a valid integer.
    /// </param>
    /// <param name="minimum">The minimum integer <see cref="value" /> can be.</param>
    /// <param name="maximum">The maximum integer <see cref="value" /> can be.</param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(Rect region, out int value, ref string? buffer, ref bool bufferValid, int minimum = int.MinValue, int maximum = int.MaxValue)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out string? newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (int.TryParse(buffer, BaseNumberStyles, NumberFormatInfo.CurrentInfo, out int result))
        {
            value = Mathf.Clamp(result, minimum, maximum);
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The floating point value that was parsed from the user's input, or <c>0</c> if the value within the
    ///     field could not be parsed into a valid floating point value.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string stored at the reference
    ///     will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text field is a valid
    ///     floating point value. If the contents isn't a valid floating point value, the text field will be decorated with
    ///     information to inform the user the text field's content isn't a valid floating point value.
    /// </param>
    /// <param name="minimum">The minimum floating point value <see cref="value" /> can be.</param>
    /// <param name="maximum">The maximum floating point value <see cref="value" /> can be.</param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(Rect region, out float value, ref string? buffer, ref bool bufferValid, float minimum = float.MinValue, float maximum = float.MaxValue)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out string? newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (float.TryParse(buffer, FloatNumberStyles, NumberFormatInfo.CurrentInfo, out float result))
        {
            value = Mathf.Clamp(result, minimum, maximum);
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a number field on the specified region of the current drawing canvas.</summary>
    /// <param name="region">The region of the drawing canvas to draw the number field in.</param>
    /// <param name="value">
    ///     The floating point value that was parsed from the user's input, or <c>0</c> if the value within the
    ///     field could not be parsed into a valid floating point value.
    /// </param>
    /// <param name="buffer">
    ///     A reference to the text being displayed within the text field. The string stored at the reference
    ///     will be updated when the user changes the contents of the text field.
    /// </param>
    /// <param name="bufferValid">
    ///     A reference to a boolean that dictates whether the contents of the text field is a valid
    ///     floating point value. If the contents isn't a valid floating point value, the text field will be decorated with
    ///     information to inform the user the text field's content isn't a valid floating point value.
    /// </param>
    /// <returns>Whether a number was successfully parsed from the input.</returns>
    public static bool DrawNumberField(Rect region, out double value, ref string buffer, ref bool bufferValid)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(region, buffer, out string? newValue))
        {
            value = 0;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (double.TryParse(buffer, FloatNumberStyles, NumberFormatInfo.CurrentInfo, out double result))
        {
            value = result;
            bufferValid = true;

            GUI.backgroundColor = Color.white;

            return true;
        }

        value = 0;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }

    /// <summary>Draws a URI input field within the specified region of the canvas.</summary>
    /// <param name="region">The area of the canvas where the URI input field will be drawn.</param>
    /// <param name="value">
    ///     The parsed <see cref="Uri" /> if the user input is valid, or <see langword="null" /> if the input
    ///     is invalid or unchanged.
    /// </param>
    /// <param name="buffer">The textual input for the URI field. This is updated to reflect the current user input.</param>
    /// <param name="bufferValid">Indicates whether the current text in <paramref name="buffer" /> represents a valid URI.</param>
    /// <returns>A value indicating whether the URI field's contents have been modified and are valid.</returns>
    public static bool DrawUri(Rect region, [NotNullWhen(true)] out Uri? value, ref string? buffer, ref bool bufferValid)
    {
        Rect fieldRegion = region;

        if (!bufferValid)
        {
            Rect warningIcon = LayoutHelper.IconRect(region.x + region.width - region.height, region.y, region.height, region.height, margin: 6f);
            fieldRegion.SetWidth(fieldRegion.width - region.height - 2f);

            IconDrawer.DrawIcon(warningIcon, Icons.TriangleExclamation.Value, UxColors.RedishPink);

            // TODO: This is pretty broad, and doesn't tell the user anything specific.
            TooltipHandler.TipRegion(warningIcon, UxLocale.InvalidUrlColored);
        }

        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (!DrawTextField(fieldRegion, buffer, out string? newValue))
        {
            value = null;
            GUI.backgroundColor = Color.white;

            return false;
        }

        buffer = newValue;

        if (Uri.TryCreate(buffer, UriKind.Absolute, out value)) // TODO: Replace this with something more optimal
        {
            bufferValid = true;
            GUI.backgroundColor = Color.white;

            return true;
        }

        value = null;
        bufferValid = false;

        GUI.backgroundColor = Color.white;

        return false;
    }
}

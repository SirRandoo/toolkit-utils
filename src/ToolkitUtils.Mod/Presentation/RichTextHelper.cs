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
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace ToolkitUtils.Mod.Presentation;

/// <summary>A utility class for creating and removing rich text that Unity can display in certain contexts.</summary>
[PublicAPI]
public static class RichTextHelper
{
    /// <summary>Determines whether a given string contains any rich text.</summary>
    /// <param name="source">The string to check for any rich text tags.</param>
    /// <remarks>
    ///     This check is rather primitive, as it only checks if there's an opening and closing character within 18
    ///     characters of each other. This method does not check to make sure the found tag is a valid rich text tag.
    /// </remarks>
    public static bool IsRichText(this string source)
    {
        int tagStart = -1;
        ReadOnlySpan<char> span = source.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            char current = span[i];

            switch (current)
            {
                case '<': tagStart = i; break;
                case '>' when tagStart >= 0:
                    if (i - tagStart > 18) break;

                    return true;
            }
        }

        return false;
    }

    /// <summary>Removes rich text tags from a given string.</summary>
    /// <param name="input">The string to remove rich text tags from.</param>
    /// <remarks>
    ///     This method is rather primitive, as it simply doesn't return any text within &lt;&gt;'s, nor the symbols
    ///     themselves.
    /// </remarks>
    public static string StripTags(this string input)
    {
        int tagStart = -1;
        var builder = new StringBuilder();
        ReadOnlySpan<char> span = input.AsSpan();

        for (var i = 0; i < span.Length; i++)
        {
            char current = span[i];

            switch (current)
            {
                case '<':                    tagStart = i; break;
                case '>' when tagStart >= 0: tagStart = -1; break;
                default:
                    if (tagStart >= 0) break;

                    builder.Append(current);

                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary>Returns a new string with the contents of the old string surrounded by rich text tags.</summary>
    /// <param name="source">The text to surround with rich text tags.</param>
    /// <param name="tag">The type of (primitive) rich text tag to surround the source string with.</param>
    /// <remarks>
    ///     This method only supports primitive rich text tags, such as bold and italics. In essence, this method is only
    ///     meant for tags that require no arguments.
    /// </remarks>
    public static string Tagged(this string source, string tag) => $"<{tag}>{source}</{tag}>";

    /// <summary>Returns a new string with the contents of the old string surrounded by a color rich text tag.</summary>
    /// <param name="source">The text to surround with a color rich text tag.</param>
    /// <param name="color">The color of the text, specified as a hex color code.</param>
    public static string ColorTagged(this string source, string color) =>
        !color.StartsWith("#") ? $"""<color="#{color}">{source}</color>""" : $"""<color="{color}">{source}</color>""";

    /// <summary>Returns a new string with the contents of the old string surrounded by a color rich text tag.</summary>
    /// <param name="source">The text to surround with a color rich text tag.</param>
    /// <param name="color">The color of the text, specified as a <see cref="Color" /> instance.</param>
    public static string ColorTagged(this string source, Color color) => ColorTagged(source, ColorUtility.ToHtmlStringRGB(color));
}

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

using System;
using System.Text;
using UnityEngine;

namespace ToolkitUtils.UX;

/// <summary>
///     A utility class for creating and removing rich text that Unity can display in certain contexts.
/// </summary>
public static class RichTextHelper
{
    /// <summary>
    ///     Determines whether a given string contains any rich text.
    /// </summary>
    /// <param name="source">The string to check for any rich text tags.</param>
    /// <remarks>
    ///     This check is rather primitive, as it only checks if there's an opening and closing character
    ///     within 18 characters of each other. This method does not check to make sure the found tag is
    ///     a valid rich text tag.
    /// </remarks>
    public static bool IsRichText(this string source)
    {
        int tagStart = -1;

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var span = source.AsReadOnlySpan();

        for (var i = 0; i < span.Length; i++)
        {
            char current = span[i];

            switch (current)
            {
                case '<':
                    tagStart = i;

                    break;
                case '>' when tagStart >= 0:
                    if (i - tagStart > 18)
                    {
                        break;
                    }

                    return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Removes rich text tags from a given string.
    /// </summary>
    /// <param name="input">The string to remove rich text tags from.</param>
    /// <remarks>
    ///     This method is rather primitive, as it simply doesn't return any text within &lt;&gt;'s, nor
    ///     the symbols themselves.
    /// </remarks>
    public static string StripTags(this string input)
    {
        int tagStart = -1;
        var builder = new StringBuilder();

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var span = input.AsReadOnlySpan();

        for (var i = 0; i < span.Length; i++)
        {
            char current = span[i];

            switch (current)
            {
                case '<':
                    tagStart = i;

                    break;
                case '>' when tagStart >= 0:
                    tagStart = -1;

                    break;
                default:
                    if (tagStart >= 0)
                    {
                        break;
                    }

                    builder.Append(current);

                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Returns a new string with the contents of the old string surrounded by rich text tags.
    /// </summary>
    /// <param name="source">The text to surround with rich text tags.</param>
    /// <param name="tag">The type of (primitive) rich text tag to surround the source string with.</param>
    /// <remarks>
    ///     This method only supports primitive rich text tags, such as bold and italics. In essence, this
    ///     method is only meant for tags that require no arguments.
    /// </remarks>
    public static string Tagged(this string source, string tag) => $"<{tag}>{source}</{tag}>";

    /// <summary>
    ///     Returns a new string with the contents of the old string surrounded by a color rich text tag.
    /// </summary>
    /// <param name="source">The text to surround with a color rich text tag.</param>
    /// <param name="color">The color of the text, specified as a hex color code.</param>
    public static string ColorTagged(this string source, string color)
    {
        if (!color.StartsWith("#"))
        {
            return $"""
                    <color="#{color}">{source}</color>
                    """;
        }

        return $"""
                <color="{color}">{source}</color>
                """;
    }

    /// <summary>
    ///     Returns a new string with the contents of the old string surrounded by a color rich text tag.
    /// </summary>
    /// <param name="source">The text to surround with a color rich text tag.</param>
    /// <param name="color">The color of the text, specified as a <see cref="Color"/> instance.</param>
    public static string ColorTagged(this string source, Color color) => ColorTagged(source, ColorUtility.ToHtmlStringRGB(color));
}

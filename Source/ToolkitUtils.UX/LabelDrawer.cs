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

public static class LabelDrawer
{
    public static void Draw(Rect region, string? text, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        Draw(region, text, Color.white, anchor, font);
    }

    public static void Draw(Rect region, string? text, Color textColor, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont font = GameFont.Small)
    {
        TextAnchor previousAnchor = Text.Anchor;
        GameFont previousFont = Text.Font;
        Color previousColor = GUI.color;

        Text.Anchor = anchor;
        Text.Font = font;

        GUI.color = textColor;
        Widgets.Label(region, text);
        GUI.color = previousColor;

        Text.Anchor = previousAnchor;
        Text.Font = previousFont;
    }
}

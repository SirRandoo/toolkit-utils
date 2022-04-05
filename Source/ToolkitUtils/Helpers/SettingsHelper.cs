// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Helpers;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class SettingsHelper
    {
        public static void DrawPriceField(Rect canvas, ref int price)
        {
            const float buttonWidth = 50f;

            var reduceRect = new Rect(canvas.x, canvas.y, buttonWidth, canvas.height);
            var raiseRect = new Rect(canvas.x + canvas.width - buttonWidth, canvas.y, buttonWidth, canvas.height);
            var fieldRect = new Rect(canvas.x + buttonWidth + 2f, canvas.y, canvas.width - buttonWidth * 2 - 4f, canvas.height);
            var buffer = price.ToString();
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);


            if (control && shift)
            {
                buffer = DrawControlShiftPriceBtns(ref price, reduceRect, buffer, raiseRect);
            }
            else if (control)
            {
                buffer = DrawControlPriceBtns(ref price, reduceRect, buffer, raiseRect);
            }
            else if (shift)
            {
                buffer = DrawShiftPriceBtns(ref price, reduceRect, buffer, raiseRect);
            }
            else
            {
                buffer = DrawBasePriceBtns(ref price, reduceRect, buffer, raiseRect);
            }


            Widgets.TextFieldNumeric(fieldRect, ref price, ref buffer, 1f);
        }

        private static string DrawControlShiftPriceBtns(ref int price, Rect reduceRect, string buffer, Rect raiseRect)
        {
            if (Widgets.ButtonText(reduceRect, "-1000"))
            {
                price -= 1000;
                buffer = price.ToString();
            }

            if (Widgets.ButtonText(raiseRect, "+1000"))
            {
                price += 1000;
                buffer = price.ToString();
            }

            return buffer;
        }

        private static string DrawControlPriceBtns(ref int price, Rect reduceRect, string buffer, Rect raiseRect)
        {
            if (Widgets.ButtonText(reduceRect, "-100"))
            {
                price -= 100;
                buffer = price.ToString();
            }

            if (Widgets.ButtonText(raiseRect, "+100"))
            {
                price += 100;
                buffer = price.ToString();
            }

            return buffer;
        }

        private static string DrawShiftPriceBtns(ref int price, Rect reduceRect, string buffer, Rect raiseRect)
        {
            if (Widgets.ButtonText(reduceRect, "-10"))
            {
                price -= 10;
                buffer = price.ToString();
            }

            if (Widgets.ButtonText(raiseRect, "+10"))
            {
                price += 10;
                buffer = price.ToString();
            }

            return buffer;
        }

        private static string DrawBasePriceBtns(ref int price, Rect reduceRect, string buffer, Rect raiseRect)
        {
            if (Widgets.ButtonText(reduceRect, "-1"))
            {
                price -= 1;
                buffer = price.ToString();
            }

            if (Widgets.ButtonText(raiseRect, "+1"))
            {
                price += 1;
                buffer = price.ToString();
            }

            return buffer;
        }

        public static bool DrawTableHeader(
            Rect backgroundRect,
            Rect textRect,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false
        )
        {
            Text.Anchor = anchor;
            Text.Font = fontScale;

            if (vertical)
            {
                backgroundRect.y += backgroundRect.width;
                GUIUtility.RotateAroundPivot(-90f, backgroundRect.position);
            }

            GUI.color = new Color(0.62f, 0.65f, 0.66f);
            Widgets.DrawHighlight(backgroundRect);
            GUI.color = Color.white;

            if (Mouse.IsOver(backgroundRect))
            {
                GUI.color = Color.grey;
                Widgets.DrawLightHighlight(backgroundRect);
                GUI.color = Color.white;
            }

            Widgets.Label(textRect, text);
            bool pressed = Widgets.ButtonInvisible(backgroundRect);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            return pressed;
        }

        public static bool DrawTableHeader(Rect backgroundRect, Rect iconRect, Texture2D icon, bool vertical = false)
        {
            if (vertical)
            {
                backgroundRect.y += backgroundRect.width;
                GUIUtility.RotateAroundPivot(-90f, backgroundRect.position);
            }

            GUI.color = new Color(0.62f, 0.65f, 0.66f);
            Widgets.DrawHighlight(backgroundRect);
            GUI.color = Color.white;

            if (Mouse.IsOver(backgroundRect))
            {
                GUI.color = Color.grey;
                Widgets.DrawLightHighlight(backgroundRect);
                GUI.color = Color.white;
            }

            GUI.DrawTexture(iconRect, icon);
            bool pressed = Widgets.ButtonInvisible(backgroundRect);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            return pressed;
        }

        public static bool DrawNumberField(Rect region, ref int value, ref string buffer, out int newValue, float min = 0f, float max = 1E+09F)
        {
            int proxy = value;
            Widgets.TextFieldNumeric(region, ref proxy, ref buffer, min, max);

            newValue = proxy;

            return proxy != value;
        }

        public static bool DrawNumberField(Rect region, ref float value, ref string buffer, out float newValue, float min = 0f, float max = 1E+09F)
        {
            float proxy = value;
            Widgets.TextFieldNumeric(region, ref proxy, ref buffer, min, max);

            newValue = proxy;

            return Math.Abs(proxy - value) > 0.0001f;
        }

        public static void DrawKarmaField(
            Rect labelRect,
            string label,
            Rect fieldRect,
            string nullLabel,
            KarmaType? karmaType,
            Action<KarmaType?> changedCallback,
            bool doResetButton = false,
            [CanBeNull] string resetTooltip = null
        )
        {
            UiHelper.Label(labelRect, label);

            if (Widgets.ButtonText(fieldRect, !karmaType.HasValue ? nullLabel : karmaType.Value.ToString()))
            {
                Find.WindowStack.Add(new FloatMenu(Data.KarmaTypes.Select(i => new FloatMenuOption(i.ToString(), () => changedCallback(i))).ToList()));
            }

            if (doResetButton && karmaType.HasValue && UiHelper.FieldButton(labelRect, Textures.Reset, resetTooltip))
            {
                changedCallback(null);
            }
        }
    }
}

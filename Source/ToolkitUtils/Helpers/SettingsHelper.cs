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
using System.Reflection;
using HarmonyLib;
using RimWorld;
using SirRandoo.ToolkitUtils.Workers;
using Steamworks;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class SettingsHelper
    {
        private static readonly FieldInfo SelectedModField = AccessTools.Field(typeof(Dialog_ModSettings), "selMod");

        private static readonly GameFont[] GameFonts = Enum.GetNames(typeof(GameFont))
           .Select(f => (GameFont) Enum.Parse(typeof(GameFont), f))
           .OrderByDescending(f => (int) f)
           .ToArray();

        public static bool DrawFieldButton(Rect canvas, string label, string tooltip = null)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, label, false);

            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(region, tooltip);
            }

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            return true;
        }

        public static bool DrawFieldButton(Rect canvas, Texture2D icon, string tooltip = null)
        {
            var region = new Rect(
                canvas.x + canvas.width - canvas.height + 6f,
                canvas.y + 6f,
                canvas.height - 12f,
                canvas.height - 12f
            );
            Widgets.ButtonImage(region, icon);

            if (!tooltip.NullOrEmpty())
            {
                TooltipHandler.TipRegion(region, tooltip);
            }

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            return true;
        }

        public static bool DrawClearButton(Rect canvas)
        {
            return DrawFieldButton(canvas, "×");
        }

        public static void DrawSortIndicator(Rect canvas, SortOrder order)
        {
            var region = new Rect(
                canvas.x + canvas.width - canvas.height + 3f,
                canvas.y + 8f,
                canvas.height - 9f,
                canvas.height - 16f
            );

            switch (order)
            {
                case SortOrder.Ascending:
                    GUI.DrawTexture(region, Textures.SortingAscend);
                    return;
                case SortOrder.Descending:
                    GUI.DrawTexture(region, Textures.SortingDescend);
                    return;
                default:
                    GUI.DrawTexture(region, Textures.QuestionMark);
                    return;
            }
        }

        public static void DrawPriceField(Rect canvas, ref int price)
        {
            const float buttonWidth = 50f;

            var reduceRect = new Rect(canvas.x, canvas.y, buttonWidth, canvas.height);
            var raiseRect = new Rect(canvas.x + canvas.width - buttonWidth, canvas.y, buttonWidth, canvas.height);
            var fieldRect = new Rect(
                canvas.x + buttonWidth + 2f,
                canvas.y,
                canvas.width - buttonWidth * 2 - 4f,
                canvas.height
            );
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

        public static bool DrawDoneButton(Rect canvas)
        {
            return DrawFieldButton(canvas, "✔");
        }

        public static void DrawShowButton(Rect canvas, ref bool state)
        {
            if (DrawFieldButton(canvas, state ? Textures.Hidden : Textures.Visible))
            {
                state = !state;
            }
        }

        public static bool WasLeftClicked(this Rect region)
        {
            return WasMouseButtonClicked(region, 0);
        }

        public static bool WasRightClicked(this Rect region)
        {
            return WasMouseButtonClicked(region, 1);
        }

        public static bool WasMouseButtonClicked(this Rect region, int mouseButton)
        {
            if (!Mouse.IsOver(region))
            {
                return false;
            }

            Event current = Event.current;
            bool was = current.button == mouseButton;

            switch (current.type)
            {
                case EventType.Used when was:
                case EventType.MouseDown when was:
                    current.Use();
                    return true;
                default:
                    return false;
            }
        }

        public static Rect ShiftLeft(this Rect region, float padding = 5f)
        {
            return new Rect(region.x - region.width - padding, region.y, region.width, region.height);
        }

        public static Rect ShiftRight(this Rect region, float padding = 5f)
        {
            return new Rect(region.x + region.width + padding, region.y, region.width, region.height);
        }

        public static bool IsRegionVisible(this Rect region, Rect scrollRect, Vector2 scrollPos)
        {
            return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y)
                   && region.y <= scrollPos.y + scrollRect.height;
        }

        public static void DrawColored(this Texture2D t, Rect region, Color color)
        {
            Color old = GUI.color;

            GUI.color = color;
            GUI.DrawTexture(region, t);
            GUI.color = old;
        }

        public static void DrawLabel(
            Rect region,
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
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            Widgets.Label(region, text);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        public static void DrawTabBackground(Rect region, bool vertical = false, bool selected = false)
        {
            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            GUI.color = selected ? new Color(0.46f, 0.49f, 0.5f) : new Color(0.21f, 0.23f, 0.24f);
            Widgets.DrawHighlight(region);
            GUI.color = Color.white;

            if (!selected && Mouse.IsOver(region))
            {
                Widgets.DrawLightHighlight(region);
            }

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }
        }

        public static bool DrawTextField(Rect region, string content, out string newContent)
        {
            string text = Widgets.TextField(region, content);

            newContent = !text.Equals(content) ? text : null;
            return newContent != null;
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

            GUI.color = new Color(0.21f, 0.23f, 0.24f);
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

            GUI.color = new Color(0.21f, 0.23f, 0.24f);
            Widgets.DrawHighlight(backgroundRect);
            GUI.color = Color.white;

            if (Mouse.IsOver(backgroundRect))
            {
                GUI.color = Color.gray;
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

        public static bool DrawTabButton(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false,
            bool selected = false
        )
        {
            Text.Anchor = anchor;
            Text.Font = fontScale;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            GUI.color = selected ? new Color(0.46f, 0.49f, 0.5f) : new Color(0.21f, 0.23f, 0.24f);
            Widgets.DrawHighlight(region);
            GUI.color = Color.white;

            if (!selected && Mouse.IsOver(region))
            {
                Widgets.DrawLightHighlight(region);
            }

            Widgets.Label(region, text);
            bool pressed = Widgets.ButtonInvisible(region);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            return pressed;
        }

        public static void DrawColoredLabel(
            Rect region,
            string text,
            Color color,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false
        )
        {
            GUI.color = color;
            DrawLabel(region, text, anchor, fontScale, vertical);
            GUI.color = Color.white;
        }

        public static void DrawFittedLabel(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont maxScale = GameFont.Small,
            bool vertical = false
        )
        {
            Text.Anchor = anchor;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            var maxFontScale = (int) maxScale;
            foreach (GameFont f in GameFonts)
            {
                if ((int) f > maxFontScale)
                {
                    continue;
                }

                Text.Font = f;

                if (Text.CalcSize(text).x <= region.width)
                {
                    break;
                }
            }

            Widgets.Label(region, text);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left.Rounded(),
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height).Rounded()
            );
        }

        public static Tuple<Rect, Rect> GetRectAsForm(this Listing listing, float factor = 0.8f)
        {
            return listing.GetRect(Text.LineHeight).ToForm(factor);
        }

        public static string Tagged(this string s, string tag)
        {
            return $"<{tag}>{s}</{tag}>";
        }

        public static string ColorTagged(this string s, string hex)
        {
            if (!hex.StartsWith("#"))
            {
                hex = $"#{hex}";
            }

            return $@"<color=""{hex}"">{s}</color>";
        }

        public static string ColorTagged(this string s, Color color)
        {
            return ColorTagged(s, ColorUtility.ToHtmlStringRGB(color));
        }

        public static void TipRegion(this Rect region, string tooltip)
        {
            TooltipHandler.TipRegion(region, tooltip);
            Widgets.DrawHighlightIfMouseover(region);
        }

        public static void OpenSettingsMenuFor(Mod modInstance)
        {
            var settings = new Dialog_ModSettings();
            SelectedModField.SetValue(settings, modInstance);

            Find.WindowStack.Add(settings);
        }

        public static Rect TrimLeft(this Rect region, float amount)
        {
            return new Rect(region.x + amount, region.y, region.width - amount, region.height);
        }

        public static Rect WithWidth(this Rect region, float width)
        {
            return new Rect(region.x, region.y, width, region.height);
        }

        public static void DrawExperimentalNotice(this Listing listing)
        {
            listing.DrawDescription("TKUtils.Experimental".Localize(), new Color(1f, 0.53f, 0.76f));
        }

        public static void DrawDescription(this Listing listing, string description, Color color)
        {
            GameFont fontCache = Text.Font;
            GUI.color = color;
            Text.Font = GameFont.Tiny;
            float height = Text.CalcHeight(description, listing.ColumnWidth * 0.7f);
            DrawLabel(
                listing.GetRect(height).TrimLeft(10f).WithWidth(listing.ColumnWidth * 0.7f).Rounded(),
                description,
                TextAnchor.UpperLeft,
                GameFont.Tiny
            );
            GUI.color = Color.white;
            Text.Font = fontCache;

            listing.Gap(6f);
        }

        public static void DrawDescription(this Listing listing, string description)
        {
            DrawDescription(listing, description, new Color(0.72f, 0.72f, 0.72f));
        }

        public static void DrawGroupHeader(this Listing listing, string heading, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
            }

            DrawLabel(listing.GetRect(Text.LineHeight), heading, TextAnchor.LowerLeft, GameFont.Tiny);
            listing.GapLine(6f);
        }

        public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
            }

            Rect lineRect = listing.GetRect(Text.LineHeight);
            DrawLabel(lineRect, modName, TextAnchor.LowerLeft, GameFont.Tiny);

            string modRequirementString = "TKUtils.ModRequirement".Localize(modName);
            GUI.color = new Color(1f, 0.53f, 0.76f);

            Text.Font = GameFont.Tiny;
            float width = Text.CalcSize(modRequirementString).x;
            var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
            Text.Font = GameFont.Small;

            DrawLabel(lineRect, modRequirementString, TextAnchor.LowerRight, GameFont.Tiny);
            GUI.color = Color.white;

            Widgets.DrawHighlightIfMouseover(modRequirementRect);
            if (Widgets.ButtonInvisible(modRequirementRect))
            {
                SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));
            }

            listing.GapLine(6f);
        }

        public static bool DrawCheckbox(Rect canvas, ref bool state)
        {
            bool proxy = state;
            Widgets.Checkbox(canvas.position, ref proxy, Mathf.Min(canvas.width, canvas.height), paintable: true);

            bool changed = proxy != state;
            state = proxy;
            return changed;
        }
    }
}

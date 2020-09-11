using System;
using Steamworks;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class SettingsHelper
    {
        public static bool DrawClearButton(Rect canvas)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "×", false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUI.FocusControl(null);
            return true;
        }

        public static void DrawPriceField(Rect canvas, ref int price, ref bool control, ref bool shift)
        {
            const float buttonWidth = 50f;

            var reduceRect = new Rect(canvas.x, canvas.y, buttonWidth, canvas.height);
            var raiseRect = new Rect(canvas.x + canvas.width - buttonWidth, canvas.y, buttonWidth, canvas.height);
            var fieldRect = new Rect(
                canvas.x + buttonWidth + 5f,
                canvas.y,
                canvas.width - buttonWidth * 2 - 10f,
                canvas.height
            );
            Event currentEvent = Event.current;
            var buffer = price.ToString();

            if (currentEvent.type == EventType.Used || currentEvent.type == EventType.KeyDown)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.LeftShift:
                    case KeyCode.RightShift:
                        shift = true;
                        break;
                    case KeyCode.LeftControl:
                    case KeyCode.RightControl:
                        control = true;
                        break;
                }
            }
            else if (currentEvent.type == EventType.KeyUp)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.LeftShift:
                    case KeyCode.RightShift:
                        shift = false;
                        break;
                    case KeyCode.LeftControl:
                    case KeyCode.RightControl:
                        control = false;
                        break;
                }
            }
            else if (currentEvent.type == EventType.Repaint || currentEvent.type == EventType.Layout)
            {
                shift = currentEvent.shift;
            }


            if (control && shift)
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
            }
            else if (control)
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
            }
            else if (shift)
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
            }
            else
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
            }


            Widgets.TextFieldNumeric(fieldRect, ref price, ref buffer);
        }

        public static bool DrawDoneButton(Rect canvas)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "✔", false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUI.FocusControl(null);
            return true;
        }

        public static void DrawShowButton(Rect canvas, ref bool state)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonImageFitted(region, state ? Textures.Hidden : Textures.Visible);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return;
            }

            state = !state;
            GUI.FocusControl(null);
        }

        public static bool WasRightClicked(this Rect region)
        {
            if (!Mouse.IsOver(region))
            {
                return false;
            }

            Event current = Event.current;
            bool was = current.button == 1;

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
            region.x -= region.width + padding;

            return region;
        }

        public static Rect ShiftRight(this Rect region, float padding = 5f)
        {
            region.x += region.width + padding;

            return region;
        }

        public static bool IsRegionVisible(this Rect region, Rect scrollView, Vector2 scrollPos)
        {
            return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y)
                   && region.y <= scrollPos.y + scrollView.height;
        }

        public static void DrawColored(this Texture2D t, Rect region, Color color)
        {
            Color old = GUI.color;

            GUI.color = color;
            GUI.DrawTexture(region, t);
            GUI.color = old;
        }

        public static void DrawLabelAnchored(Rect region, string text, TextAnchor anchor)
        {
            TextAnchor cache = Text.Anchor;

            Text.Anchor = anchor;
            Widgets.Label(region, text);
            Text.Anchor = cache;
        }

        public static void DrawBigLabelAnchored(Rect region, string text, TextAnchor anchor)
        {
            GameFont cache = Text.Font;

            Text.Font = GameFont.Medium;
            DrawLabelAnchored(region, text, anchor);
            Text.Font = cache;
        }

        public static void DrawSmallLabelAnchored(Rect region, string text, TextAnchor anchor)
        {
            GameFont cache = Text.Font;

            Text.Font = GameFont.Tiny;
            DrawLabelAnchored(region, text, anchor);
            Text.Font = cache;
        }

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left,
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height)
            );
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
            DrawSmallLabelAnchored(
                listing.GetRect(height).TrimLeft(10f).WithWidth(listing.ColumnWidth * 0.7f),
                description,
                TextAnchor.UpperLeft
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
                listing.Gap(Text.LineHeight * 1.25f);
            }

            DrawSmallLabelAnchored(listing.GetRect(Text.LineHeight), heading, TextAnchor.LowerLeft);
            listing.GapLine(6f);
        }

        public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Text.LineHeight * 1.25f);
            }

            Rect lineRect = listing.GetRect(Text.LineHeight);
            DrawSmallLabelAnchored(lineRect, modName, TextAnchor.LowerLeft);

            string modRequirementString = "TKUtils.ModRequirement".Localize(modName);
            GUI.color = new Color(1f, 0.53f, 0.76f);

            Text.Font = GameFont.Tiny;
            float width = Text.CalcSize(modRequirementString).x;
            var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
            Text.Font = GameFont.Small;

            DrawSmallLabelAnchored(lineRect, modRequirementString, TextAnchor.LowerRight);
            GUI.color = Color.white;

            Widgets.DrawHighlightIfMouseover(modRequirementRect);
            if (Widgets.ButtonInvisible(modRequirementRect))
            {
                SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));
            }

            listing.GapLine(6f);
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Steamworks;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class SettingsHelper
    {
        private static readonly FieldInfo SelectedModField = AccessTools.Field(typeof(Dialog_ModSettings), "selMod");

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

        [SuppressMessage("ReSharper", "CognitiveComplexity")]
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

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left.Rounded(),
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height).Rounded()
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
                listing.GetRect(height).TrimLeft(10f).WithWidth(listing.ColumnWidth * 0.7f),
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
                listing.Gap(Text.LineHeight * 1.25f);
            }

            DrawLabel(listing.GetRect(Text.LineHeight), heading, TextAnchor.LowerLeft, GameFont.Tiny);
            listing.GapLine(6f);
        }

        public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Text.LineHeight * 1.25f);
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
    }
}

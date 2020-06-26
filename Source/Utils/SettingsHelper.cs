using System;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class SettingsHelper
    {
        public static bool DrawClearButton(Rect canvas)
        {
            var region = new Rect(canvas.x + canvas.width - 16f, canvas.y, 16f, canvas.height);
            Widgets.ButtonText(region, "×", false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Event.current.clickCount > 0;

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

        public static bool IsRegionVisible(this Rect region, Rect scrollView, Vector2 scrollPos)
        {
            return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y)
                   && region.y <= scrollPos.y + scrollView.height;
        }

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.7f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left,
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height)
            );
        }
    }
}

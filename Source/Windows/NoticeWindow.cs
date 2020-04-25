using System;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class NoticeWindow : Window
    {
        private readonly string def;

        public NoticeWindow(string def)
        {
            this.def = def;
        }

        public override Vector2 InitialSize => new Vector2(512, 256);

        public override void DoWindowContents(Rect inRect)
        {
            var header = new Rect(inRect.x, inRect.y, inRect.width, Text.LineHeight);
            var body = new Rect(
                inRect.x,
                inRect.y + Text.LineHeight * 3,
                inRect.width,
                inRect.height - Text.LineHeight * 1.5f
            );
            var midpoint = inRect.width / 2;
            var showText = "TKUtils.Windows.Config.Buttons.DontShowAgain".Translate();
            var buttonSize = Text.CalcSize(showText).x * 1.5f;

            var old = Text.Anchor;
            var old2 = Text.Font;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(header, "TKUtils.Windows.Config.Notices.Header".Translate());

            Text.Anchor = old;
            Text.Font = old2;
            Widgets.Label(body, $"TKUtils.Windows.Config.Notices.{def}".Translate());

            Text.Anchor = old;
            if (Widgets.ButtonText(
                new Rect(midpoint - buttonSize - 5f, inRect.height - Text.LineHeight, buttonSize, Text.LineHeight),
                "OK".Translate()
            ))
            {
                Close();
            }

            if (Widgets.ButtonText(
                new Rect(midpoint + 5f, inRect.height - Text.LineHeight, buttonSize, Text.LineHeight),
                showText
            ))
            {
                try
                {
                    switch (def)
                    {
                        case "AddTrait":
                            TkSettings.UtilsNoticeAdd = false;
                            LoadedModManager.GetMod<TkUtils>().GetSettings<TkSettings>().Write();
                            Close();
                            break;
                        case "RemoveTrait":
                            TkSettings.UtilsNoticeRemove = false;
                            LoadedModManager.GetMod<TkUtils>().GetSettings<TkSettings>().Write();
                            Close();
                            break;
                        case "BuyPawn":
                            TkSettings.UtilsNoticePawn = false;
                            LoadedModManager.GetMod<TkUtils>().GetSettings<TkSettings>().Write();
                            Close();
                            break;
                    }
                }
                catch (Exception e)
                {
                    TkLogger.Error("Could not save notice preferences!", e);
                }
            }
        }
    }
}

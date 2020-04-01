using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;
using RimWorld;
using SirRandoo.ToolkitUtils.IncidentHelpers;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Incidents;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window))]
    public class Window_PostOpen
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Window.PostOpen))]
        public static void PostOpen(Window __instance)
        {
            if (!(__instance is StoreIncidentEditor))
            {
                return;
            }

            var editor = (StoreIncidentEditor) __instance;

            if (!editor.variableIncident)
            {
                return;
            }

            if (editor.storeIncidentVariables.defName.Equals("AddTrait")
                || editor.storeIncidentVariables.defName.Equals("RemoveTrait")
                || editor.storeIncidentVariables.defName.Equals("BuyPawn"))
            {
                Find.WindowStack.Add(new NoticeWindow(editor.storeIncidentVariables.defName));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Window.PreClose))]
        public static void PreClose(Window __instance)
        {
            if (!(__instance is StoreIncidentEditor))
            {
                return;
            }

            var editor = (StoreIncidentEditor) __instance;

            if (!editor.variableIncident)
            {
                return;
            }

            if (editor.storeIncidentVariables.cost <= 0)
            {
                return;
            }

            switch (editor.storeIncidentVariables.defName)
            {
                case "AddTrait":
                    foreach (var trait in TkUtils.ShopExpansion.Traits)
                    {
                        trait.AddPrice = editor.storeIncidentVariables.cost;
                    }

                    editor.storeIncidentVariables.cost = 0;
                    return;
                case "RemoveTrait":
                    foreach (var trait in TkUtils.ShopExpansion.Traits)
                    {
                        trait.RemovePrice = editor.storeIncidentVariables.cost;
                    }

                    editor.storeIncidentVariables.cost = 0;
                    return;
                case "BuyPawn":
                    foreach (var race in TkUtils.ShopExpansion.Races)
                    {
                        race.Price = editor.storeIncidentVariables.cost;
                    }

                    editor.storeIncidentVariables.cost = 0;
                    return;
            }
        }
    }

    public class NoticeWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(512, 256);
        private readonly string def;

        public NoticeWindow(string def)
        {
            this.def = def;
        }

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
                switch (def)
                {
                    case "AddTrait":
                        TkSettings.UtilsNoticeAdd = false;
                        Close();
                        break;
                    case "RemoveTrait":
                        TkSettings.UtilsNoticeRemove = false;
                        Close();
                        break;
                    case "BuyPawn":
                        TkSettings.UtilsNoticePawn = false;
                        Close();
                        break;
                }
            }
        }
    }
}

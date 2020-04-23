using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Window))]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class WindowPatches
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

            if (editor.storeIncidentVariables.defName.Equals("AddTrait") && TkSettings.UtilsNoticeAdd
                || editor.storeIncidentVariables.defName.Equals("RemoveTrait") && TkSettings.UtilsNoticeRemove
                || editor.storeIncidentVariables.defName.Equals("BuyPawn") && TkSettings.UtilsNoticePawn)
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

            if (editor.storeIncidentVariables.cost <= 1)
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

                    editor.storeIncidentVariables.cost = 1;
                    return;
                case "RemoveTrait":
                    foreach (var trait in TkUtils.ShopExpansion.Traits)
                    {
                        trait.RemovePrice = editor.storeIncidentVariables.cost;
                    }

                    editor.storeIncidentVariables.cost = 1;
                    return;
                case "BuyPawn":
                    foreach (var race in TkUtils.ShopExpansion.Races)
                    {
                        race.Price = editor.storeIncidentVariables.cost;
                    }

                    editor.storeIncidentVariables.cost = 1;
                    return;
            }
        }
    }
}

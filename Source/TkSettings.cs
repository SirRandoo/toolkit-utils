﻿using System;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum Categories { General, CommandTweaks, PawnCommands }

    public enum LeaveMethods { Thanos, MentalBreak }

    public class TkSettings : ModSettings
    {
        public static bool DecorateUtils;
        public static bool Emojis = true;
        public static bool FilterWorkPriorities;
        public static bool Sexuality = true;
        public static bool ShowApparel;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool SortWorkPriorities;
        public static bool Race = true;
        public static bool TempInGear;
        public static bool DropInventory;
        public static bool JsonShop;
        public static string LeaveMethod = ToolkitUtils.LeaveMethods.MentalBreak.ToString();
        public static int LookupLimit = 10;
        public static bool VersionedModList;
        public static bool ShowCoinRate = true;

        public static bool UtilsNoticeAdd = true;
        public static bool UtilsNoticeRemove = true;
        public static bool UtilsNoticePawn = true;

        private static Categories _category = Categories.General;
        private static readonly string[] LeaveMethods = Enum.GetNames(typeof(LeaveMethods));

        public static void DoWindowContents(Rect inRect)
        {
            var catRect = new Rect(inRect.x, inRect.y, inRect.width * 0.25f, inRect.height);
            var setRect = new Rect(
                catRect.width + 10f,
                inRect.y,
                inRect.width - catRect.width - 10f,
                inRect.height
            );

            var listing = new Listing_Standard();
            Widgets.DrawMenuSection(catRect);

            listing.Begin(catRect);
            var generalRect = listing.Label("TKUtils.SettingGroups.General".Translate());
            var commandTweakRect = listing.Label("TKUtils.SettingGroups.CommandTweaks".Translate());
            var pawnRect = listing.Label("TKUtils.SettingGroups.PawnCommands".Translate());

            if (Widgets.ButtonInvisible(generalRect) || _category == Categories.General)
            {
                _category = Categories.General;

                Widgets.DrawHighlight(generalRect);
            }

            if (Widgets.ButtonInvisible(commandTweakRect) || _category == Categories.CommandTweaks)
            {
                _category = Categories.CommandTweaks;

                Widgets.DrawHighlight(commandTweakRect);
            }

            if (Widgets.ButtonInvisible(pawnRect) || _category == Categories.PawnCommands)
            {
                _category = Categories.PawnCommands;

                Widgets.DrawHighlight(pawnRect);
            }

            listing.End();

            switch (_category)
            {
                case Categories.General:
                    DrawGeneral(setRect);
                    break;
                case Categories.CommandTweaks:
                    DrawCommandTweaks(setRect);
                    break;
                case Categories.PawnCommands:
                    DrawPawnCommands(setRect);
                    break;
            }
        }

        private static void DrawGeneral(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);
            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.VersionedModList.Label".Translate(),
                ref VersionedModList,
                "TKUtils.SettingGroups.General.VersionedModList.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.Emojis.Label".Translate(),
                ref Emojis,
                "TKUtils.SettingGroups.General.Emojis.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.DecorateUtils.Label".Translate(),
                ref DecorateUtils,
                "TKUtils.SettingGroups.General.DecorateUtils.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.Sexuality.Label".Translate(),
                ref Sexuality,
                "TKUtils.SettingGroups.General.Sexuality.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.Race.Label".Translate(),
                ref Race,
                "TKUtils.SettingGroups.General.Race.Tooltip".Translate()
            );

            var line = listing.GetRect(Text.LineHeight);
            var (labelRect, entryRect) = line.ToForm();
            var buffer = LookupLimit.ToString();

            Widgets.Label(labelRect, "TKUtils.SettingGroups.General.LookupLimit.Label".Translate());
            Widgets.TextFieldNumeric(entryRect, ref LookupLimit, ref buffer);

            if (Mouse.IsOver(line))
            {
                Widgets.DrawHighlightIfMouseover(line);
                TooltipHandler.TipRegion(line, "TKUtils.SettingGroups.General.LookupLimit.Tooltip".Translate());
            }

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.JsonStore.Label".Translate(),
                ref JsonShop,
                "TKUtils.SettingGroups.General.JsonStore.Tooltip".Translate()
            );

            listing.End();
        }

        private static void DrawCommandTweaks(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.CommandTweaks.CoinRate.Label".Translate(),
                ref ShowCoinRate,
                "TKUtils.SettingGroups.CommandTweaks.CoinRate.Tooltip".Translate()
            );

            listing.End();
        }

        private static void DrawPawnCommands(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);
            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.TempInGear.Label".Translate(),
                ref TempInGear,
                "TKUtils.SettingGroups.PawnCommands.TempInGear.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.ShowApparel.Label".Translate(),
                ref ShowApparel,
                "TKUtils.SettingGroups.PawnCommands.ShowApparel.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.ShowArmor.Label".Translate(),
                ref ShowArmor,
                "TKUtils.SettingGroups.PawnCommands.ShowArmor.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.ShowWeapon.Label".Translate(),
                ref ShowWeapon,
                "TKUtils.SettingGroups.PawnCommands.ShowWeapon.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.ShowSurgeries.Label".Translate(),
                ref ShowSurgeries,
                "TKUtils.SettingGroups.PawnCommands.ShowSurgeries.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.SortWork.Label".Translate(),
                ref SortWorkPriorities,
                "TKUtils.SettingGroups.PawnCommands.SortWork.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.PawnCommands.FilterWork.Label".Translate(),
                ref FilterWorkPriorities,
                "TKUtils.SettingGroups.PawnCommands.FilterWork.Tooltip".Translate()
            );

            listing.Gap();

            var (leaveLabelRect, leaveRect) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(leaveLabelRect, "TKUtils.SettingGroups.PawnCommands.LeaveMethod.Label".Translate());

            if (Widgets.ButtonText(leaveRect, LeaveMethod))
            {
                Find.WindowStack.Add(
                    new FloatMenu(LeaveMethods.Select(o => new FloatMenuOption(o, () => LeaveMethod = o)).ToList())
                );
            }

            if (!LeaveMethod.EqualsIgnoreCase("Thanos"))
            {
                listing.CheckboxLabeled(
                    "TKUtils.SettingGroups.PawnCommands.LeaveGear.Label".Translate(),
                    ref DropInventory,
                    "TKUtils.SettingGroups.PawnCommands.LeaveGear.Tooltip".Translate()
                );
            }

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Emojis, "emojis", true);
            Scribe_Values.Look(ref DecorateUtils, "decorateUtils");
            Scribe_Values.Look(ref VersionedModList, "versionedModList");

            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowApparel, "apparel");
            Scribe_Values.Look(ref TempInGear, "tempInGear");
            Scribe_Values.Look(ref Sexuality, "sexuality", true);
            Scribe_Values.Look(ref SortWorkPriorities, "sortWork");
            Scribe_Values.Look(ref FilterWorkPriorities, "filterWork");

            Scribe_Values.Look(ref LookupLimit, "lookupLimit", 10);
            Scribe_Values.Look(ref Race, "race", true);
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", ToolkitUtils.LeaveMethods.MentalBreak.ToString());
            Scribe_Values.Look(ref DropInventory, "dropInventory");

            Scribe_Values.Look(ref JsonShop, "shopJson");
            Scribe_Values.Look(ref UtilsNoticeAdd, "utilsNoticeAdd", true);
            Scribe_Values.Look(ref UtilsNoticeRemove, "utilsNoticeRemove", true);
            Scribe_Values.Look(ref UtilsNoticePawn, "utilsNoticePawn", true);
        }
    }
}

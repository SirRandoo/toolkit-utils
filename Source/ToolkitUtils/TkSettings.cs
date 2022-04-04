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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonLib.Helpers;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum LeaveMethod { Thanos, MentalBreak }

    public enum DumpStyle { SingleFile, MultiFile }

    public enum UserCoinType { Broadcaster, Subscriber, Vip, Moderator, None }

    [StaticConstructorOnStartup]
    public class TkSettings : ModSettings
    {
        public static bool TrueNeutral;
        public static bool ForceFullItem;
        public static bool Commands = true;
        public static string Prefix = "!";
        public static string BuyPrefix = "$";
        public static bool ToolkitStyleCommands = true;
        public static bool MinifyData;
        public static bool DecorateMods;
        public static bool Emojis = true;
        public static bool FilterWorkPriorities;
        public static bool ShowApparel;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool SortWorkPriorities;
        public static bool PurchasePawnKinds = true;
        public static bool TempInGear;
        public static bool DropInventory;
        public static string BroadcasterCoinType = nameof(UserCoinType.Broadcaster);
        public static string LeaveMethod = nameof(ToolkitUtils.LeaveMethod.MentalBreak);
        public static string DumpStyle = nameof(ToolkitUtils.DumpStyle.SingleFile);
        public static int LookupLimit = 10;
        public static bool VersionedModList;
        public static bool ShowCoinRate = true;
        public static bool HairColor = true;
        public static int OpinionMinimum;
        public static bool AsapPurchases;
        public static int StoreBuildRate = 60;
        public static bool StoreState = true; // Used by !togglestore to temporarily disable Twitch Toolkit's store.
        public static bool Offload;
        public static bool BuyItemBalance;
        public static bool ClassChanges;
        public static bool ResetClass;
        public static bool VisualExceptions;
        public static bool MinimalRelations = true;
        public static bool GatewayPuff = true;

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();

        internal static List<FloatMenuOption> LeaveMenuOptions;
        private static List<FloatMenuOption> _dumpStyleOptions;
        private static List<FloatMenuOption> _coinUserTypeOptions;

        private static WorkTypeDef[] _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();

        private static Vector2 _workScrollPos = Vector2.zero;
        private static Vector2 _commandTweaksPos = Vector2.zero;
        private static Vector2 _dataScrollPos = Vector2.zero;
        public static bool EasterEggs = true;
        public static bool CommandRouter = true;
        public static bool TransparentColors;

        private static readonly TabWorker TabWorker = new TabWorker();

        static TkSettings()
        {
            TabWorker.AddTab(new TabItem { Label = "" });
        }

        public static void DoWindowContents(Rect inRect)
        {
            // A fix for how some windows embed Utils' settings.
            inRect.height = inRect.height > 620f ? 620f : inRect.height;
            ValidateEnumOptions();

            Color cache = GUI.color;
            GUI.color = Color.grey;
            Widgets.DrawLightHighlight(inRect);
            GUI.color = cache;

            GUI.BeginGroup(inRect);
            var tabBarRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
            var tabPanelRect = new Rect(0f, tabBarRect.height, inRect.width, inRect.height - tabBarRect.height);
            Rect contentRect = tabPanelRect.ContractedBy(20f);


            GUI.BeginGroup(tabBarRect);
            GUI.EndGroup();

            Widgets.DrawLightHighlight(tabPanelRect);
            GUI.BeginGroup(contentRect);

            GUI.EndGroup();


            GUI.EndGroup();
        }

        [UsedImplicitly]
        private static void DrawModCompatTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.ModGroupHeader("Humanoid Alien Races", 839005762, false);
            listing.CheckboxLabeled("TKUtils.HAR.PawnKinds.Label".TranslateSimple(), ref PurchasePawnKinds);
            listing.DrawDescription("TKUtils.HAR.PawnKinds.Description".TranslateSimple());

            listing.ModGroupHeader("A RimWorld of Magic", 1201382956);
            listing.CheckboxLabeled("TKUtils.TMagic.Classes.Label".TranslateSimple(), ref ClassChanges);
            listing.DrawDescription("TKUtils.TMagic.Classes.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            if (ClassChanges)
            {
                listing.CheckboxLabeled("TKUtils.TMagic.ResetClass.Label".TranslateSimple(), ref ResetClass);
                listing.DrawDescription("TKUtils.TMagic.ResetClass.Description".TranslateSimple());
                listing.DrawExperimentalNotice();
            }

            listing.ModGroupHeader("Visual Exceptions", 2538411704);
            listing.CheckboxLabeled("TKUtils.VisualExceptions.SendErrors.Label".TranslateSimple(), ref VisualExceptions);
            listing.DrawDescription("TKUtils.VisualExceptions.SendErrors.Description".TranslateSimple());
            listing.DrawExperimentalNotice();

            listing.End();
        }

        [UsedImplicitly]
        private static void DrawDataTab(Rect canvas)
        {
            var view = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 32f);
            var listing = new Listing_Standard();
            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _dataScrollPos, view);
            listing.Begin(view);

            listing.GroupHeader("TKUtils.Data.Files".Localize(), false);

            (Rect dumpLabel, Rect dumpBtn) = listing.Split();
            UiHelper.Label(dumpLabel, "TKUtils.DumpStyle.Label".Translate());
            listing.DrawDescription("TKUtils.DumpStyle.Description".Translate());

            if (Widgets.ButtonText(dumpBtn, $"TKUtils.DumpStyle.{DumpStyle}".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(_dumpStyleOptions));
            }

            listing.CheckboxLabeled("TKUtils.MinifyData.Label".Translate(), ref MinifyData);
            listing.DrawDescription("TKUtils.MinifyData.Description".Translate());

            listing.CheckboxLabeled("TKUtils.OffloadShop.Label".Translate(), ref Offload);
            listing.DrawDescription("TKUtils.OffloadShop.Description".Translate());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.DoPurchasesAsap.Label".Translate(), ref AsapPurchases);
            listing.DrawDescription("TKUtils.DoPurchasesAsap.Description".Translate());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.TrueNeutral.Label".Translate(), ref TrueNeutral);
            listing.DrawDescription("TKUtils.TrueNeutral.Description".Translate());
            listing.DrawExperimentalNotice();

            (Rect coinTypeLabel, Rect coinTypeField) = listing.Split();
            UiHelper.Label(coinTypeLabel, "TKUtils.BroadcasterUserType.Label".Translate());
            listing.DrawDescription("TKUtils.BroadcasterUserType.Description".Translate());
            listing.DrawExperimentalNotice();

            if (Widgets.ButtonText(coinTypeField, $"TKUtils.BroadcasterUserType.{BroadcasterCoinType}".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(_coinUserTypeOptions));
            }


            listing.GroupHeader("TKUtils.Data.LazyProcess".Translate());

            (Rect storeLabel, Rect storeField) = listing.Split();
            UiHelper.Label(storeLabel, "TKUtils.StoreRate.Label".Translate());
            listing.DrawDescription("TKUtils.StoreRate.Description".Translate());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

            listing.End();
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private static void ValidateEnumOptions()
        {
            _dumpStyleOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.DumpStyle.SingleFile".Translate(), () => DumpStyle = nameof(ToolkitUtils.DumpStyle.SingleFile)),
                new FloatMenuOption("TKUtils.DumpStyle.MultiFile".Translate(), () => DumpStyle = nameof(ToolkitUtils.DumpStyle.MultiFile))
            };

            LeaveMenuOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.Abandon.Method.Thanos".Translate(), () => LeaveMethod = nameof(ToolkitUtils.LeaveMethod.Thanos)),
                new FloatMenuOption("TKUtils.Abandon.Method.MentalBreak".Translate(), () => LeaveMethod = nameof(ToolkitUtils.LeaveMethod.MentalBreak))
            };

            _coinUserTypeOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.BroadcasterUserType.Broadcaster".Translate(), () => BroadcasterCoinType = nameof(UserCoinType.Broadcaster)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Subscriber".Translate(), () => BroadcasterCoinType = nameof(UserCoinType.Subscriber)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Vip".Translate(), () => BroadcasterCoinType = nameof(UserCoinType.Vip)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.Moderator".Translate(), () => BroadcasterCoinType = nameof(UserCoinType.Moderator)),
                new FloatMenuOption("TKUtils.BroadcasterUserType.None".Translate(), () => BroadcasterCoinType = nameof(UserCoinType.None))
            };
        }

        [UsedImplicitly]
        private static void DrawGeneralTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.GroupHeader("TKUtils.General.Emojis".Translate(), false);
            listing.CheckboxLabeled("TKUtils.Emojis.Label".Translate(), ref Emojis);
            listing.DrawDescription("TKUtils.Emojis.Description".Translate());


            listing.GroupHeader("TKUtils.General.Viewer".Translate());
            listing.CheckboxLabeled("TKUtils.HairColor.Label".Translate(), ref HairColor);
            listing.DrawDescription("TKUtils.HairColor.Description".Translate());

            listing.GroupHeader("TKUtils.General.Gateway".Translate());
            listing.CheckboxLabeled("TKUtils.GatewayPuff.Label".Translate(), ref GatewayPuff);
            listing.DrawDescription("TKUtils.GatewayPuff.Description".Translate());

            listing.GroupHeader("TKUtils.General.Basket".Translate());
            listing.CheckboxLabeled("TKUtils.EasterEggs.Label".Translate(), ref EasterEggs);
            listing.DrawDescription("TKUtils.EasterEggs.Description".Translate());

            listing.End();
        }

        [UsedImplicitly]
        private static void DrawCommandTweaksTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 48f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.GroupHeader("TKUtils.CommandTweaks.Balance".Translate(), false);
            listing.CheckboxLabeled("TKUtils.CoinRate.Label".Translate(), ref ShowCoinRate);
            listing.DrawDescription("TKUtils.CoinRate.Description".Translate());


            listing.GroupHeader("TKUtils.CommandTweaks.Handler".Translate());

            if (Commands)
            {
                (Rect prefixLabel, Rect prefixField) = listing.Split();
                UiHelper.Label(prefixLabel, "TKUtils.CommandPrefix.Label".Translate());
                listing.DrawDescription("TKUtils.CommandPrefix.Description".Translate());
                Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, Prefix));

                (Rect buyPrefixLabel, Rect buyPrefixField) = listing.Split();
                UiHelper.Label(buyPrefixLabel, "TKUtils.PurchasePrefix.Label".Translate());
                listing.DrawDescription("TKUtils.PurchasePrefix.Description".Translate());
                BuyPrefix = CommandHelper.ValidatePrefix(Widgets.TextField(buyPrefixField, BuyPrefix));
            }

            listing.CheckboxLabeled("TKUtils.CommandParser.Label".Translate(), ref Commands);
            listing.DrawDescription("TKUtils.CommandParser.Description".Translate());

            if (Commands)
            {
                listing.CheckboxLabeled("TKUtils.ToolkitStyleCommands.Label".Translate(), ref ToolkitStyleCommands);
                listing.DrawDescription("TKUtils.ToolkitStyleCommands.Description".Translate());
            }

            listing.CheckboxLabeled("TKUtils.CommandRouter.Label".Translate(), ref CommandRouter);
            listing.DrawDescription("TKUtils.CommandRouter.Description".Translate());
            listing.DrawExperimentalNotice();


            listing.GroupHeader("TKUtils.CommandTweaks.InstalledMods".Translate());
            listing.CheckboxLabeled("TKUtils.DecorateUtils.Label".Translate(), ref DecorateMods);
            listing.DrawDescription("TKUtils.DecorateUtils.Description".Translate());

            listing.CheckboxLabeled("TKUtils.VersionedModList.Label".Translate(), ref VersionedModList);
            listing.DrawDescription("TKUtils.VersionedModList.Description".Translate());


            listing.GroupHeader("TKUtils.CommandTweaks.BuyItem".Translate());
            listing.CheckboxLabeled("TKUtils.BuyItemBalance.Label".Translate(), ref BuyItemBalance);
            listing.DrawDescription("TKUtils.BuyItemBalance.Description".Translate());
            listing.CheckboxLabeled("TKUtils.BuyItemFullSyntax.Label".Translate(), ref ForceFullItem);
            listing.DrawDescription("TKUtils.BuyItemFullSyntax.Description".Translate());


            listing.GroupHeader("TKUtils.CommandTweaks.Lookup".Translate());

            (Rect lookupLimitLabel, Rect lookupLimitField) = listing.Split();
            var buffer = LookupLimit.ToString();

            UiHelper.Label(lookupLimitLabel, "TKUtils.LookupLimit.Label".Translate());
            Widgets.TextFieldNumeric(lookupLimitField, ref LookupLimit, ref buffer);
            listing.DrawDescription("TKUtils.LookupLimit.Description".Translate());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        [UsedImplicitly]
        private static void DrawPawnCommandsTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 40f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.GroupHeader("TKUtils.PawnCommands.Abandon".Translate(), false);

            (Rect leaveLabelRect, Rect leaveRect) = listing.Split();
            UiHelper.Label(leaveLabelRect, "TKUtils.Abandon.Method.Label".Translate());
            listing.DrawDescription("TKUtils.Abandon.Method.Description".Translate());

            if (Widgets.ButtonText(leaveRect, $"TKUtils.Abandon.Method.{LeaveMethod}".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(LeaveMenuOptions));
            }

            if (!LeaveMethod.EqualsIgnoreCase(nameof(ToolkitUtils.LeaveMethod.Thanos)))
            {
                listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".Translate(), ref DropInventory);
                listing.DrawDescription("TKUtils.Abandon.Gear.Description".Translate());
            }

            listing.GroupHeader("TKUtils.PawnCommands.Gear".Translate());
            listing.CheckboxLabeled("TKUtils.PawnGear.Temperature.Label".Translate(), ref TempInGear);
            listing.DrawDescription("TKUtils.PawnGear.Temperature.Description".Translate());
            listing.CheckboxLabeled("TKUtils.PawnGear.Apparel.Label".Translate(), ref ShowApparel);
            listing.DrawDescription("TKUtils.PawnGear.Apparel.Description".Translate());
            listing.CheckboxLabeled("TKUtils.PawnGear.Armor.Label".Translate(), ref ShowArmor);
            listing.DrawDescription("TKUtils.PawnGear.Armor.Description".Translate());
            listing.CheckboxLabeled("TKUtils.PawnGear.Weapon.Label".Translate(), ref ShowWeapon);
            listing.DrawDescription("TKUtils.PawnGear.Weapon.Description".Translate());


            listing.GroupHeader("TKUtils.PawnCommands.Health".Translate());
            listing.CheckboxLabeled("TKUtils.PawnHealth.Surgeries.Label".Translate(), ref ShowSurgeries);
            listing.DrawDescription("TKUtils.PawnHealth.Surgeries.Description".Translate());


            listing.GroupHeader("TKUtils.PawnCommands.Relations".Translate());
            (Rect opinionLabel, Rect opinionField) = listing.Split();
            var buffer = OpinionMinimum.ToString();

            if (!MinimalRelations)
            {
                UiHelper.Label(opinionLabel, "TKUtils.PawnRelations.OpinionThreshold.Label".Translate());
                Widgets.TextFieldNumeric(opinionField, ref OpinionMinimum, ref buffer);
                listing.DrawDescription("TKUtils.PawnRelations.OpinionThreshold.Description".Translate());
            }

            listing.CheckboxLabeled("TKUtils.PawnRelations.MinimalRelations.Label".Translate(), ref MinimalRelations);
            listing.DrawDescription("TKUtils.PawnRelations.MinimalRelations.Description".Translate());

            listing.GroupHeader("TKUtils.PawnCommands.Work".Translate());
            listing.CheckboxLabeled("TKUtils.PawnWork.Sort.Label".Translate(), ref SortWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Sort.Description".Translate());
            listing.CheckboxLabeled("TKUtils.PawnWork.Filter.Label".Translate(), ref FilterWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Filter.Description".Translate());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        [UsedImplicitly]
        private static void DrawPawnWorkTab(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _workTypeDefs.Length * Text.LineHeight);

            Widgets.BeginScrollView(content, ref _workScrollPos, view);
            listing.Begin(view);

            for (var index = 0; index < _workTypeDefs.Length; index++)
            {
                WorkTypeDef workType = _workTypeDefs[index];
                WorkSetting workSetting = WorkSettings.FirstOrDefault(w => w.WorkTypeDef.EqualsIgnoreCase(workType.defName));

                if (workSetting == null)
                {
                    workSetting = new WorkSetting { Enabled = true, WorkTypeDef = workType.defName };

                    WorkSettings.Add(workSetting);
                }

                Rect line = listing.GetRect(Text.LineHeight);

                if (!line.IsVisible(content, _workScrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.CheckboxLabeled(line, workSetting.WorkTypeDef, ref workSetting.Enabled);

                Widgets.DrawHighlightIfMouseover(line);
            }

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Commands, "commands", true);
            Scribe_Values.Look(ref Prefix, "prefix", "!");
            Scribe_Values.Look(ref BuyPrefix, "buyPrefix", "$");
            Scribe_Values.Look(ref ToolkitStyleCommands, "toolkitStyleCommands", true);
            Scribe_Values.Look(ref DecorateMods, "decorateUtils");
            Scribe_Values.Look(ref ForceFullItem, "forceFullItemSyntax");
            Scribe_Values.Look(ref Emojis, "emojis", true);
            Scribe_Values.Look(ref FilterWorkPriorities, "filterWork");
            Scribe_Values.Look(ref ShowApparel, "apparel");
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowWeapon, "weapon", true);
            Scribe_Values.Look(ref SortWorkPriorities, "sortWork");
            Scribe_Values.Look(ref PurchasePawnKinds, "race", true);
            Scribe_Values.Look(ref TempInGear, "tempInGear");
            Scribe_Values.Look(ref DropInventory, "dropInventory");
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", nameof(ToolkitUtils.LeaveMethod.MentalBreak));
            Scribe_Values.Look(ref DumpStyle, "dumpStyle", nameof(ToolkitUtils.DumpStyle.SingleFile));
            Scribe_Values.Look(ref BroadcasterCoinType, "broadcasterCoinType", nameof(UserCoinType.Broadcaster));
            Scribe_Values.Look(ref LookupLimit, "lookupLimit", 10);
            Scribe_Values.Look(ref AsapPurchases, "asapPurchases");
            Scribe_Values.Look(ref VersionedModList, "versionedModList");
            Scribe_Values.Look(ref ShowCoinRate, "balanceCoinRate", true);
            Scribe_Values.Look(ref TrueNeutral, "trueNeutral");
            Scribe_Values.Look(ref HairColor, "hairColor", true);
            Scribe_Values.Look(ref OpinionMinimum, "minimumOpinion", 20);
            Scribe_Values.Look(ref StoreBuildRate, "storeBuildRate", 60);
            Scribe_Values.Look(ref Offload, "offload");
            Scribe_Values.Look(ref BuyItemBalance, "buyItemBalance");
            Scribe_Values.Look(ref ClassChanges, "classChanges");
            Scribe_Values.Look(ref ResetClass, "resetClass");
            Scribe_Values.Look(ref MinimalRelations, "minimalRelations", true);
            Scribe_Values.Look(ref GatewayPuff, "gatewayPuff", true);
            Scribe_Values.Look(ref EasterEggs, "easterEggs", true);
            Scribe_Values.Look(ref TransparentColors, "allowTransparentColors");

            Scribe_Collections.Look(ref WorkSettings, "workSettings", LookMode.Deep);
        }

        internal static void ValidateDynamicSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }


            WorkSettings ??= new List<WorkSetting>();


            foreach (WorkTypeDef workType in _workTypeDefs.Where(d => !WorkSettings.Any(s => s.WorkTypeDef.EqualsIgnoreCase(d.defName))))
            {
                WorkSettings.Add(new WorkSetting { Enabled = true, WorkTypeDef = workType.defName });
            }
        }

        public class WorkSetting : IExposable
        {
            // ReSharper disable once StringLiteralTypo
            [Description("Whether or not the work priority will be shown in !mypawnwork")]
            public bool Enabled;

            [Description("The def name of the work type instance.")] public string WorkTypeDef;

            public void ExposeData()
            {
                Scribe_Values.Look(ref WorkTypeDef, "defName");
                Scribe_Values.Look(ref Enabled, "enabled", true);
            }
        }
    }
}

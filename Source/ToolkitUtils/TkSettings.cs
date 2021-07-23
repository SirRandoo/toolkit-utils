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
using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum Categories
    {
        General,
        Data,
        CommandTweaks,
        PawnCommands,
        PawnWork,
        ModCompat
    }

    public enum LeaveMethods { Thanos, MentalBreak }

    public enum DumpStyles { SingleFile, MultiFile }

    public enum UserCoinType { Broadcaster, Subscriber, Vip, Moderator, None }

    [StaticConstructorOnStartup]
    public class TkSettings : ModSettings
    {
        private static string _modVersion;
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
        public static string LeaveMethod = nameof(LeaveMethods.MentalBreak);
        public static string DumpStyle = nameof(DumpStyles.SingleFile);
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
        public static bool Puppeteer;
        public static bool MinimalRelations = true;
        public static bool GatewayPuff = true;

        internal static bool DebuggingIncidents;

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();

        private static Categories _category = Categories.General;
        internal static List<FloatMenuOption> LeaveMenuOptions;
        private static List<FloatMenuOption> _dumpStyleOptions;
        private static List<FloatMenuOption> _coinUserTypeOptions;
        private static TabEntry[] _tabEntries;

        private static WorkTypeDef[] _workTypeDefs;

        private static Vector2 _workScrollPos = Vector2.zero;
        private static Vector2 _commandTweaksPos = Vector2.zero;
        private static Vector2 _dataScrollPos = Vector2.zero;

        static TkSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }
        }

        public static void DoWindowContents(Rect inRect)
        {
            // A fix for how some windows embed Utils' settings.
            inRect.height = inRect.height > 620f ? 620f : inRect.height;
            ValidateTabs();
            ValidateEnumOptions();

            Color cache = GUI.color;
            GUI.color = Color.grey;
            Widgets.DrawLightHighlight(inRect);
            GUI.color = cache;

            GUI.BeginGroup(inRect);
            var tabBarRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
            var tabPanelRect = new Rect(0f, tabBarRect.height, inRect.width, inRect.height - tabBarRect.height);
            Rect contentRect = tabPanelRect.ContractedBy(20f);
            var trueContentRect = new Rect(0f, 0f, contentRect.width, contentRect.height);


            GUI.BeginGroup(tabBarRect);
            DrawTabs(tabBarRect);
            GUI.EndGroup();

            DrawTabPanelLine(tabPanelRect);
            Widgets.DrawLightHighlight(tabPanelRect);
            GUI.BeginGroup(contentRect);
            switch (_category)
            {
                case Categories.General:
                    DrawGeneralTab(trueContentRect);
                    break;
                case Categories.Data:
                    DrawDataTab(trueContentRect);
                    break;
                case Categories.CommandTweaks:
                    DrawCommandTweaksTab(trueContentRect);
                    break;
                case Categories.PawnCommands:
                    DrawPawnCommandsTab(trueContentRect);
                    break;
                case Categories.PawnWork:
                    DrawPawnWorkTab(trueContentRect);
                    break;
                case Categories.ModCompat:
                    DrawModCompatTab(trueContentRect);
                    break;
            }

            GUI.EndGroup();


            GUI.EndGroup();
        }

        private static void DrawModCompatTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.DrawModGroupHeader("Humanoid Alien Races", 839005762, false);
            listing.CheckboxLabeled("TKUtils.HAR.PawnKinds.Label".Localize(), ref PurchasePawnKinds);
            listing.DrawDescription("TKUtils.HAR.PawnKinds.Description".Localize());

            listing.DrawModGroupHeader("A RimWorld of Magic", 1201382956);
            listing.CheckboxLabeled("TKUtils.TMagic.Classes.Label".Localize(), ref ClassChanges);
            listing.DrawDescription("TKUtils.TMagic.Classes.Description".Localize());
            listing.DrawExperimentalNotice();

            if (ClassChanges)
            {
                listing.CheckboxLabeled("TKUtils.TMagic.ResetClass.Label".Localize(), ref ResetClass);
                listing.DrawDescription("TKUtils.TMagic.ResetClass.Description".Localize());
                listing.DrawExperimentalNotice();
            }

            listing.DrawModGroupHeader("Puppeteer", 2057192142);
            listing.CheckboxLabeled("TKUtils.Puppeteer.Redirect.Label".Localize(), ref Puppeteer);
            listing.DrawDescription("TKUtils.Puppeteer.Redirect.Description".Localize());
            listing.DrawExperimentalNotice();

            listing.End();
        }

        private static void DrawDataTab(Rect canvas)
        {
            var view = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 32f);
            var listing = new Listing_Standard();
            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _dataScrollPos, view);
            listing.Begin(view);

            listing.DrawGroupHeader("TKUtils.Data.Files".Localize(), false);

            (Rect dumpLabel, Rect dumpBtn) = listing.GetRect(Text.LineHeight).ToForm();
            SettingsHelper.DrawLabel(dumpLabel, "TKUtils.DumpStyle.Label".Localize());
            listing.DrawDescription("TKUtils.DumpStyle.Description".Localize());

            if (Widgets.ButtonText(dumpBtn, $"TKUtils.DumpStyle.{DumpStyle}".Localize()))
            {
                Find.WindowStack.Add(new FloatMenu(_dumpStyleOptions));
            }

            listing.CheckboxLabeled("TKUtils.MinifyData.Label".Localize(), ref MinifyData);
            listing.DrawDescription("TKUtils.MinifyData.Description".Localize());

            listing.CheckboxLabeled("TKUtils.OffloadShop.Label".Localize(), ref Offload);
            listing.DrawDescription("TKUtils.OffloadShop.Description".Localize());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.DoPurchasesAsap.Label".Localize(), ref AsapPurchases);
            listing.DrawDescription("TKUtils.DoPurchasesAsap.Description".Localize());
            listing.DrawExperimentalNotice();

            listing.CheckboxLabeled("TKUtils.TrueNeutral.Label".Localize(), ref TrueNeutral);
            listing.DrawDescription("TKUtils.TrueNeutral.Description".Localize());
            listing.DrawExperimentalNotice();

            (Rect coinTypeLabel, Rect coinTypeField) = listing.GetRect(Text.LineHeight).ToForm();
            SettingsHelper.DrawLabel(coinTypeLabel, "TKUtils.BroadcasterUserType.Label".Localize());
            listing.DrawDescription("TKUtils.BroadcasterUserType.Description".Localize());
            listing.DrawExperimentalNotice();

            if (Widgets.ButtonText(coinTypeField, $"TKUtils.BroadcasterUserType.{BroadcasterCoinType}".Localize()))
            {
                Find.WindowStack.Add(new FloatMenu(_coinUserTypeOptions));
            }


            listing.DrawGroupHeader("TKUtils.Data.LazyProcess".Localize());

            (Rect storeLabel, Rect storeField) = listing.GetRect(Text.LineHeight).ToForm();
            SettingsHelper.DrawLabel(storeLabel, "TKUtils.StoreRate.Label".Localize());
            listing.DrawDescription("TKUtils.StoreRate.Description".Localize());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

            listing.End();
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private static void DrawTabPanelLine(Rect canvas)
        {
            int entryIndex = _tabEntries.FirstIndexOf(e => e.Category == _category);
            float entryStartPoint = _tabEntries.Take(entryIndex).Sum(e => e.Width);
            float entryEndPoint = canvas.x + entryStartPoint + _tabEntries[entryIndex].Width;

            Color cache = GUI.color;

            GUI.color = Color.black;
            Widgets.DrawLineVertical(canvas.x, canvas.y, canvas.height);
            Widgets.DrawLineHorizontal(canvas.x, canvas.y + canvas.height - 1f, canvas.width);

            GUI.color = Color.grey;

            if (entryStartPoint > 0)
            {
                Widgets.DrawLineHorizontal(canvas.x, canvas.y, entryStartPoint + 1f);
            }

            Widgets.DrawLineVertical(canvas.x + canvas.width - 1f, canvas.y, canvas.height);
            Widgets.DrawLineHorizontal(entryEndPoint, canvas.y, canvas.width - entryEndPoint);

            GUI.color = cache;
        }

        private static void ValidateTabs()
        {
            _tabEntries ??= Enum.GetNames(typeof(Categories))
               .Select(
                    n => new TabEntry
                    {
                        Label = $"TKUtils.{n}".Localize(), Category = (Categories) Enum.Parse(typeof(Categories), n)
                    }
                )
               .ToArray();
        }

        private static void DrawTabs(Rect canvas)
        {
            Color cache = GUI.color;
            float distributedWidth = canvas.width / _tabEntries.Length;
            var currentTabCanvas = new Rect(0f, 0f, distributedWidth, canvas.height);

            _modVersion ??= Data.Mods?.FirstOrDefault(
                                    m => m.Name.StartsWith("ToolkitUtils", StringComparison.InvariantCultureIgnoreCase)
                                )
                              ?.Version
                            ?? "";

            if (!_modVersion.NullOrEmpty())
            {
                SettingsHelper.DrawColoredLabel(canvas, $"v{_modVersion}  ", Color.gray, TextAnchor.MiddleRight);
            }

            GUI.color = Color.black;
            Widgets.DrawLineVertical(currentTabCanvas.x, currentTabCanvas.y, currentTabCanvas.height);
            GUI.color = cache;

            foreach (TabEntry entry in _tabEntries)
            {
                currentTabCanvas.width = Mathf.Min(canvas.width - currentTabCanvas.x, entry.Width);

                if (_category == entry.Category)
                {
                    Widgets.DrawLightHighlight(currentTabCanvas);
                    GUI.color = Color.grey;
                }
                else
                {
                    GUI.color = Color.black;
                }

                Widgets.DrawLineHorizontal(currentTabCanvas.x, currentTabCanvas.y, currentTabCanvas.width);
                Widgets.DrawLineVertical(currentTabCanvas.x + entry.Width, currentTabCanvas.y, currentTabCanvas.height);

                GUI.color = cache;
                SettingsHelper.DrawLabel(currentTabCanvas, entry.Label, TextAnchor.MiddleCenter);

                if (Widgets.ButtonInvisible(currentTabCanvas))
                {
                    _category = entry.Category;
                }

                currentTabCanvas = currentTabCanvas.ShiftRight(0f);
            }

            GUI.color = cache;
        }

        private static void ValidateEnumOptions()
        {
            _dumpStyleOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.DumpStyle.SingleFile".Localize(),
                    () => DumpStyle = nameof(DumpStyles.SingleFile)
                ),
                new FloatMenuOption(
                    "TKUtils.DumpStyle.MultiFile".Localize(),
                    () => DumpStyle = nameof(DumpStyles.MultiFile)
                )
            };

            LeaveMenuOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Abandon.Method.Thanos".Localize(),
                    () => LeaveMethod = nameof(LeaveMethods.Thanos)
                ),
                new FloatMenuOption(
                    "TKUtils.Abandon.Method.MentalBreak".Localize(),
                    () => LeaveMethod = nameof(LeaveMethods.MentalBreak)
                )
            };

            _coinUserTypeOptions ??= new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.BroadcasterUserType.Broadcaster".Localize(),
                    () => BroadcasterCoinType = nameof(UserCoinType.Broadcaster)
                ),
                new FloatMenuOption(
                    "TKUtils.BroadcasterUserType.Subscriber".Localize(),
                    () => BroadcasterCoinType = nameof(UserCoinType.Subscriber)
                ),
                new FloatMenuOption(
                    "TKUtils.BroadcasterUserType.Vip".Localize(),
                    () => BroadcasterCoinType = nameof(UserCoinType.Vip)
                ),
                new FloatMenuOption(
                    "TKUtils.BroadcasterUserType.Moderator".Localize(),
                    () => BroadcasterCoinType = nameof(UserCoinType.Moderator)
                ),
                new FloatMenuOption(
                    "TKUtils.BroadcasterUserType.None".Localize(),
                    () => BroadcasterCoinType = nameof(UserCoinType.None)
                )
            };
        }

        private static void DrawGeneralTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.DrawGroupHeader("TKUtils.General.Emojis".Localize(), false);
            listing.CheckboxLabeled("TKUtils.Emojis.Label".Localize(), ref Emojis);
            listing.DrawDescription("TKUtils.Emojis.Description".Localize());


            listing.DrawGroupHeader("TKUtils.General.Viewer".Localize());
            listing.CheckboxLabeled("TKUtils.HairColor.Label".Localize(), ref HairColor);
            listing.DrawDescription("TKUtils.HairColor.Description".Localize());

            listing.DrawGroupHeader("TKUtils.General.Gateway".Localize());
            listing.CheckboxLabeled("TKUtils.GatewayPuff.Label".Localize(), ref GatewayPuff);
            listing.DrawDescription("TKUtils.GatewayPuff.Description".Localize());

            listing.End();
        }

        private static void DrawCommandTweaksTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 48f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.DrawGroupHeader("TKUtils.CommandTweaks.Balance".Localize(), false);
            listing.CheckboxLabeled("TKUtils.CoinRate.Label".Localize(), ref ShowCoinRate);
            listing.DrawDescription("TKUtils.CoinRate.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.Handler".Localize());

            if (Commands)
            {
                (Rect prefixLabel, Rect prefixField) = listing.GetRect(Text.LineHeight).ToForm();
                SettingsHelper.DrawLabel(prefixLabel, "TKUtils.CommandPrefix.Label".Localize());
                listing.DrawDescription("TKUtils.CommandPrefix.Description".Localize());
                Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, Prefix));

                (Rect buyPrefixLabel, Rect buyPrefixField) = listing.GetRect(Text.LineHeight).ToForm();
                SettingsHelper.DrawLabel(buyPrefixLabel, "TKUtils.PurchasePrefix.Label".Localize());
                listing.DrawDescription("TKUtils.PurchasePrefix.Description".Localize());
                BuyPrefix = CommandHelper.ValidatePrefix(Widgets.TextField(buyPrefixField, BuyPrefix));
            }

            listing.CheckboxLabeled("TKUtils.CommandParser.Label".Localize(), ref Commands);
            listing.DrawDescription("TKUtils.CommandParser.Description".Localize());

            if (Commands)
            {
                listing.CheckboxLabeled("TKUtils.ToolkitStyleCommands.Label".Localize(), ref ToolkitStyleCommands);
                listing.DrawDescription("TKUtils.ToolkitStyleCommands.Description".Localize());
            }


            listing.DrawGroupHeader("TKUtils.CommandTweaks.InstalledMods".Localize());
            listing.CheckboxLabeled("TKUtils.DecorateUtils.Label".Localize(), ref DecorateMods);
            listing.DrawDescription("TKUtils.DecorateUtils.Description".Localize());

            listing.CheckboxLabeled("TKUtils.VersionedModList.Label".Localize(), ref VersionedModList);
            listing.DrawDescription("TKUtils.VersionedModList.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.BuyItem".Localize());
            listing.CheckboxLabeled("TKUtils.BuyItemBalance.Label".Localize(), ref BuyItemBalance);
            listing.DrawDescription("TKUtils.BuyItemBalance.Description".Localize());
            listing.CheckboxLabeled("TKUtils.BuyItemFullSyntax.Label".Localize(), ref ForceFullItem);
            listing.DrawDescription("TKUtils.BuyItemFullSyntax.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.Lookup".Localize());

            (Rect lookupLimitLabel, Rect lookupLimitField) = listing.GetRect(Text.LineHeight).ToForm();
            var buffer = LookupLimit.ToString();

            SettingsHelper.DrawLabel(lookupLimitLabel, "TKUtils.LookupLimit.Label".Localize());
            Widgets.TextFieldNumeric(lookupLimitField, ref LookupLimit, ref buffer);
            listing.DrawDescription("TKUtils.LookupLimit.Description".Localize());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

        private static void DrawPawnCommandsTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 40f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _commandTweaksPos, viewPort);
            listing.Begin(viewPort);

            listing.DrawGroupHeader("TKUtils.PawnCommands.Abandon".Localize(), false);

            (Rect leaveLabelRect, Rect leaveRect) = listing.GetRect(Text.LineHeight).ToForm();
            SettingsHelper.DrawLabel(leaveLabelRect, "TKUtils.Abandon.Method.Label".Localize());
            listing.DrawDescription("TKUtils.Abandon.Method.Description".Localize());

            if (Widgets.ButtonText(leaveRect, $"TKUtils.Abandon.Method.{LeaveMethod}".Localize()))
            {
                Find.WindowStack.Add(new FloatMenu(LeaveMenuOptions));
            }

            if (!LeaveMethod.EqualsIgnoreCase(nameof(LeaveMethods.Thanos)))
            {
                listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".Localize(), ref DropInventory);
                listing.DrawDescription("TKUtils.Abandon.Gear.Description".Localize());
            }

            listing.DrawGroupHeader("TKUtils.PawnCommands.Gear".Localize());
            listing.CheckboxLabeled("TKUtils.PawnGear.Temperature.Label".Localize(), ref TempInGear);
            listing.DrawDescription("TKUtils.PawnGear.Temperature.Description".Localize());
            listing.CheckboxLabeled("TKUtils.PawnGear.Apparel.Label".Localize(), ref ShowApparel);
            listing.DrawDescription("TKUtils.PawnGear.Apparel.Description".Localize());
            listing.CheckboxLabeled("TKUtils.PawnGear.Armor.Label".Localize(), ref ShowArmor);
            listing.DrawDescription("TKUtils.PawnGear.Armor.Description".Localize());
            listing.CheckboxLabeled("TKUtils.PawnGear.Weapon.Label".Localize(), ref ShowWeapon);
            listing.DrawDescription("TKUtils.PawnGear.Weapon.Description".Localize());


            listing.DrawGroupHeader("TKUtils.PawnCommands.Health".Localize());
            listing.CheckboxLabeled("TKUtils.PawnHealth.Surgeries.Label".Localize(), ref ShowSurgeries);
            listing.DrawDescription("TKUtils.PawnHealth.Surgeries.Description".Localize());


            listing.DrawGroupHeader("TKUtils.PawnCommands.Relations".Localize());
            (Rect opinionLabel, Rect opinionField) = listing.GetRect(Text.LineHeight).ToForm();
            var buffer = OpinionMinimum.ToString();

            if (!MinimalRelations)
            {
                SettingsHelper.DrawLabel(opinionLabel, "TKUtils.PawnRelations.OpinionThreshold.Label".Localize());
                Widgets.TextFieldNumeric(opinionField, ref OpinionMinimum, ref buffer);
                listing.DrawDescription("TKUtils.PawnRelations.OpinionThreshold.Description".Localize());
            }

            listing.CheckboxLabeled("TKUtils.PawnRelations.MinimalRelations.Label".Localize(), ref MinimalRelations);
            listing.DrawDescription("TKUtils.PawnRelations.MinimalRelations.Description".Localize());

            listing.DrawGroupHeader("TKUtils.PawnCommands.Work".Localize());
            listing.CheckboxLabeled("TKUtils.PawnWork.Sort.Label".Localize(), ref SortWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Sort.Description".Localize());
            listing.CheckboxLabeled("TKUtils.PawnWork.Filter.Label".Localize(), ref FilterWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Filter.Description".Localize());

            GUI.EndGroup();
            Widgets.EndScrollView();
            listing.End();
        }

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
                WorkSetting workSetting =
                    WorkSettings.FirstOrDefault(w => w.WorkTypeDef.EqualsIgnoreCase(workType.defName));

                if (workSetting == null)
                {
                    workSetting = new WorkSetting {Enabled = true, WorkTypeDef = workType.defName};

                    WorkSettings.Add(workSetting);
                }

                Rect line = listing.GetRect(Text.LineHeight);

                if (!line.IsRegionVisible(content, _workScrollPos))
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
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", nameof(LeaveMethods.MentalBreak));
            Scribe_Values.Look(ref DumpStyle, "dumpStyle", nameof(DumpStyles.SingleFile));
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
            Scribe_Values.Look(ref Puppeteer, "puppeteer");
            Scribe_Values.Look(ref MinimalRelations, "minimalRelations", true);
            Scribe_Values.Look(ref GatewayPuff, "gatewayPuff", true);

            Scribe_Collections.Look(ref WorkSettings, "workSettings", LookMode.Deep);
        }

        internal static void ValidateDynamicSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }


            WorkSettings ??= new List<WorkSetting>();


            foreach (WorkTypeDef workType in _workTypeDefs.Where(
                d => !WorkSettings.Any(s => s.WorkTypeDef.EqualsIgnoreCase(d.defName))
            ))
            {
                WorkSettings.Add(new WorkSetting {Enabled = true, WorkTypeDef = workType.defName});
            }
        }

        public class WorkSetting : IExposable
        {
            [Description("Whether or not the work priority will be shown in !mypawnwork")]
            public bool Enabled;

            [Description("The def name of the work type instance.")]
            public string WorkTypeDef;

            public void ExposeData()
            {
                Scribe_Values.Look(ref WorkTypeDef, "defName");
                Scribe_Values.Look(ref Enabled, "enabled", true);
            }
        }

        private class TabEntry
        {
            private string label;
            private float width;

            public string Label
            {
                get => label;
                set
                {
                    label = value;
                    width = -1;
                }
            }

            public Categories Category { get; set; }

            public float Width
            {
                get
                {
                    if (width <= 0)
                    {
                        width = Text.CalcSize(label).x + 25f;
                    }

                    return width;
                }
            }
        }
    }
}

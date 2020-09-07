using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
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
        PawnStats
    }

    public enum LeaveMethods { Thanos, MentalBreak }

    public enum DumpStyles { SingleFile, MultiFile }

    [StaticConstructorOnStartup]
    public class TkSettings : ModSettings
    {
        public static bool Commands;
        public static string Prefix = "!";
        public static bool ToolkitStyleCommands;
        public static bool DecorateUtils;
        public static bool Emojis = true;
        public static bool FilterWorkPriorities;
        public static bool ShowApparel;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool SortWorkPriorities;
        public static bool Race = true;
        public static bool TempInGear;
        public static bool DropInventory;
        public static string LeaveMethod = nameof(LeaveMethods.MentalBreak);
        public static string DumpStyle = nameof(DumpStyles.SingleFile);
        public static int LookupLimit = 10;
        public static bool VersionedModList;
        public static bool ShowCoinRate = true;
        public static bool HairColor = true;
        public static int StoreBuildRate = 60;
        public static bool StoreState = true;
        public static bool Offload;
        public static bool BuyItemBalance;

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();
        public static List<StatSetting> StatSettings = new List<StatSetting>();

        private static Categories _category = Categories.General;
        private static List<FloatMenuOption> _leaveMenuOptions;
        private static List<FloatMenuOption> _dumpStyleOptions;
        private static TabEntry[] _tabEntries;

        private static WorkTypeDef[] _workTypeDefs;
        private static StatDef[] _statDefs;

        private static Vector2 _workScrollPos = Vector2.zero;
        private static Vector2 _statScrollPos = Vector2.zero;
        private static Vector2 _commandTweaksPos = Vector2.zero;

        static TkSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }

            if (_statDefs.NullOrEmpty())
            {
                _statDefs = DefDatabase<StatDef>.AllDefsListForReading.ToArray();
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
                case Categories.PawnStats:
                    DrawPawnStatsTab(trueContentRect);
                    break;
            }

            GUI.EndGroup();


            GUI.EndGroup();
        }

        private static void DrawDataTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.DrawGroupHeader("TKUtils.Data.Files".Localize(), false);

            (Rect dumpLabel, Rect dumpBtn) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(dumpLabel, "TKUtils.DumpStyle.Label".Localize());
            listing.DrawDescription("TKUtils.DumpStyle.Description".Localize());

            if (Widgets.ButtonText(dumpBtn, DumpStyle))
            {
                Find.WindowStack.Add(new FloatMenu(_dumpStyleOptions));
            }

            listing.CheckboxLabeled("TKUtils.OffloadShop.Label".Localize(), ref Offload);
            listing.DrawDescription("TKUtils.OffloadShop.Description".Localize());
            listing.DrawDescription("TKUtils.Experimental".Localize(), new Color(1f, 0.53f, 0.76f));


            listing.DrawGroupHeader("TKUtils.Data.LazyProcess".Localize());

            (Rect storeLabel, Rect storeField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(storeLabel, "TKUtils.StoreRate.Label".Localize());
            listing.DrawDescription("TKUtils.StoreRate.Description".Localize());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

            listing.End();
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
                SettingsHelper.DrawLabelAnchored(currentTabCanvas, entry.Label, TextAnchor.MiddleCenter);

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

            _leaveMenuOptions ??= new List<FloatMenuOption>
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

            listing.End();
        }

        private static void DrawCommandTweaksTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 40f);

            GUI.BeginGroup(canvas);
            listing.BeginScrollView(canvas, ref _commandTweaksPos, ref viewPort);

            listing.DrawGroupHeader("TKUtils.CommandTweaks.Balance".Localize(), false);
            listing.CheckboxLabeled("TKUtils.CoinRate.Label".Localize(), ref ShowCoinRate);
            listing.DrawDescription("TKUtils.CoinRate.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.Handler".Localize());

            if (Commands)
            {
                (Rect prefixLabel, Rect prefixField) = listing.GetRect(Text.LineHeight).ToForm();
                Widgets.Label(prefixLabel, "TKUtils.CommandPrefix.Label".Localize());
                listing.DrawDescription("TKUtils.CommandPrefix.Description".Localize());
                Prefix = CommandHelper.ValidatePrefix(Widgets.TextField(prefixField, Prefix));
            }

            listing.CheckboxLabeled("TKUtils.CommandParser.Label".Localize(), ref Commands);
            listing.DrawDescription("TKUtils.CommandParser.Description".Localize());

            if (Commands)
            {
                listing.CheckboxLabeled("TKUtils.ToolkitStyleCommands.Label".Localize(), ref ToolkitStyleCommands);
                listing.DrawDescription("TKUtils.ToolkitStyleCommands.Description".Localize());
            }


            listing.DrawGroupHeader("TKUtils.CommandTweaks.InstalledMods".Localize());
            listing.CheckboxLabeled("TKUtils.DecorateUtils.Label".Localize(), ref DecorateUtils);
            listing.DrawDescription("TKUtils.DecorateUtils.Description".Localize());

            listing.CheckboxLabeled("TKUtils.VersionedModList.Label".Localize(), ref VersionedModList);
            listing.DrawDescription("TKUtils.VersionedModList.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.BuyItem".Localize());
            listing.CheckboxLabeled("TKUtils.BuyItemBalance.Label".Localize(), ref BuyItemBalance);
            listing.DrawDescription("TKUtils.BuyItemBalance.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.Lookup".Localize());

            (Rect lookupLimitLabel, Rect lookupLimitField) = listing.GetRect(Text.LineHeight).ToForm();
            var buffer = LookupLimit.ToString();

            Widgets.Label(lookupLimitLabel, "TKUtils.LookupLimit.Label".Localize());
            Widgets.TextFieldNumeric(lookupLimitField, ref LookupLimit, ref buffer);
            listing.DrawDescription("TKUtils.LookupLimit.Description".Localize());


            listing.DrawGroupHeader("TKUtils.CommandTweaks.BuyPawn".Localize());
            listing.CheckboxLabeled("TKUtils.PawnKind.Label".Localize(), ref Race);
            listing.DrawDescription("TKUtils.PawnKind.Description".Localize());

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
        }

        private static void DrawPawnCommandsTab(Rect canvas)
        {
            var listing = new Listing_Standard();
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.LineHeight * 32f);

            GUI.BeginGroup(canvas);
            listing.BeginScrollView(canvas, ref _commandTweaksPos, ref viewPort);

            listing.DrawGroupHeader("TKUtils.PawnCommands.Gear".Localize(), false);
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


            listing.DrawGroupHeader("TKUtils.PawnCommands.Work".Localize());
            listing.CheckboxLabeled("TKUtils.PawnWork.Sort.Label".Localize(), ref SortWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Sort.Description".Localize());
            listing.CheckboxLabeled("TKUtils.PawnWork.Filter.Label".Localize(), ref FilterWorkPriorities);
            listing.DrawDescription("TKUtils.PawnWork.Filter.Description".Localize());


            listing.DrawGroupHeader("TKUtils.PawnCommands.Abandon".Localize());

            (Rect leaveLabelRect, Rect leaveRect) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(leaveLabelRect, "TKUtils.Abandon.Method.Label".Localize());

            if (Widgets.ButtonText(leaveRect, LeaveMethod))
            {
                Find.WindowStack.Add(new FloatMenu(_leaveMenuOptions));
            }

            if (!LeaveMethod.EqualsIgnoreCase(nameof(LeaveMethods.Thanos)))
            {
                listing.CheckboxLabeled("TKUtils.Abandon.Gear.Label".Localize(), ref DropInventory);
                listing.DrawDescription("TKUtils.Abandon.Gear.Description".Localize());
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
        }

        private static void DrawPawnWorkTab(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _workTypeDefs.Length * Text.LineHeight);

            listing.BeginScrollView(content, ref _workScrollPos, ref view);

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
            listing.EndScrollView(ref view);
        }

        private static void DrawPawnStatsTab(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _statDefs.Length * Text.LineHeight);

            listing.BeginScrollView(content, ref _statScrollPos, ref view);
            for (var index = 0; index < _statDefs.Length; index++)
            {
                StatDef statDef = _statDefs[index];
                StatSetting statSetting = StatSettings.FirstOrDefault(w => w.StatDef.EqualsIgnoreCase(statDef.defName));

                if (statSetting == null)
                {
                    statSetting = new StatSetting {Enabled = true, StatDef = statDef.defName};

                    StatSettings.Add(statSetting);
                }

                Rect line = listing.GetRect(Text.LineHeight);

                if (!line.IsRegionVisible(content, _statScrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.CheckboxLabeled(
                    line,
                    statDef.LabelForFullStatListCap ?? statDef.LabelCap,
                    ref statSetting.Enabled
                );

                Widgets.DrawHighlightIfMouseover(line);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref view);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Commands, "commands", true);
            Scribe_Values.Look(ref Prefix, "prefix", "!");
            Scribe_Values.Look(ref ToolkitStyleCommands, "toolkitStyleCommands");

            Scribe_Values.Look(ref Emojis, "emojis", true);
            Scribe_Values.Look(ref DecorateUtils, "decorateUtils");
            Scribe_Values.Look(ref VersionedModList, "versionedModList");
            Scribe_Values.Look(ref HairColor, "hairColor", true);
            Scribe_Values.Look(ref BuyItemBalance, "buyItemBalance");

            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowApparel, "apparel");
            Scribe_Values.Look(ref TempInGear, "tempInGear");
            Scribe_Values.Look(ref SortWorkPriorities, "sortWork");
            Scribe_Values.Look(ref FilterWorkPriorities, "filterWork");

            Scribe_Values.Look(ref LookupLimit, "lookupLimit", 10);
            Scribe_Values.Look(ref Race, "race", true);
            Scribe_Values.Look(ref LeaveMethod, "leaveMethod", nameof(LeaveMethods.MentalBreak));
            Scribe_Values.Look(ref DumpStyle, "dumpStyle", nameof(DumpStyles.SingleFile));
            Scribe_Values.Look(ref DropInventory, "dropInventory");

            Scribe_Collections.Look(ref WorkSettings, "workSettings", LookMode.Deep);
            Scribe_Collections.Look(ref StatSettings, "statSettings", LookMode.Deep);

            Scribe_Values.Look(ref Offload, "offload");
        }

        internal static void ValidateDynamicSettings()
        {
            if (_workTypeDefs.NullOrEmpty())
            {
                _workTypeDefs = WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.ToArray();
            }

            if (_statDefs.NullOrEmpty())
            {
                _statDefs = DefDatabase<StatDef>.AllDefsListForReading.ToArray();
            }


            WorkSettings ??= new List<WorkSetting>();
            StatSettings ??= new List<StatSetting>();


            foreach (WorkTypeDef workType in _workTypeDefs.Where(
                d => !WorkSettings.Any(s => s.WorkTypeDef.EqualsIgnoreCase(d.defName))
            ))
            {
                WorkSettings.Add(new WorkSetting {Enabled = true, WorkTypeDef = workType.defName});
            }

            foreach (StatDef stat in _statDefs.Where(d => !StatSettings.Any(s => s.StatDef.EqualsIgnoreCase(d.defName)))
            )
            {
                StatSettings.Add(new StatSetting {Enabled = true, StatDef = stat.defName});
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

        public class StatSetting : IExposable
        {
            [Description("Whether or not the stat will be shown in !mypawnstats")]
            public bool Enabled;

            [Description("The def name of the stat instance.")]
            public string StatDef;

            public void ExposeData()
            {
                Scribe_Values.Look(ref StatDef, "defName");
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

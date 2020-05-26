using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum Categories
    {
        General, CommandTweaks, PawnCommands,
        PawnWork, PawnStats
    }

    public enum LeaveMethods { Thanos, MentalBreak }

    public enum DumpStyles { SingleFile, MultiFile }

    [StaticConstructorOnStartup]
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
        public static string DumpStyle = ToolkitUtils.DumpStyles.SingleFile.ToString();
        public static int LookupLimit = 10;
        public static bool VersionedModList;
        public static bool ShowCoinRate = true;
        public static bool HairColor = true;
        public static bool StoreLoading = true;
        public static int StoreBuildRate = 60;

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();
        public static List<StatSetting> StatSettings = new List<StatSetting>();

        private static Categories _category = Categories.General;
        private static readonly string[] LeaveMethods = Enum.GetNames(typeof(LeaveMethods));
        private static readonly string[] DumpStyles = Enum.GetNames(typeof(DumpStyles));
        private static readonly Tuple<string, Categories>[] MenuCategories;

        private static WorkTypeDef[] _workTypeDefs;
        private static StatDef[] _statDefs;

        private static Vector2 _menuScrollPos = Vector2.zero;
        private static Vector2 _workScrollPos = Vector2.zero;
        private static Vector2 _statScrollPos = Vector2.zero;

        static TkSettings()
        {
            MenuCategories = Enum.GetNames(typeof(Categories))
                .Select(n => new Tuple<string, Categories>(n, (Categories) Enum.Parse(typeof(Categories), n)))
                .ToArray();

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
            GUI.BeginGroup(inRect);
            var catRect = new Rect(0f, 0f, inRect.width * 0.25f, inRect.height);
            var setRect = new Rect(
                catRect.width + 10f,
                0f,
                inRect.width - catRect.width - 10f,
                inRect.height
            );

            GUI.BeginGroup(catRect);
            var menu = new Rect(0f, 0f, catRect.width, catRect.height).ContractedBy(5f);
            var menuView = new Rect(0f, 0f, menu.width, Text.LineHeight * MenuCategories.Length);

            if (menuView.height > menu.height)
            {
                menuView.width -= 16f;
            }

            var listing = new Listing_Standard();
            Widgets.DrawMenuSection(new Rect(0f, 0f, catRect.width, catRect.height));

            listing.BeginScrollView(menu, ref _menuScrollPos, ref menuView);
            foreach (var (name, value) in MenuCategories)
            {
                var line = listing.GetRect(Text.LineHeight);

                if (!line.IsRegionVisible(menu, _menuScrollPos))
                {
                    continue;
                }

                if (_category == value)
                {
                    Widgets.DrawHighlightSelected(line);
                }

                Widgets.Label(line, $"TKUtils.SettingGroups.{name}".Translate());

                if (Widgets.ButtonInvisible(line))
                {
                    _category = value;
                }

                Widgets.DrawHighlightIfMouseover(line);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref menuView);


            GUI.BeginGroup(setRect);
            var contentArea = new Rect(0f, 0f, setRect.width, setRect.height);

            switch (_category)
            {
                case Categories.General:
                    DrawGeneral(contentArea);
                    break;
                case Categories.CommandTweaks:
                    DrawCommandTweaks(contentArea);
                    break;
                case Categories.PawnCommands:
                    DrawPawnCommands(contentArea);
                    break;
                case Categories.PawnWork:
                    DrawPawnWork(contentArea);
                    break;
                case Categories.PawnStats:
                    DrawPawnStats(contentArea);
                    break;
            }

            GUI.EndGroup();

            GUI.EndGroup();
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
                "TKUtils.SettingGroups.General.HairColor.Label".Translate(),
                ref HairColor,
                "TKUtils.SettingGroups.General.HairColor.Tooltip".Translate()
            );

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.JsonStore.Label".Translate(),
                ref JsonShop,
                "TKUtils.SettingGroups.General.JsonStore.Tooltip".Translate()
            );

            var (dumpLabel, dumpBtn) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(dumpLabel, "TKUtils.SettingGroups.General.DumpStyle.Label".Translate());

            if (Widgets.ButtonText(dumpBtn, DumpStyle))
            {
                Find.WindowStack.Add(
                    new FloatMenu(DumpStyles.Select(o => new FloatMenuOption(o, () => DumpStyle = o)).ToList())
                );
            }

            listing.CheckboxLabeled(
                "TKUtils.SettingGroups.General.StoreLoading.Label".Translate(),
                ref StoreLoading,
                "TKUtils.SettingGroups.General.StoreLoading.Tooltip".Translate()
            );
            var (storeLabel, storeField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(storeLabel, "TKUtils.SettingGroups.General.StoreRate.Label".Translate());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

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

        private static void DrawPawnWork(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            // listing.Begin(canvas);
            //
            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _workTypeDefs.Length * Text.LineHeight);

            listing.BeginScrollView(content, ref _workScrollPos, ref view);

            for (var index = 0; index < _workTypeDefs.Length; index++)
            {
                var workType = _workTypeDefs[index];
                var workSetting = WorkSettings.FirstOrDefault(w => w.WorkTypeDef.EqualsIgnoreCase(workType.defName));

                if (workSetting == null)
                {
                    workSetting = new WorkSetting {Enabled = true, WorkTypeDef = workType.defName};

                    WorkSettings.Add(workSetting);
                }

                var line = listing.GetRect(Text.LineHeight);

                if (!line.IsRegionVisible(content, _workScrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(line);
                }

                Widgets.CheckboxLabeled(
                    line,
                    workSetting.WorkTypeDef,
                    ref workSetting.Enabled
                );

                Widgets.DrawHighlightIfMouseover(line);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref view);
            // listing.End();
        }

        private static void DrawPawnStats(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var listing = new Listing_Standard();
            // listing.Begin(canvas);

            var content = new Rect(0f, 0f, canvas.width, canvas.height);
            var view = new Rect(0f, 0f, canvas.width - 16f, _statDefs.Length * Text.LineHeight);

            listing.BeginScrollView(content, ref _statScrollPos, ref view);
            for (var index = 0; index < _statDefs.Length; index++)
            {
                var statDef = _statDefs[index];
                var statSetting = StatSettings.FirstOrDefault(w => w.StatDef.EqualsIgnoreCase(statDef.defName));

                if (statSetting == null)
                {
                    statSetting = new StatSetting {Enabled = true, StatDef = statDef.defName};

                    StatSettings.Add(statSetting);
                }

                var line = listing.GetRect(Text.LineHeight);

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
            // listing.End();
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
            Scribe_Values.Look(ref DumpStyle, "dumpStyle", ToolkitUtils.DumpStyles.SingleFile.ToString());
            Scribe_Values.Look(ref DropInventory, "dropInventory");

            Scribe_Values.Look(ref JsonShop, "shopJson");

            Scribe_Collections.Look(ref WorkSettings, "workSettings", LookMode.Deep);
            Scribe_Collections.Look(ref StatSettings, "statSettings", LookMode.Deep);
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


            foreach (var workType in _workTypeDefs.Where(
                d => !WorkSettings.Any(s => s.WorkTypeDef.EqualsIgnoreCase(d.defName))
            ))
            {
                WorkSettings.Add(new WorkSetting {Enabled = true, WorkTypeDef = workType.defName});
            }

            foreach (var stat in _statDefs.Where(
                d => !StatSettings.Any(s => s.StatDef.EqualsIgnoreCase(d.defName))
            ))
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
    }
}

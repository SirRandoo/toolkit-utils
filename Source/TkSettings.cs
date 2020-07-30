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
        CommandTweaks,
        PawnCommands,
        PawnWork,
        PawnStats,
        Experimental
    }

    public enum LeaveMethods { Thanos, MentalBreak }

    public enum DumpStyles { SingleFile, MultiFile }

    [StaticConstructorOnStartup]
    public class TkSettings : ModSettings
    {
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

        public static List<WorkSetting> WorkSettings = new List<WorkSetting>();
        public static List<StatSetting> StatSettings = new List<StatSetting>();

        private static Categories _category = Categories.General;
        private static List<FloatMenuOption> _leaveMenuOptions;
        private static List<FloatMenuOption> _dumpStyleOptions;
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
            const float adjustedHeight = 700f - 80f;

            if (inRect.height > adjustedHeight)
            {
                inRect.height = adjustedHeight;
            }

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
                    "TKUtils.LeaveMethod.Thanos".Localize(),
                    () => LeaveMethod = nameof(LeaveMethods.Thanos)
                ),
                new FloatMenuOption(
                    "TKUtils.LeaveMethod.MentalBreak".Localize(),
                    () => LeaveMethod = nameof(LeaveMethods.MentalBreak)
                )
            };

            GUI.BeginGroup(inRect);
            var catRect = new Rect(0f, 0f, inRect.width * 0.25f, inRect.height);
            var setRect = new Rect(catRect.width + 10f, 0f, inRect.width - catRect.width - 10f, inRect.height);

            GUI.BeginGroup(catRect);
            Rect menu = new Rect(0f, 0f, catRect.width, catRect.height).ContractedBy(5f);
            var menuView = new Rect(0f, 0f, menu.width, Text.LineHeight * MenuCategories.Length);

            if (menuView.height > menu.height)
            {
                menuView.width -= 16f;
            }

            var listing = new Listing_Standard();
            Widgets.DrawMenuSection(new Rect(0f, 0f, catRect.width, catRect.height));

            listing.BeginScrollView(menu, ref _menuScrollPos, ref menuView);
            foreach ((string name, Categories value) in MenuCategories)
            {
                Rect line = listing.GetRect(Text.LineHeight);

                if (!line.IsRegionVisible(menu, _menuScrollPos))
                {
                    continue;
                }

                if (_category == value)
                {
                    Widgets.DrawHighlightSelected(line);
                }

                Widgets.Label(line, $"TKUtils.{name}".Localize());

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
                case Categories.Experimental:
                    DrawExperimental(contentArea);
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
                "TKUtils.VersionedModList.Label".Localize(),
                ref VersionedModList,
                "TKUtils.VersionedModList.Tooltip".Localize()
            );

            listing.CheckboxLabeled("TKUtils.Emojis.Label".Localize(), ref Emojis, "TKUtils.Emojis.Tooltip".Localize());

            listing.CheckboxLabeled(
                "TKUtils.DecorateUtils.Label".Localize(),
                ref DecorateUtils,
                "TKUtils.DecorateUtils.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnKind.Label".Localize(),
                ref Race,
                "TKUtils.PawnKind.Tooltip".Localize()
            );

            Rect line = listing.GetRect(Text.LineHeight);
            (Rect labelRect, Rect entryRect) = line.ToForm();
            var buffer = LookupLimit.ToString();

            Widgets.Label(labelRect, "TKUtils.LookupLimit.Label".Localize());
            Widgets.TextFieldNumeric(entryRect, ref LookupLimit, ref buffer);

            if (Mouse.IsOver(line))
            {
                Widgets.DrawHighlight(line);
                TooltipHandler.TipRegion(line, "TKUtils.LookupLimit.Tooltip".Localize());
            }

            listing.CheckboxLabeled(
                "TKUtils.HairColor.Label".Localize(),
                ref HairColor,
                "TKUtils.HairColor.Tooltip".Localize()
            );

            (Rect dumpLabel, Rect dumpBtn) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(dumpLabel, "TKUtils.DumpStyle.Label".Localize());

            if (Widgets.ButtonText(dumpBtn, DumpStyle))
            {
                Find.WindowStack.Add(new FloatMenu(_dumpStyleOptions));
            }

            (Rect storeLabel, Rect storeField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(storeLabel, "TKUtils.StoreRate.Label".Localize());

            var storeBuffer = StoreBuildRate.ToString();
            Widgets.TextFieldNumeric(storeField, ref StoreBuildRate, ref storeBuffer);

            if (Mouse.IsOver(storeLabel))
            {
                Widgets.DrawHighlight(storeLabel);
                TooltipHandler.TipRegion(storeLabel, "TKUtils.StoreRate.Tooltip".Localize());
            }

            listing.End();
        }

        private static void DrawCommandTweaks(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.CoinRate.Label".Translate(),
                ref ShowCoinRate,
                "TKUtils.CoinRate.Tooltip".Localize()
            );

            listing.End();
        }

        private static void DrawPawnCommands(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.PawnGear.Temperature.Label".Localize(),
                ref TempInGear,
                "TKUtils.PawnGear.Temperature.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnGear.Apparel.Label".Localize(),
                ref ShowApparel,
                "TKUtils.PawnGear.Apparel.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnGear.Armor.Label".Localize(),
                ref ShowArmor,
                "TKUtils.PawnGear.Armor.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnGear.Weapon.Label".Localize(),
                ref ShowWeapon,
                "TKUtils.PawnGear.Weapon.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnHealth.Surgeries.Label".Localize(),
                ref ShowSurgeries,
                "TKUtils.PawnHealth.Surgeries.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnWork.Sort.Label".Localize(),
                ref SortWorkPriorities,
                "TKUtils.PawnWork.Sort.Tooltip".Localize()
            );

            listing.CheckboxLabeled(
                "TKUtils.PawnWork.Filter.Label".Localize(),
                ref FilterWorkPriorities,
                "TKUtils.PawnWork.Filter.Tooltip".Localize()
            );

            listing.Gap();

            (Rect leaveLabelRect, Rect leaveRect) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(leaveLabelRect, "TKUtils.Abandon.Method.Label".Localize());

            if (Widgets.ButtonText(leaveRect, LeaveMethod))
            {
                Find.WindowStack.Add(new FloatMenu(_leaveMenuOptions));
            }

            if (!LeaveMethod.EqualsIgnoreCase(nameof(LeaveMethods.Thanos)))
            {
                listing.CheckboxLabeled(
                    "TKUtils.Abandon.Gear.Label".Localize(),
                    ref DropInventory,
                    "TKUtils.Abandon.Gear.Tooltip".Localize()
                );
            }

            listing.End();
        }

        private static void DrawPawnWork(Rect canvas)
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

        private static void DrawPawnStats(Rect canvas)
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

        private static void DrawExperimental(Rect canvas)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            listing.CheckboxLabeled(
                "TKUtils.OffloadShop.Label".Localize(),
                ref Offload,
                "TKUtils.OffloadShop.Tooltip".Localize()
            );

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
    }
}

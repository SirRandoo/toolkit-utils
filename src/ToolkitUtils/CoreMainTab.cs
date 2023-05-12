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
using System.Text;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.CommonLib.Interfaces;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using ToolkitCore.Windows;
using ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace ToolkitUtils
{
    /// <summary>
    ///     A <see cref="RimWorld.MainTabWindow"/> used by RimWorld to
    ///     display an in-game menu, as well as a button, at the bottom of
    ///     the in-game screen.
    /// </summary>
    /// <remarks>
    ///     This class' responsibility is to cache
    ///     <see cref="ToolkitCore.Interfaces.IAddonMenu"/>s, as well as
    ///     displaying any errors raised during command and/or event
    ///     execution.
    /// </remarks>
    [UsedImplicitly]
    public class CoreMainTab : MainTabWindow_ToolkitCore
    {
        private const float ButtonHeight = Text.SmallFontHeight * 1.5f;
        private static readonly MenuEntry[] MenuEntries = FetchMenuEntries();
        private static float _expectedMenuHeight;

        private static readonly IRimLogger Logger = new RimLogger(typeof(CoreMainTab).FullName);
        private static readonly string[] SettingStrings = { "options", "settings", "TKUtils.AddonMenu.Settings".TranslateSimple(), "Options".TranslateSimple() };
        private static readonly Color BackgroundTextColor = new Color(0.39f, 0.39f, 0.39f);

        private string _closeTooltip;
        private string _debugTooltip;
        private string _errorTooltip;
        private string _healthReportText;
        private Vector2 _healthScrollPos = Vector2.zero;
        private string _hoursText;
        private string _infoTooltip;
        private string _minutesText;
        private string _noReportsText;
        private string _quickActionsText;
        private string _secondsText;
        private Vector2? _tabSize;
        private string _warningTooltip;

        public override Vector2 RequestedTabSize => _tabSize ??= new Vector2(550f, Mathf.Max(150.0f, _expectedMenuHeight) + Text.SmallFontHeight + Margin * 2f);

        private static bool IsSettingsString(string text)
        {
            return SettingStrings.Any(str => string.Equals(text, str, StringComparison.CurrentCultureIgnoreCase));
        }

        [ContractAnnotation("=> true, mod: notnull; => false, mod: null")]
        private static bool TryGetModFor(ToolkitAddon addon, out Mod mod)
        {
            foreach (Mod handle in LoadedModManager.ModHandles)
            {
                if (string.IsNullOrEmpty(handle.SettingsCategory()))
                {
                    continue;
                }

                if (handle.Content.AllDefs.All(def => def != addon))
                {
                    continue;
                }

                mod = handle;

                return true;
            }

            mod = null;

            return false;
        }

        [NotNull]
        private static MenuEntry[] FetchMenuEntries()
        {
            _expectedMenuHeight = 0f;
            var insertPoint = 0;
            var builder = new StringBuilder();
            var container = new MenuEntry[AddonRegistry.ToolkitAddons.Count];

            foreach (ToolkitAddon addon in AddonRegistry.ToolkitAddons)
            {
                if (TryCreateEntry(addon, out MenuEntry entry))
                {
                    container[insertPoint++] = entry;
                    _expectedMenuHeight += ButtonHeight;
                }
                else
                {
                    builder.Append($"  - {addon.defName}\n");
                }
            }

            if (builder.Length <= 0)
            {
                return container;
            }

            builder.Insert(0, "The following addon menus couldn't be cached:\n");
            Logger.Warn(builder.ToString());

            return container;
        }

        [ContractAnnotation("=> true, entry: notnull; => false, entry: null")]
        private static bool TryCreateEntry([NotNull] ToolkitAddon addon, out MenuEntry entry)
        {
            var container = new List<FloatMenuOption>();

            foreach (FloatMenuOption option in addon.GetAddonMenu().MenuOptions())
            {
                if (!IsSettingsString(option.Label) || !TryGetModFor(addon, out Mod mod))
                {
                    container.Add(option);

                    continue;
                }

                if (mod != ToolkitCore.ToolkitCore.settings.Mod)
                {
                    option.action = mod.OpenSettings;
                }

                container.Add(new FloatMenuOption(SettingStrings[2], option.action));
            }

            entry = new MenuEntry(addon, container);

            return true;
        }

        public override void PostOpen()
        {
            _secondsText = "TKUtils.Fields.Seconds".TranslateSimple();
            _minutesText = "TKUtils.Fields.Minutes".TranslateSimple();
            _hoursText = "TKUtils.Fields.Hours".TranslateSimple();
            _noReportsText = "TKUtils.MainTab.NoHealthReports".TranslateSimple();
            _closeTooltip = "TKUtils.MainTabTooltips.Close".TranslateSimple();
            _healthReportText = "TKUtils.MainTab.HealthReport".TranslateSimple();
            _quickActionsText = "TKUtils.MainTab.QuickActions".TranslateSimple().Tagged("b");
            _infoTooltip = "TKUtils.MainTabTooltips.Info".TranslateSimple();
            _warningTooltip = "TKUtils.MainTabTooltips.Warning".TranslateSimple();
            _debugTooltip = "TKUtils.MainTabTooltips.Debug".TranslateSimple();
            _errorTooltip = "TKUtils.MainTabTooltips.Error".TranslateSimple();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GameFont cache = Text.Font;
            Text.Font = GameFont.Small;

            GUI.BeginGroup(inRect);

            var leftColumn = new Rect(0f, 0f, Mathf.FloorToInt((inRect.width - 10f) * 0.65f), inRect.height);
            var rightColumn = new Rect(leftColumn.x + leftColumn.width + 10f, 0f, inRect.width - 10f - leftColumn.width, inRect.height);

            GUI.BeginGroup(leftColumn);
            DrawLeftColumn(leftColumn.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(rightColumn);
            DrawRightColumn(rightColumn.AtZero());
            GUI.EndGroup();

            Widgets.DrawLineVertical(leftColumn.x + leftColumn.width + 5f, 3f, inRect.height - 6f);

            GUI.EndGroup();

            Text.Font = cache;
        }

        private void DrawLeftColumn(Rect region)
        {
            var titleRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            UiHelper.Label(titleRect, _healthReportText.Tagged("b"), ColorLibrary.LightBlue, TextAnchor.MiddleCenter, GameFont.Small);

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);

            if (!Data.AllHealthReports.Any())
            {
                UiHelper.Label(contentRect.AtZero(), _noReportsText, BackgroundTextColor, TextAnchor.MiddleCenter, GameFont.Small);
                GUI.EndGroup();

                return;
            }

            try
            {
                DrawHealthReports(contentRect.AtZero());
            }
            catch (Exception)
            {
                // Unused
            }

            GUI.EndGroup();
        }

        private void DrawHealthReports(Rect region)
        {
            var viewRect = new Rect(0f, 0f, region.width - 16f, Data.AllHealthReports.Sum(r => r.Height));

            _healthScrollPos = GUI.BeginScrollView(region, _healthScrollPos, viewRect);

            var y = 0f;
            var alternate = false;

            try
            {
                foreach (HealthReport report in Data.AllHealthReports)
                {
                    if (report.Height <= 0)
                    {
                        report.Height = Text.CalcHeight(report.Message, region.width - 20f);
                    }

                    var lineRect = new Rect(0f, y, region.width - 16f, report.Height);
                    y += report.Height;

                    if (!lineRect.IsVisible(region, _healthScrollPos))
                    {
                        continue;
                    }

                    GUI.BeginGroup(lineRect);
                    DrawHealthReport(lineRect.AtZero(), report, alternate);
                    GUI.EndGroup();

                    alternate = !alternate;
                }
            }
            catch (IndexOutOfRangeException)
            {
                // In case the user removes a health report while this is being drawn.
            }

            GUI.EndScrollView();
        }

        private void DrawHealthReport(Rect region, [NotNull] HealthReport report, bool alternate = false)
        {
            var iconRect = new Rect(0f, 0f, 16f, region.height);
            var closeRect = new Rect(region.x + region.width - 16f, 0f, 16f, region.height);
            var messageRect = new Rect(iconRect.x + iconRect.width + 2f, 0f, region.width - iconRect.width - closeRect.width - 4f, region.height);

            if (alternate)
            {
                Widgets.DrawLightHighlight(region);
            }

            Widgets.DrawLightHighlight(iconRect);

            Texture2D texture = GetTextureFor(report.Type);
            Color color = GetColorFor(report.Type);
            string iconTooltip = GetTooltipFor(report.Type);

            if (texture != null)
            {
                UiHelper.Icon(iconRect, texture, color);
                iconRect.TipRegion(iconTooltip);
            }

            UiHelper.Label(messageRect, report.Message, color, TextAnchor.MiddleLeft, GameFont.Small);

            if (!Mouse.IsOver(messageRect))
            {
                report.OccurredAtString = GetTextString(DateTime.Now - report.OccurredAt);
            }

            messageRect.TipRegion("TKUtils.MainTabTooltips.Report".Translate(report.Reporter, report.OccurredAtString));

            if (!report.Stacktrace.NullOrEmpty() && Widgets.ButtonInvisible(messageRect))
            {
                GUIUtility.systemCopyBuffer = report.Stacktrace;
            }

            UiHelper.Icon(closeRect, Widgets.CheckboxOffTex, Color.red);
            closeRect.TipRegion(_closeTooltip);

            if (Widgets.ButtonInvisible(closeRect))
            {
                Data.RemoveHealthReport(report);
            }
        }

        private static Texture2D GetTextureFor(HealthReport.ReportType reportType)
        {
            switch (reportType)
            {
                case HealthReport.ReportType.Info:
                    return Textures.Info;
                case HealthReport.ReportType.Warning:
                    return Textures.Warning;
                case HealthReport.ReportType.Error:
                case HealthReport.ReportType.Debug:
                    return Textures.Debug;
                default:
                    return Textures.QuestionMark;
            }
        }

        private static Color GetColorFor(HealthReport.ReportType reportType)
        {
            switch (reportType)
            {
                case HealthReport.ReportType.Info:
                    return ColorLibrary.PaleGreen;
                case HealthReport.ReportType.Warning:
                    return ColorLibrary.Yellow;
                case HealthReport.ReportType.Error:
                    return ColorLibrary.Salmon;
                case HealthReport.ReportType.Debug:
                    return ColorLibrary.LightPink;
                default:
                    return Color.white;
            }
        }

        private string GetTooltipFor(HealthReport.ReportType reportType)
        {
            switch (reportType)
            {
                case HealthReport.ReportType.Info:
                    return _infoTooltip;
                case HealthReport.ReportType.Warning:
                    return _warningTooltip;
                case HealthReport.ReportType.Error:
                    return _errorTooltip;
                case HealthReport.ReportType.Debug:
                    return _debugTooltip;
                default:
                    return null;
            }
        }

        private void DrawRightColumn(Rect region)
        {
            var titleRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            UiHelper.Label(titleRect, _quickActionsText, ColorLibrary.LightBlue, TextAnchor.MiddleCenter, GameFont.Small);

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);
            DrawQuickMenus(contentRect.AtZero());
            GUI.EndGroup();
        }

        private static void DrawQuickMenus(Rect region)
        {
            for (var index = 0; index < MenuEntries.Length; index++)
            {
                MenuEntry addon = MenuEntries[index];

                var lineRect = new Rect(0f, ButtonHeight * index, region.width, ButtonHeight);

                if (Widgets.ButtonText(lineRect, addon.Label))
                {
                    Find.WindowStack.Add(new FloatMenu(addon.Options));
                }
            }
        }

        [NotNull]
        private string GetTextString(TimeSpan span)
        {
            if (span.Hours > 0)
            {
                return $"{span.TotalHours:N2} {_hoursText}";
            }

            return span.Minutes > 0 ? $"{span.TotalMinutes:N2} {_minutesText}" : $"{span.TotalSeconds:N2} {_secondsText}";
        }

        private sealed class MenuEntry
        {
            public MenuEntry([NotNull] Def addon, List<FloatMenuOption> options)
            {
                Label = (addon.label ?? addon.defName).CapitalizeFirst();
                Options = options;
            }

            public string Label { get; }
            public List<FloatMenuOption> Options { get; }
        }
    }
}

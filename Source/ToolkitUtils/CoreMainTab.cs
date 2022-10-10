﻿// ToolkitUtils
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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
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
        private static readonly List<MenuEntry> MenuCaches = new List<MenuEntry>();
        private float _buttonHeight;
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

        static CoreMainTab()
        {
            var builder = new StringBuilder();

            foreach (ToolkitAddon addon in AddonRegistry.ToolkitAddons)
            {
                var cache = MenuEntry.CreateInstance(addon, out string error);

                if (!error.NullOrEmpty())
                {
                    builder.Append($" - {error}\n");
                }

                MenuCaches.Add(cache);
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following addon menus could not reliably be opened by the user:\n");
            TkUtils.Logger.Warn(builder.ToString());
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                _buttonHeight = Mathf.CeilToInt(Text.SmallFontHeight * 1.5f);

                return _tabSize ??= new Vector2(550f, Mathf.Max(150.0f, MenuCaches.Count * _buttonHeight) + Text.SmallFontHeight + Margin * 2f);
            }
        }

        public override void PostOpen()
        {
            _secondsText = "TKUtils.Fields.Seconds".Localize();
            _minutesText = "TKUtils.Fields.Minutes".Localize();
            _hoursText = "TKUtils.Fields.Hours".Localize();
            _noReportsText = "TKUtils.MainTab.NoHealthReports".Localize();
            _closeTooltip = "TKUtils.MainTabTooltips.Close".Localize();
            _healthReportText = "TKUtils.MainTab.HealthReport".Localize();
            _quickActionsText = "TKUtils.MainTab.QuickActions".Localize();
            _infoTooltip = "TKUtils.MainTabTooltips.Info".Localize();
            _warningTooltip = "TKUtils.MainTabTooltips.Warning".Localize();
            _debugTooltip = "TKUtils.MainTabTooltips.Debug".Localize();
            _errorTooltip = "TKUtils.MainTabTooltips.Error".Localize();
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
                UiHelper.Label(contentRect.AtZero(), _noReportsText, new Color(0.39f, 0.39f, 0.39f), TextAnchor.MiddleCenter, GameFont.Small);
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

            messageRect.TipRegion("TKUtils.MainTabTooltips.Report".LocalizeKeyed(report.Reporter, report.OccurredAtString));

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
            UiHelper.Label(titleRect, _quickActionsText.Tagged("b"), ColorLibrary.LightBlue, TextAnchor.MiddleCenter, GameFont.Small);

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);
            DrawQuickMenus(contentRect.AtZero());
            GUI.EndGroup();
        }

        private void DrawQuickMenus(Rect region)
        {
            for (var index = 0; index < MenuCaches.Count; index++)
            {
                MenuEntry addon = MenuCaches[index];

                var lineRect = new Rect(0f, _buttonHeight * index, region.width, _buttonHeight);

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
            public string Label => (Addon.label ?? Addon.defName).CapitalizeFirst();
            public ToolkitAddon Addon { get; set; }
            public List<FloatMenuOption> Options { get; private set; }

            [NotNull]
            public static MenuEntry CreateInstance(ToolkitAddon addon, [CanBeNull] out string error)
            {
                error = null;
                var cache = new MenuEntry { Addon = addon, Options = new List<FloatMenuOption>() };

                var hasSettings = false;
                string settingsTranslated = "TKUtils.AddonMenu.Settings".TranslateSimple();

                try
                {
                    string optionsTranslated = "Options".TranslateSimple();

                    foreach (FloatMenuOption option in addon.GetAddonMenu().MenuOptions())
                    {
                        cache.Options.Add(option);

                        if (option.Label.EqualsIgnoreCase("settings") || option.Label.EqualsIgnoreCase(settingsTranslated) || option.Label.EqualsIgnoreCase("options")
                            || option.Label.EqualsIgnoreCase(optionsTranslated))
                        {
                            hasSettings = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    error = $@"The addon menu for ""{cache.Label}"" couldn't open successfully -- Reason: {e.GetType().Name}({e.Message})";
                }

                if (hasSettings)
                {
                    return cache;
                }

                Mod mod = LoadedModManager.ModHandles.FirstOrDefault(m => m.Content == addon.modContentPack);

                if (mod?.SettingsCategory().NullOrEmpty() == true)
                {
                    return cache;
                }

                cache.Options.Insert(0, new FloatMenuOption(settingsTranslated, mod.OpenSettings));

                return cache;
            }
        }
    }
}

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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using ToolkitCore.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class CoreMainTab : MainTabWindow_ToolkitCore
    {
        private static readonly List<MenuCache> MenuCaches;
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
            MenuCaches = new List<MenuCache>();
            var builder = new StringBuilder();

            foreach (ToolkitAddon addon in AddonRegistry.ToolkitAddons)
            {
                var cache = MenuCache.CreateInstance(addon, out string error);

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
            LogHelper.Warn(builder.ToString());
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

        public override void DoWindowContents(Rect region)
        {
            GameFont cache = Text.Font;
            Text.Font = GameFont.Small;

            GUI.BeginGroup(region);

            var leftColumn = new Rect(0f, 0f, Mathf.FloorToInt((region.width - 10f) * 0.65f), region.height);
            var rightColumn = new Rect(leftColumn.x + leftColumn.width + 10f, 0f, region.width - 10f - leftColumn.width, region.height);

            GUI.BeginGroup(leftColumn);
            DrawLeftColumn(leftColumn.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(rightColumn);
            DrawRightColumn(rightColumn.AtZero());
            GUI.EndGroup();

            Widgets.DrawLineVertical(leftColumn.x + leftColumn.width + 5f, 3f, region.height - 6f);

            GUI.EndGroup();

            Text.Font = cache;
        }

        private void DrawLeftColumn(Rect region)
        {
            var titleRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            SettingsHelper.DrawColoredLabel(titleRect, _healthReportText.Tagged("b"), ColorLibrary.LightBlue, TextAnchor.MiddleCenter);

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);

            if (Data.HealthReports.Count <= 0)
            {
                SettingsHelper.DrawColoredLabel(contentRect.AtZero(), _noReportsText, new Color(0.39f, 0.39f, 0.39f), TextAnchor.MiddleCenter);
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
            var viewRect = new Rect(0f, 0f, region.width - 16f, Data.HealthReports.Sum(r => r.Height));

            _healthScrollPos = GUI.BeginScrollView(region, _healthScrollPos, viewRect);

            var y = 0f;

            for (int index = Data.HealthReports.Count - 1; index >= 0; index--)
            {
                HealthReport report = Data.HealthReports[index];

                if (report.Height <= 0)
                {
                    report.Height = Text.CalcHeight(report.Message, region.width - 20f);
                }

                var lineRect = new Rect(0f, y, region.width - 16f, report.Height);
                y += report.Height;

                if (!lineRect.IsRegionVisible(region, _healthScrollPos))
                {
                    continue;
                }

                GUI.BeginGroup(lineRect);
                DrawHealthReport(lineRect.AtZero(), report, index % 2 == 0);
                GUI.EndGroup();
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

            Texture2D texture;
            Color color = Color.white;
            var iconTooltip = "";

            switch (report.Type)
            {
                case HealthReport.ReportType.Info:
                    texture = Textures.Info;
                    color = ColorLibrary.PaleGreen;
                    iconTooltip = _infoTooltip;

                    break;
                case HealthReport.ReportType.Warning:
                    texture = Textures.Warning;
                    color = ColorLibrary.Yellow;
                    iconTooltip = _warningTooltip;

                    break;
                case HealthReport.ReportType.Error:
                    texture = Textures.Warning;
                    color = ColorLibrary.Salmon;
                    iconTooltip = _errorTooltip;

                    break;
                case HealthReport.ReportType.Debug:
                    texture = Textures.Debug;
                    color = ColorLibrary.LightPink;
                    iconTooltip = _debugTooltip;

                    break;
                default:
                    texture = Textures.QuestionMark;

                    break;
            }

            if (texture != null)
            {
                texture.DrawColored(SettingsHelper.RectForIcon(iconRect), color);
                iconRect.TipRegion(iconTooltip);
            }

            SettingsHelper.DrawColoredLabel(messageRect, report.Message, color);

            if (!Mouse.IsOver(messageRect))
            {
                report.OccurredAtString = GetTextString(DateTime.Now - report.OccurredAt);
            }

            messageRect.TipRegion("TKUtils.MainTabTooltips.Report".LocalizeKeyed(report.Reporter, report.OccurredAtString));

            if (!report.Stacktrace.NullOrEmpty() && messageRect.WasLeftClicked())
            {
                GUIUtility.systemCopyBuffer = report.Stacktrace;
            }

            Widgets.CheckboxOffTex.DrawColored(SettingsHelper.RectForIcon(closeRect), Color.red);
            closeRect.TipRegion(_closeTooltip);

            if (closeRect.WasLeftClicked())
            {
                Data.HealthReports.Remove(report);
            }
        }

        private void DrawRightColumn(Rect region)
        {
            var titleRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            SettingsHelper.DrawColoredLabel(titleRect, _quickActionsText.Tagged("b"), ColorLibrary.LightBlue, TextAnchor.MiddleCenter);

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);
            DrawQuickMenus(contentRect.AtZero());
            GUI.EndGroup();
        }

        private void DrawQuickMenus(Rect region)
        {
            for (var index = 0; index < MenuCaches.Count; index++)
            {
                MenuCache addon = MenuCaches[index];

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

        private class MenuCache
        {
            public string Label => (Addon.label ?? Addon.defName).CapitalizeFirst();
            public ToolkitAddon Addon { get; set; }
            public List<FloatMenuOption> Options { get; private set; }

            [NotNull]
            public static MenuCache CreateInstance(ToolkitAddon addon, [CanBeNull] out string error)
            {
                error = null;
                var cache = new MenuCache { Addon = addon, Options = new List<FloatMenuOption>() };

                var hasSettings = false;
                string settingsTranslated = "TKUtils.AddonMenu.Settings".Localize();

                try
                {
                    string optionsTranslated = "Options".Localize();

                    foreach (FloatMenuOption option in addon.GetAddonMenu().MenuOptions())
                    {
                        cache.Options.Add(option);

                        if (option.Label.EqualsIgnoreCase("settings")
                            || option.Label.EqualsIgnoreCase(settingsTranslated)
                            || option.Label.EqualsIgnoreCase("options")
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

                cache.Options.Insert(0, new FloatMenuOption(settingsTranslated, () => SettingsHelper.OpenSettingsMenuFor(mod)));

                return cache;
            }
        }
    }
}

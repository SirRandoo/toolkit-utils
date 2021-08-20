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
        private float buttonHeight;
        private Vector2 healthScrollPos = Vector2.zero;
        private string hoursText;
        private string minutesText;
        private string noReportsText;
        private string secondsText;
        private Vector2? tabSize;

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
                buttonHeight = Mathf.CeilToInt(Text.SmallFontHeight * 1.5f);
                return tabSize ??= new Vector2(
                    500f,
                    Mathf.Max(150.0f, MenuCaches.Count * buttonHeight) + Text.SmallFontHeight + Margin * 2f
                );
            }
        }

        public override void PostOpen()
        {
            secondsText = "TKUtils.Fields.Seconds".Localize();
            minutesText = "TKUtils.Fields.Minutes".Localize();
            hoursText = "TKUtils.Fields.Hours".Localize();
            noReportsText = "TKUtils.MainTab.NoHealthReports".Localize();
        }

        public override void DoWindowContents(Rect region)
        {
            GameFont cache = Text.Font;
            Text.Font = GameFont.Small;

            GUI.BeginGroup(region);

            var leftColumn = new Rect(0f, 0f, Mathf.FloorToInt((region.width - 10f) * 0.62f), region.height);
            var rightColumn = new Rect(
                leftColumn.x + leftColumn.width + 10f,
                0f,
                region.width - 10f - leftColumn.width,
                region.height
            );

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
            SettingsHelper.DrawColoredLabel(
                titleRect,
                "TKUtils.MainTab.HealthReport".Localize().Tagged("b"),
                ColorLibrary.LightBlue,
                TextAnchor.MiddleCenter
            );

            var contentRect = new Rect(0f, Text.SmallFontHeight, region.width, region.height - Text.SmallFontHeight);

            GUI.BeginGroup(contentRect);

            if (Data.HealthReports.Count <= 0)
            {
                SettingsHelper.DrawColoredLabel(
                    contentRect.AtZero(),
                    noReportsText,
                    new Color(0.39f, 0.39f, 0.39f),
                    TextAnchor.MiddleCenter
                );
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

            healthScrollPos = GUI.BeginScrollView(region, healthScrollPos, viewRect);

            var y = 0f;
            HealthReport toRemove = null;
            for (int index = Data.HealthReports.Count - 1; index >= 0; index--)
            {
                HealthReport report = Data.HealthReports[index];

                if (report.Height <= 0)
                {
                    report.Height = Text.CalcHeight(report.Message, region.width - 20f);
                }

                var lineRect = new Rect(0f, y, region.width, report.Height);
                y += report.Height;

                if (!lineRect.IsRegionVisible(region, healthScrollPos))
                {
                    continue;
                }

                GUI.BeginGroup(lineRect);
                DrawHealthReport(lineRect.AtZero(), report, index % 2 == 0);
                GUI.EndGroup();

                if (lineRect.WasLeftClicked())
                {
                    toRemove = report;
                }
            }

            if (toRemove != null)
            {
                Data.HealthReports.Remove(toRemove);
            }

            GUI.EndScrollView();
        }

        private void DrawHealthReport(Rect region, [NotNull] HealthReport report, bool alternate = false)
        {
            var iconRect = new Rect(0f, 0f, 16f, region.height);
            var messageRect = new Rect(
                iconRect.x + iconRect.width + 2f,
                0f,
                region.width - iconRect.width,
                region.height
            );

            if (alternate)
            {
                Widgets.DrawLightHighlight(region);
            }

            Widgets.DrawLightHighlight(iconRect);

            Texture2D texture;
            Color color = Color.white;
            switch (report.Type)
            {
                case HealthReport.ReportType.Info:
                    texture = Textures.Info;
                    color = ColorLibrary.PaleGreen;
                    break;
                case HealthReport.ReportType.Warning:
                    texture = Textures.Warning;
                    color = ColorLibrary.Yellow;
                    break;
                case HealthReport.ReportType.Error:
                    texture = Textures.Warning;
                    color = ColorLibrary.Salmon;
                    break;
                case HealthReport.ReportType.Debug:
                    texture = Textures.Debug;
                    color = ColorLibrary.LightPink;
                    break;
                default:
                    texture = null;
                    break;
            }

            if (texture != null)
            {
                texture.DrawColored(SettingsHelper.RectForIcon(iconRect), color);
                iconRect.TipRegion($"TKUtils.MainTabTooltips.{report.Type}".Localize());
            }

            SettingsHelper.DrawColoredLabel(messageRect, report.Message, color);

            if (!Mouse.IsOver(messageRect))
            {
                report.OccurredAtString = GetTextString(DateTime.Now - report.OccurredAt);
            }

            messageRect.TipRegion(
                "TKUtils.MainTabTooltips.Report".LocalizeKeyed(report.Reporter, report.OccurredAtString)
            );
        }

        private void DrawRightColumn(Rect region)
        {
            var titleRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            SettingsHelper.DrawColoredLabel(
                titleRect,
                "TKUtils.MainTab.QuickActions".Localize().Tagged("b"),
                ColorLibrary.LightBlue,
                TextAnchor.MiddleCenter
            );

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

                var lineRect = new Rect(0f, buttonHeight * index, region.width, buttonHeight);

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
                return $"{span.TotalHours:N2} {hoursText}";
            }

            return span.Minutes > 0 ? $"{span.TotalMinutes:N2} {minutesText}" : $"{span.TotalSeconds:N2} {secondsText}";
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
                    error =
                        $@"The addon menu for ""{cache.Label}"" couldn't open successfully -- Reason: {e.GetType().Name}({e.Message})";
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

                cache.Options.Insert(
                    0,
                    new FloatMenuOption(settingsTranslated, () => SettingsHelper.OpenSettingsMenuFor(mod))
                );

                return cache;
            }
        }
    }
}

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
using System.Threading;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils;

[UsedImplicitly]
public class TkUtils : Mod
{
    public TkUtils(ModContentPack content) : base(content)
    {
        Instance = this;
        GetSettings<TkSettings>();

        try
        {
            Logger = new RimThreadedLogger(Content.Name);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        SettingsWindow = new UtilsSettingsWindow();
        Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
    }

    public static TkUtils Instance { get; private set; }
    internal static SynchronizationContext Context { get; set; }
    public static RimLogger Logger { get; private set; }
    internal UtilsSettingsWindow SettingsWindow { get; set; }

    /// <inheritdoc />
    public override string SettingsCategory() => Content.Name;

    public static void HandleException(Exception exception, string? reporter = null)
    {
        HandleException(exception.Message ?? "An unhandled exception occurred", exception, reporter);
    }

    public static void HandleException(string? message, Exception exception, string? reporter = null)
    {
        if (UnityData.IsInMainThread && TkSettings.VisualExceptions && VisualExceptions.Active)
        {
            VisualExceptions.HandleException(exception);

            return;
        }

        string exceptionMessage = message ?? exception.Message ?? "An unhandled exception occurred";
        Logger.Error(exceptionMessage, exception);

        Data.RegisterHealthReport(
            new HealthReport
            {
                Message = $"{exceptionMessage} :: Reason: {exception.GetType().Name}({exception.Message})",
                Stacktrace = StackTraceUtility.ExtractStringFromException(exception),
                Type = HealthReport.ReportType.Error,
                OccurredAt = DateTime.Now,
                Reporter = reporter ?? "Unknown"
            }
        );
    }

    /// <inheritdoc />
    public override void DoSettingsWindowContents(Rect inRect)
    {
        ProxySettingsWindow.Open(new UtilsSettingsWindow());
    }
}

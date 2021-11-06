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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";

        public TkUtils(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        internal static TkUtils Instance { get; private set; }
        internal static SynchronizationContext Context { get; set; }
        public TkSettings Settings { get; }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }

        [NotNull]
        public override string SettingsCategory()
        {
            return Content.Name;
        }

        internal static void HandleException([NotNull] Exception exception, [CanBeNull] string reporter = null)
        {
            HandleException(exception.Message ?? "An unhandled exception occurred", exception, reporter);
        }

        internal static void HandleException(string message, Exception exception, [CanBeNull] string reporter = null)
        {
            if (TkSettings.VisualExceptions && VisualExceptions.Active)
            {
                VisualExceptions.HandleException(exception);
                return;
            }

            string exceptionMessage = message ?? exception.Message ?? "An unhandled exception occurred";
            LogHelper.Error(exceptionMessage, exception);
            Data.HealthReports.Add(
                new HealthReport
                {
                    Message = $"{exceptionMessage} :: Reason: {exception.GetType().Name}({exception.Message})", Stacktrace = StackTraceUtility.ExtractStringFromException(exception),
                    Type = HealthReport.ReportType.Error, OccurredAt = DateTime.Now,
                    Reporter = reporter ?? "Unknown"
                }
            );
        }
    }
}

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
using SirRandoo.CommonLib;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using SirRandoo.CommonLib.Windows;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     A <see cref="ModPlus"/> implementation that outlines the core mod
    ///     class for ToolkitUtils. This class is created and stored by
    ///     RimWorld itself.
    /// </summary>
    [UsedImplicitly]
    public class TkUtils : ModPlus
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

            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        internal static TkUtils Instance { get; private set; }
        internal static SynchronizationContext Context { get; set; }
        internal static IRimLogger Logger { get; private set; }

        /// <inheritdoc cref="ModPlus.SettingsWindow"/>
        [NotNull]
        public override ProxySettingsWindow SettingsWindow => new UtilsSettingsWindow();

        internal static void HandleException([NotNull] Exception exception, [CanBeNull] string reporter = null)
        {
            HandleException(exception.Message ?? "An unhandled exception occurred", exception, reporter);
        }

        internal static void HandleException(string message, Exception exception, [CanBeNull] string reporter = null)
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
    }
}

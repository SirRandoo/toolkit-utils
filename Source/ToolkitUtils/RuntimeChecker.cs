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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;
#if DEBUG
using System.Diagnostics;

#endif

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     The main class responsible for ensuring the environment the mod
    ///     is running it is valid.
    /// </summary>
    /// <remarks>
    ///     The main validation that takes place is ensuring overridden
    ///     events aren't set to their Twitch Toolkit defaults. Its secondary
    ///     validation is ensuring Twitch Toolkit's ticker is stopped.
    /// </remarks>
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class RuntimeChecker
    {
        static RuntimeChecker()
        {
            TkUtils.Context ??= SynchronizationContext.Current;

            TkSettings.ValidateDynamicSettings();
            TryLanceTicker();
            ValidateExpandedEvents();
        }

        private static void ValidateExpandedEvents()
        {
            var wereChanges = false;

            // TODO: Check to see if this fixes the issue described below.
            Store_IncidentEditor.LoadCopies();

            // We're not going to update this to use EventExtension
            // since it appears to wipe previous settings.
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs.Where(
                i => i.defName == "BuyPawn" || i.defName == "AddTrait" || i.defName == "RemoveTrait"
            ))
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                incident.cost = 1;
                wereChanges = true;
            }

            if (wereChanges)
            {
                Store_IncidentEditor.UpdatePriceSheet();
            }
        }

        private static void TryLanceTicker()
        {
            FieldInfo tickerField = AccessTools.Field(typeof(TwitchToolkit.TwitchToolkit), "ticker");

            if (!(tickerField.GetValue(Toolkit.Mod) is Ticker ticker))
            {
                TkUtils.Logger.Warn($"Could not lance Toolkit's ticker; it was an unexpected value of {tickerField.GetValue(Toolkit.Mod).GetType().FullDescription()}");

                return;
            }

            try
            {
                (AccessTools.Field(typeof(Ticker), "_registerThread").GetValue(ticker) as Thread)?.Interrupt();

                TkUtils.Logger.Warn(
                    new StringBuilder().Append("Successfully lanced Twitch Toolkit's ticker.\n")
                       .Append("A message from RimWorld about discarding an unnamed def can be safely ignored.\n")
                       .Append("An exception about aborting a thread can be safely ignored.")
                       .ToString()
                );
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not abort Toolkit's ticker thread", e);
            }

            ticker.timer?.Change(0, 0);
            ticker.Discard(true);
            tickerField.SetValue(Toolkit.Mod, null);
        }

        internal static void Execute(string command, [NotNull] Action func)
        {
        #if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
        #endif

            func();

        #if DEBUG
            stopwatch.Stop();
            TkUtils.Logger.Debug($"Command {command} finished in {stopwatch.ElapsedMilliseconds:0.000}ms");
        #endif
        }
    }
}

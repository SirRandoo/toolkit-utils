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
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

#if DEBUG
using System.Diagnostics;

#endif

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class RuntimeChecker
    {
        static RuntimeChecker()
        {
            Do13Patches = ModLister.GetActiveModWithIdentifier("hodlhodl.twitchtoolkit")?.VersionCompatible != true;
            TkSettings.ValidateDynamicSettings();
            TkUtils.Context ??= SynchronizationContext.Current;

            FieldInfo tickerField = AccessTools.Field(typeof(TwitchToolkit.TwitchToolkit), "ticker");

            if (tickerField.GetValue(Toolkit.Mod) is Ticker ticker)
            {
                try
                {
                    (AccessTools.Field(typeof(Ticker), "_registerThread").GetValue(ticker) as Thread)?.Interrupt();

                    LogHelper.Warn(
                        new StringBuilder().Append("Successfully lanced Twitch Toolkit's ticker.\n")
                           .Append("A message from RimWorld about discarding an unnamed def can be safely ignored.\n")
                           .Append("An exception about aborting a thread can be safely ignored.")
                           .ToString()
                    );
                }
                catch (Exception e)
                {
                    LogHelper.Error("Could not abort Toolkit's ticker thread", e);
                }

                ticker.timer?.Change(0, 0);
                ticker.Discard(true);
                tickerField.SetValue(Toolkit.Mod, null);
            }

            var wereChanges = false;

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

        public static bool Do13Patches { get; }

        internal static void ExecuteInMainThread(string command, Action func)
        {
            if (TkSettings.MainThreadCommands && TkUtils.Context != null)
            {
                TkUtils.Context.Post(
                    delegate
                    {
                        Action action = func;
                        string commandInput = command;
                        Execute(commandInput, action);
                    },
                    null
                );
                return;
            }

            Execute(command, func);
        }

        private static void Execute(string command, [NotNull] Action func)
        {
        #if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
        #endif

            func();

        #if DEBUG
            stopwatch.Stop();
            LogHelper.Debug($"Command {command} finished in {stopwatch.ElapsedMilliseconds:0.000}ms");
        #endif
        }
    }
}

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
using System.Threading;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

#if DEBUG
using SirRandoo.ToolkitUtils.Helpers;
using System.Diagnostics;

#endif

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class RuntimeChecker
    {
        static RuntimeChecker()
        {
            TkSettings.ValidateDynamicSettings();
            TkUtils.Context ??= SynchronizationContext.Current;

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
            LogHelper.Debug($"Command {command} finished in {stopwatch.ElapsedMilliseconds}ms");
        #endif
        }
    }
}

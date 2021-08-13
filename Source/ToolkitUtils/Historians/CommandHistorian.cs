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
using System.Collections.Concurrent;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils
{
    public static class CommandHistorian
    {
        private static readonly ConcurrentDictionary<string, CommandHistory> History =
            new ConcurrentDictionary<string, CommandHistory>();

        public static void ResetHistory()
        {
            History.Clear();
        }

        public static bool IsOnCooldown([NotNull] Command command)
        {
            if (!History.TryGetValue(command.defName, out CommandHistory history))
            {
                return false;
            }

            DateTime now = DateTime.Now;
            return history.Data.Data?.HasGlobalCooldown == true
                   && (now - history.LastUsage).TotalMinutes < history.Data.Data.GlobalCooldown;
        }

        public static bool IsOnCooldown([NotNull] Command command, Viewer viewer)
        {
            if (!History.TryGetValue(command.defName, out CommandHistory history))
            {
                return false;
            }

            DateTime now = DateTime.Now;
            if (history.Data.Data?.HasGlobalCooldown == true
                && (now - history.LastUsage).TotalMinutes < history.Data.Data.GlobalCooldown)
            {
                return true;
            }

            if (history.Data.Data?.HasLocalCooldown == true
                && history.LastViewerUsages.TryGetValue(viewer.username.ToLowerInvariant(), out DateTime lastUsage))
            {
                return (now - lastUsage).TotalMinutes < history.Data.Data.LocalCooldown;
            }

            return true;
        }

        public static void RegisterUsage([NotNull] Command command)
        {
            History.AddOrUpdate(
                command.defName,
                new CommandHistory
                {
                    Command = command,
                    Data = CommandItem.FromToolkit(command),
                    DefName = command.defName,
                    LastUsage = DateTime.Now
                },
                delegate(string s, CommandHistory history)
                {
                    history.LastUsage = DateTime.Now;
                    return history;
                }
            );
        }

        public static void RegisterUsage([NotNull] Command command, [NotNull] Viewer viewer)
        {
            if (!History.TryGetValue(command.defName, out CommandHistory history))
            {
                history = new CommandHistory
                {
                    DefName = command.defName,
                    Command = command,
                    Data = CommandItem.FromToolkit(command),
                    LastUsage = DateTime.Now
                };

                History.TryAdd(command.defName, history);
            }

            history.LastViewerUsages.TryAdd(viewer.username.ToLowerInvariant(), DateTime.Now);
        }
    }
}

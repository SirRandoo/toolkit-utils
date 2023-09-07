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

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;

namespace SirRandoo.ToolkitUtils;

/// <summary>
///     The main class responsible for cataloguing command and event
///     usage, as well as determining if they're on a cooldown for the
///     given user, or globally.
/// </summary>
public static class UsageService
{
    private static readonly UsageWorker<EventItem> EventWorker = new UsageWorker<EventItem>();
    private static readonly UsageWorker<CommandItem> CommandWorker = new UsageWorker<CommandItem>();

    /// <summary>
    ///     Records an event usage for the given user.
    /// </summary>
    /// <param name="event">The event to record the usage of</param>
    /// <param name="user">The user that used the event</param>
    public static void RecordUsage(EventItem @event, string user)
    {
        EventWorker.RecordUsage(@event, user);
    }

    /// <summary>
    ///     Records a command usage for the given user.
    /// </summary>
    /// <param name="command">The command to record the usage of</param>
    /// <param name="user">The user that used the command</param>
    public static void RecordUsage(CommandItem command, string user)
    {
        CommandWorker.RecordUsage(command, user);
    }

    /// <summary>
    ///     Whether the given event is on cooldown for the user.
    /// </summary>
    /// <param name="event">The event to check</param>
    /// <param name="user">The user to check</param>
    public static bool IsOnCooldown(EventItem @event, string user) => EventWorker.IsOnCooldown(@event, user);

    /// <summary>
    ///     Whether the given command is on cooldown for the user.
    /// </summary>
    /// <param name="command">The command to check</param>
    /// <param name="user">The user to check</param>
    public static bool IsOnCooldown(CommandItem command, string user) => CommandWorker.IsOnCooldown(command, user);
}
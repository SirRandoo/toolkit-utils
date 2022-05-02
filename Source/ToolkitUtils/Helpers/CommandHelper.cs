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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using UnityEngine;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class CommandHelper
    {
        public static void Execute([NotNull] this Command command, [NotNull] ITwitchMessage message, bool emojiOverride = false)
        {
            if (command.requiresAdmin && !message.HasBadges("broadcaster"))
            {
                return;
            }

            if (command.requiresMod && !message.HasBadges("broadcaster", "moderator", "global_mod", "staff"))
            {
                return;
            }

            CommandItem item = Data.Commands.Find(c => string.Equals(c.DefName, command.defName));

            if (item != null && UsageService.IsOnCooldown(item, message.Username))
            {
                return;
            }

            if (emojiOverride)
            {
                bool emojis = TkSettings.Emojis;
                
                TkSettings.Emojis = false;
                ExecuteInternal(command, message);
                TkSettings.Emojis = emojis;
            }
            else
            {
                ExecuteInternal(command, message);
            }

            if (item != null)
            {
                UsageService.RecordUsage(item, message.Username);
            }
        }

        private static void ExecuteInternal([NotNull] Command command, ITwitchMessage message)
        {
            try
            {
                command.RunCommand(message);
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error($@"Command ""{command.command}"" threw an exception!", e);

                Data.RegisterHealthReport(
                    new HealthReport
                    {
                        Message = $@"Command ""{message.Message} ({command.command})"" didn't execute successfully. Reason: {e.GetType().Name}({e.Message})",
                        OccurredAt = DateTime.Now,
                        Reporter = "ToolkitUtils - Command Handler",
                        Type = HealthReport.ReportType.Error,
                        Stacktrace = StackTraceUtility.ExtractStringFromException(e)
                    }
                );
            }
        }

        [NotNull]
        internal static string ValidatePrefix(string prefix)
        {
            if (prefix.StartsWith("/") || prefix.StartsWith("."))
            {
                prefix = prefix.Substring(1);
            }

            return prefix.Replace(" ", "");
        }
    }
}

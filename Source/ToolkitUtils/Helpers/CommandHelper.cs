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
using JetBrains.Annotations;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class CommandHelper
    {
        internal static readonly HashSet<string> ModeratorBadges =
            new HashSet<string> {"moderator", "admin", "broadcaster", "global_mod", "staff"};

        public static void Execute(
            [NotNull] this Command command,
            [NotNull] ITwitchMessage message,
            bool emojiOverride = false
        )
        {
            if (command.requiresAdmin && !message.ChatMessage.IsBroadcaster)
            {
                return;
            }

            bool hasModBadge = message.ChatMessage.Badges.Select(p => p.Key).Any(i => ModeratorBadges.Contains(i));
            if (command.requiresMod && !hasModBadge)
            {
                return;
            }

            RuntimeChecker.ExecuteInMainThread(
                $"{command.command}[{message.Message}]",
                delegate
                {
                    bool emojis = TkSettings.Emojis;

                    try
                    {
                        if (emojiOverride)
                        {
                            TkSettings.Emojis = false;
                        }

                        command.RunCommand(message);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error($@"Command ""{command.command}"" threw an exception!", e);
                    }
                    finally
                    {
                        TkSettings.Emojis = emojis;
                    }
                }
            );
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

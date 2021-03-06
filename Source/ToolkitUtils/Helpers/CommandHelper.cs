﻿// ToolkitUtils
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

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class CommandHelper
    {
        public static void Execute(
            [NotNull] this Command command,
            [NotNull] ITwitchMessage message,
            bool emojiOverride = false
        )
        {
            UserData data = UserRegistry.GetData(message.Username);

            if (command.requiresAdmin && data?.IsBroadcaster != true)
            {
                return;
            }

            if (command.requiresMod && data?.IsModerator != true)
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

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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(CommandsHandler), "CheckCommand")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class CommandsHandlerPatch
    {
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            if (!TkSettings.Commands)
            {
                return true;
            }

            if (twitchMessage?.Message == null)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
            viewer.last_seen = DateTime.Now;

            if (viewer.IsBanned)
            {
                return false;
            }

            string message = twitchMessage.Message;

            if (!message.StartsWith(TkSettings.Prefix, StringComparison.InvariantCultureIgnoreCase)
                && !message.StartsWith(TkSettings.BuyPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            string sanitized = GetCommandString(message);

            if (sanitized == null)
            {
                return false;
            }

            List<string> segments = CommandFilter.Parse(sanitized).ToList();
            bool text = segments.Any(i => i.EqualsIgnoreCase("--text"))
                        || twitchMessage is ChatMessage chatMessage
                        && chatMessage.BotUsername.Equals("puppeteer", StringComparison.InvariantCultureIgnoreCase);

            if (segments.Count <= 0)
            {
                return false;
            }

            if (segments.First().StartsWith("/w"))
            {
                segments = segments.Skip(1).ToList();
            }

            if (text)
            {
                segments = segments.Where(i => !i.EqualsIgnoreCase("--text")).ToList();
            }

            LocateCommand(segments.ToArray())
              ?.Execute(twitchMessage.WithMessage("!" + CombineSegments(segments).Trim())!, text);
            return false;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [CanBeNull]
        public static Exception Finalizer([CanBeNull] Exception __exception)
        {
            if (__exception != null)
            {
                LogHelper.Error("Command parser encountered an error", __exception);
            }

            return null;
        }

        [NotNull]
        private static string CombineSegments([NotNull] IEnumerable<string> segments)
        {
            return string.Join(
                " ",
                segments.Select(s => s.Contains(' ') ? $@"""{s.Replace("\"", "\\\"")}""" : s).ToArray()
            );
        }

        [CanBeNull]
        private static Command LocateCommand(string[] query)
        {
            foreach (Command commandDef in DefDatabase<Command>.AllDefs.Where(c => c.enabled))
            {
                if (commandDef.command.Contains(" "))
                {
                    int spaces = commandDef.command.Count(c => c.Equals(' '));
                    string joined = string.Join(" ", query.Take(spaces));

                    if (!IsCommand(commandDef.command, joined))
                    {
                        continue;
                    }

                    return commandDef;
                }

                if (!IsCommand(commandDef.command, query.Take(1).First()))
                {
                    continue;
                }

                return commandDef;
            }

            return null;
        }

        private static bool IsCommand(string command, string input)
        {
            if (TkSettings.ToolkitStyleCommands
                && input.StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return input.EqualsIgnoreCase(command);
        }

        [CanBeNull]
        private static string GetCommandString(string message)
        {
            if (message.StartsWith("/w"))
            {
                message = message.Substring(3);
            }

            if (message.StartsWith(TkSettings.Prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return message.Substring(TkSettings.Prefix.Length);
            }

            return message.StartsWith(TkSettings.BuyPrefix, StringComparison.InvariantCultureIgnoreCase)
                ? $"{CommandDefOf.Buy.command} {message.Substring(TkSettings.BuyPrefix.Length)}"
                : null;
        }
    }
}

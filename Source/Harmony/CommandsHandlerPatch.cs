using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using ToolkitCore.Utilities;
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

            if (message.StartsWith(TkSettings.Prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                message = message.Substring(TkSettings.Prefix.Length);
            }

            List<string> segments = CommandFilter.Parse(message).ToList();
            bool unemoji = segments.Any(i => i.EqualsIgnoreCase("--text"));

            if (segments.Count <= 0)
            {
                return false;
            }

            if (segments.First().StartsWith("/w"))
            {
                segments = segments.Skip(1).ToList();
            }

            if (unemoji)
            {
                segments = segments.Where(i => !i.EqualsIgnoreCase("--text")).ToList();
            }

            LocateCommand(segments.ToArray())
              ?.Execute(twitchMessage.WithMessage("!" + CombineSegments(segments).Trim()), unemoji);
            return false;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                TkLogger.Error("Command parser encountered an error", __exception);
            }

            return null;
        }

        private static string CombineSegments(IEnumerable<string> segments)
        {
            return string.Join(
                " ",
                segments.Select(s => s.Contains(' ') ? $@"""{s.Replace("\"", "\\\"")}""" : s).ToArray()
            );
        }

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
    }
}

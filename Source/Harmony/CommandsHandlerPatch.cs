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
    public static class CommandsHandlerPatch
    {
        [UsedImplicitly]
        public static bool Prefix(ITwitchMessage msg)
        {
            if (!TkSettings.Commands)
            {
                return true;
            }

            if (msg?.Message == null)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(msg.Username);
            viewer.last_seen = DateTime.Now;

            if (viewer.IsBanned)
            {
                return false;
            }

            string message = msg.Message;

            if (message.StartsWith(TkSettings.Prefix))
            {
                message = message.Substring(1);
            }

            string[] segments = CommandFilter.Parse(message).ToArray();
            bool unemoji = segments.Any(i => i.EqualsIgnoreCase("--text"));

            if (segments.Length <= 0)
            {
                return false;
            }

            if (segments.First().StartsWith("/w"))
            {
                segments = segments.Skip(1).ToArray();
            }

            if (unemoji)
            {
                segments = segments.Where(i => !i.EqualsIgnoreCase("--text")).ToArray();
            }

            LocateCommand(segments)?.Execute(msg.WithMessage(CombineSegments(segments).Trim()));
            return false;
        }

        [UsedImplicitly]
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
                segments.Select(segment => segment.Contains(' ') ? $@"""{segment.Replace("\"", "\\\"")}""" : segment)
                   .ToArray()
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

                    if (!commandDef.command.EqualsIgnoreCase(joined))
                    {
                        continue;
                    }

                    return commandDef;
                }

                if (!commandDef.command.EqualsIgnoreCase(query.Take(1).First()))
                {
                    continue;
                }

                return commandDef;
            }

            return null;
        }
    }
}

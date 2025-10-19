// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Patches. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitCore.Utilities;
using ToolkitUtils.Api;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;
using Command = TwitchToolkit.Command;

namespace ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for adjusting how Twitch Toolkit's command parsing code is performed. This patch is
///     responsible for performing case-insensitive comparisons against command names, as well as powering the
///     <see cref="TkSettings.BuyPrefix" /> code.
/// </summary>
[PublicAPI]
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class CommandsHandlerPatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(CommandsHandlerPatch));

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(CommandsHandler), nameof(CommandsHandler.CheckCommand));
    }

    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not patch {Method} :: A lot of 'shortcut' commands won't work properly", original.FullDescription());

        return null;
    }

    private static bool Prefix(ITwitchMessage? twitchMessage)
    {
        if (twitchMessage?.Message == null) return false;

        Viewer? viewer = Viewers.GetViewer(twitchMessage.Username);
        viewer.last_seen = DateTime.UtcNow;

        if (viewer.IsBanned) return false;

        string? sanitized = GetCommandString(twitchMessage.Message);

        if (sanitized == null) return false;

        List<string> segments = CommandFilter.Parse(sanitized).ToList();
        bool text = segments.Any(i => i.EqualsIgnoreCase("--text"));

        if (segments.Count <= 0) return false;

        if (text) segments = segments.Where(i => !i.EqualsIgnoreCase("--text")).ToList();

        // LocateCommand(segments.ToArray())?.Execute(twitchMessage.SetMessage("!" + CombineSegments(segments).Trim())!, text);

        return false;
    }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
    private static Exception? Finalizer(Exception? __exception)
    {
        if (__exception != null) Logger.Error(__exception, message: "Command parser encountered an error");

        return null;
    }

    private static string CombineSegments(IEnumerable<string> segments)
    {
        return string.Join(separator: " ", segments.Select(s => s.Contains(' ') ? s.Replace(oldValue: "\"", newValue: "\\\"") : s).ToArray());
    }

    private static Command? LocateCommand(string[] query)
    {
        foreach (Command commandDef in DefDatabase<Command>.AllDefs.Where(c => c.enabled))
        {
            if (commandDef.command.Contains(" "))
            {
                int spaces = commandDef.command.Count(c => c.Equals(' '));
                string joined = string.Join(separator: " ", query.Take(spaces));

                if (!IsCommand(commandDef.command, joined)) continue;

                return commandDef;
            }

            if (!IsCommand(commandDef.command, query.Take(1).First())) continue;

            return commandDef;
        }

        return null;
    }

    private static bool IsCommand(string command, string input) =>
        // FIXME
        // if (SettingsRegistry.Command.Settings.Classic && input.StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
        // {
        // return true;
        // }
        input.EqualsIgnoreCase(command);

    private static string? GetCommandString(string message)
    {
        if (message.StartsWith("/w")) message = message[3..];

        // if (message.StartsWith(SettingsRegistry.Command.Settings.Prefix, StringComparison.InvariantCultureIgnoreCase))
        // {
        // return message[SettingsRegistry.Command.Settings.Prefix.Length..];
        // }

        return null; // FIXME

        // return message.StartsWith(SettingsRegistry.Command.Settings.BuyPrefix, StringComparison.InvariantCultureIgnoreCase)
        //     ? $"{CommandDefOfs.Buy.command} {message[SettingsRegistry.Command.Settings.BuyPrefix.Length..]}"
        //     : null;
    }
}

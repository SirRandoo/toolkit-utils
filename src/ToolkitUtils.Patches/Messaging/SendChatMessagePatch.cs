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
using System.Reflection;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitCore;
using ToolkitUtils.Api;
using TwitchLib.Client.Models;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for breaking chat messages sent by the mod, or addons, into chunks if it'd exceed the message
///     limit.
/// </summary>
/// <inheritdoc cref="DisablerPatch" path="/remarks[@id='patch']" />
[HarmonyPatch]
[UsedImplicitly(targetFlags: ImplicitUseTargetFlags.WithMembers)]
internal static class SendChatMessagePatch
{
    private const int MessageLimit = 500;
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(SendChatMessagePatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(TwitchWrapper), nameof(TwitchWrapper.SendChatMessage));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not apply patch for 'long message splitting' :: Expect commands to be unresponsive when the message is too large.");

        return null;
    }

    [UsedImplicitly]
    private static bool Prefix(string? message)
    {
        if (message.NullOrEmpty()) return false;

        message = message!.Replace(oldValue: "@", newValue: "");
        JoinedChannel channel = TwitchWrapper.Client.GetJoinedChannel(channel: ToolkitCoreSettings.channel_username);

        foreach (string segment in SplitMessages(message: message)) TwitchWrapper.Client.SendMessage(channel, segment);

        return false;
    }

    private static IEnumerable<string> SplitMessages(string message)
    {
        if (message.Length < MessageLimit)
        {
            yield return message.StripTags();

            yield break;
        }

        string[] words = message.StripTags().Split([' ',], StringSplitOptions.RemoveEmptyEntries);
        var builder = new StringBuilder();
        var chars = 0;

        foreach (string word in words)
        {
            if (chars + word.Length <= MessageLimit - 3)
            {
                builder.Append($"{word} ");
                chars += word.Length + 1;
            }
            else
            {
                builder.Append(value: "...");

                yield return builder.ToString();
                builder.Clear();
                chars = 0;
            }
        }

        if (builder.Length <= 0) yield break;

        yield return builder.ToString();
        builder.Clear();
    }
}

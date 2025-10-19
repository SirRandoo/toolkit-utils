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
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Extensions;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Twitch;
using UnityEngine;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

// TODO: This patch is obsolete since the mod has a different system for tracking viewers.
// TODO: This patch is obsolete since the mod has a different system for tracking pawns assigned to viewers.

/// <summary>A Harmony patch for populating viewer data from messages sent in chat.</summary>
[HarmonyPatch]
[UsedImplicitly(targetFlags: ImplicitUseTargetFlags.WithMembers)]
internal static class ViewerUpdaterPatch
{
    private const string PatchId = "patches:viewer.updater";
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(ViewerUpdaterPatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(ViewerUpdater), nameof(ViewerUpdater.ParseMessage));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, $"Could not apply {PatchId} ; Except pawns to disconnect from viewers!");

        return null;
    }

    [UsedImplicitly]
    private static bool Prefix(ITwitchMessage? twitchMessage)
    {
        if (twitchMessage?.ChatMessage == null) return false;

        Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
        var component = Current.Game.GetComponent<GameComponentPawns>();

        ToolkitSettings.ViewerColorCodes[twitchMessage.Username.ToLowerInvariant()] = twitchMessage.ChatMessage.ColorHex;

        // TODO: For future implementations the mod shouldn't parse the user's color every message.
        // The color system should instead just track the previous color, then compare the current color to it.
        // If the colors are different, then the color should be parsed and the tracked data updated.

        if (
            // SettingsRegistry.Cosmetic.Settings.DyeHair
            // &&
            component.HasUserBeenNamed(twitchMessage.Username) && ColorUtility.TryParseHtmlString(twitchMessage.ChatMessage.ColorHex, out Color hairColor))
        {
            Pawn pawn = component.PawnAssignedToUser(twitchMessage.Username);

            if (pawn?.story != null) pawn.story.HairColor = hairColor;
        }

        viewer.mod = twitchMessage.ChatMessage.HasBadges("moderator", "broadcaster", "global_mod", "staff");
        viewer.subscriber = twitchMessage.ChatMessage.HasBadges("subscriber", "founder");
        viewer.vip = twitchMessage.ChatMessage.HasBadges("vip");

        return false;
    }

    private static void UpdateBroadcasterData(Viewer viewer)
    {
        // TODO: Implement viewer updating.
        // if (!TkSettings.BroadcasterCoinType.EqualsIgnoreCase("broadcaster"))
        // {
        // viewer.subscriber = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("subscriber");
        // viewer.mod = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("moderator");
        // viewer.vip = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("vip");
        // }
        // else
        // {
        // viewer.subscriber = true;
        // viewer.mod = true;
        // viewer.vip = true;
        // }
    }
}

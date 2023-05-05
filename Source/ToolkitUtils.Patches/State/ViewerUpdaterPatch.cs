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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Twitch;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for populating viewer data from messages sent in
    ///     chat.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class ViewerUpdaterPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(ViewerUpdater), nameof(ViewerUpdater.ParseMessage));
        }

        [CanBeNull]
        private static Exception Cleanup(MethodBase original, [CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

            return null;
        }

        private static bool Prefix([CanBeNull] ITwitchMessage twitchMessage)
        {
            if (twitchMessage?.ChatMessage == null)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
            var component = Current.Game.GetComponent<GameComponentPawns>();

            ToolkitSettings.ViewerColorCodes[twitchMessage.Username.ToLowerInvariant()] = twitchMessage.ChatMessage.ColorHex;

            if (TkSettings.HairColor && component.HasUserBeenNamed(twitchMessage.Username)
                && ColorUtility.TryParseHtmlString(twitchMessage.ChatMessage.ColorHex, out Color hairColor))
            {
                Pawn pawn = component.PawnAssignedToUser(twitchMessage.Username);

                if (pawn?.story != null)
                {
                    pawn.story.HairColor = hairColor;
                }
            }

            viewer.mod = twitchMessage.HasBadges("moderator", "broadcaster", "global_mod", "staff");
            viewer.subscriber = twitchMessage.HasBadges("subscriber", "founder");
            viewer.vip = twitchMessage.HasBadges("vip");

            if (!Data.RegisterViewer(viewer.username))
            {
                TkUtils.Logger.Warn($"Viewer {viewer.username} could not be added to the viewer list through participation.");
            }

            return false;
        }

        private static void UpdateBroadcasterData([NotNull] Viewer viewer)
        {
            if (!TkSettings.BroadcasterCoinType.EqualsIgnoreCase("broadcaster"))
            {
                viewer.subscriber = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("subscriber");
                viewer.mod = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("moderator");
                viewer.vip = TkSettings.BroadcasterCoinType.EqualsIgnoreCase("vip");
            }
            else
            {
                viewer.subscriber = true;
                viewer.mod = true;
                viewer.vip = true;
            }
        }
    }
}

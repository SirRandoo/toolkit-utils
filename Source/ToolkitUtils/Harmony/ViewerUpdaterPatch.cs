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

using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Twitch;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(ViewerUpdater), "ParseMessage")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class ViewerUpdaterPatch
    {
        public static bool Prefix([CanBeNull] ITwitchMessage twitchMessage)
        {
            if (twitchMessage?.ChatMessage == null)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
            var component = Current.Game.GetComponent<GameComponentPawns>();

            ToolkitSettings.ViewerColorCodes[twitchMessage.Username.ToLowerInvariant()] =
                twitchMessage.ChatMessage.ColorHex;

            if (TkSettings.HairColor
                && component.HasUserBeenNamed(twitchMessage.Username)
                && ColorUtility.TryParseHtmlString(twitchMessage.ChatMessage.ColorHex, out Color hairColor))
            {
                component.PawnAssignedToUser(twitchMessage.Username).story.hairColor = hairColor;
            }

            UserData data = UserRegistry.UpdateData(twitchMessage);

            if (data == null)
            {
                return false;
            }

            viewer.mod = data.IsModerator;
            viewer.subscriber = data.IsSubscriber;
            viewer.vip = data.IsVip;
            return false;
        }
    }
}

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

using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Commands.ViewerCommands;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Buy), "RunCommand")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class BuyPatch
    {
        private static string buyCommand;

        public static bool Prefix(CommandDriver __instance, ITwitchMessage twitchMessage)
        {
            if (__instance == null)
            {
                return true;
            }

            if (!TkSettings.StoreState)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
            ITwitchMessage message = twitchMessage;

            if (!__instance.command.defName.Equals("Buy"))
            {
                buyCommand ??= Verse.DefDatabase<Command>.GetNamed("Buy", false).command;
                message = twitchMessage.WithMessage($"!{buyCommand} {twitchMessage.Message.Substring(1)}");
            }
            else
            {
                buyCommand = __instance.command.command;
            }

            if (message.Message.Split(' ').Length < 2)
            {
                return false;
            }

            Purchase_Handler.ResolvePurchase(viewer, message);
            return false;
        }
    }
}

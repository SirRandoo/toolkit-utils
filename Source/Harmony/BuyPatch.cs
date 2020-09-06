﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public static class BuyPatch
    {
        private static string buyCommand;

        [UsedImplicitly]
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
                buyCommand ??= Verse.DefDatabase<Command>.AllDefs.FirstOrDefault(c => c.defName.Equals("Buy"))?.command
                               ?? "buy";

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

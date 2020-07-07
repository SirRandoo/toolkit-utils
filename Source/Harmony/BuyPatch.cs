using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Commands.ViewerCommands;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Buy), "RunCommand")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [UsedImplicitly]
    public class BuyPatch
    {
        private static string buyCommand;

        [HarmonyPrefix]
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
                buyCommand ??=
                    Verse.DefDatabase<Command>.AllDefsListForReading.FirstOrDefault(c => c.defName.Equals("Buy"))
                        ?.command
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

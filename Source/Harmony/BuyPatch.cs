using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Commands.ViewerCommands;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Buy), "RunCommand")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class BuyPatch
    {
        [HarmonyPrefix]
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
                message = twitchMessage.WithMessage($"!buy {twitchMessage.Message.Substring(1)}");
            }

            if (message.Message.Split(' ').Length < 2)
            {
                return false;
            }

            // TkLogger.Info($"{twitchMessage.Message} → {message.Message}");
            Purchase_Handler.ResolvePurchase(viewer, message);

            return false;
        }
    }
}

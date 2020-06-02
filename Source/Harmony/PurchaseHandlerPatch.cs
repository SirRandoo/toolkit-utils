using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Purchase_Handler), "ResolvePurchase")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PurchaseHandlerPatch
    {
        [HarmonyFinalizer]
        public static Exception Finalizer(Exception __exception, Viewer viewer, ITwitchMessage twitchMessage)
        {
            if (__exception == null)
            {
                return null;
            }

            if (viewer != null && Purchase_Handler.viewerNamesDoingVariableCommands.Contains(viewer.username))
            {
                Purchase_Handler.viewerNamesDoingVariableCommands.Remove(viewer.username);
                twitchMessage.Reply("TKUtils.Responses.Exception".Translate());
            }

            TkLogger.Error("Purchase handler raised an exception!", __exception);
            return null;
        }
    }
}

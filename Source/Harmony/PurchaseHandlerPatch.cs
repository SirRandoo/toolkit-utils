using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Purchase_Handler), "ResolvePurchase")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PurchaseHandlerPatch
    {
        [HarmonyFinalizer]
        public static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                TkLogger.Error("Purchase handler raised an exception!", __exception);
            }

            return null;
        }
    }
}

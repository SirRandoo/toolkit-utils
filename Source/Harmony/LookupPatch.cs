using HarmonyLib;
using JetBrains.Annotations;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_Lookup), "ParseMessage")]
    [UsedImplicitly]
    public static class LookupPatch
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            return false;
        }
    }
}

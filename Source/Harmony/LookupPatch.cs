using HarmonyLib;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_Lookup), "ParseMessage")]
    public static class LookupPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            return false;
        }
    }
}

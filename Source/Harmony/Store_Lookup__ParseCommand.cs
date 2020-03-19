using HarmonyLib;

using TwitchLib.Client.Models;

using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_Lookup), "ParseCommand")]
    public static class Store_Lookup__ParseCommand
    {
        [HarmonyPrefix]
        public static bool ParseCommand(ChatMessage msg)
        {
            return false;
        }
    }
}

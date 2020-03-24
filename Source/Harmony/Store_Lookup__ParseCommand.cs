using HarmonyLib;
using TwitchToolkit.IRC;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_Lookup), "ParseCommand")]
    public static class Store_Lookup__ParseCommand
    {
        [HarmonyPrefix]
        public static bool ParseCommand(IRCMessage msg)
        {
            return false;
        }
    }
}

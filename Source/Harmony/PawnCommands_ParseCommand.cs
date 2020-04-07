using HarmonyLib;
using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseCommand")]
    public static class PawnCommands_ParseCommand
    {
        [HarmonyPrefix]
        public static bool ParseCommand(IRCMessage msg)
        {
            return false;
        }
    }
}

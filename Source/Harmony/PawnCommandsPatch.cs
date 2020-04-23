using HarmonyLib;
using TwitchLib.Client.Models;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseCommand")]
    public static class PawnCommandsPatch
    {
        [HarmonyPrefix]
        public static bool ParseCommand(ChatMessage msg)
        {
            return false;
        }
    }
}

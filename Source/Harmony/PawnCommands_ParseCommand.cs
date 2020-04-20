using HarmonyLib;
using TwitchLib.Client.Models;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseCommand")]
    public static class PawnCommands_ParseCommand
    {
        [HarmonyPrefix]
        public static bool ParseCommand(ChatMessage msg)
        {
            return false;
        }
    }
}

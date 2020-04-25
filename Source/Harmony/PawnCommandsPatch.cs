using HarmonyLib;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseMessage")]
    public static class PawnCommandsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            return false;
        }
    }
}

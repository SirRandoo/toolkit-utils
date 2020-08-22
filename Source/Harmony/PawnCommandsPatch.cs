using HarmonyLib;
using JetBrains.Annotations;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseMessage")]
    public static class PawnCommandsPatch
    {
        [UsedImplicitly]
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            return false;
        }
    }
}

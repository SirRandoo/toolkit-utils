using HarmonyLib;
using JetBrains.Annotations;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(PawnCommands), "ParseMessage")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class PawnCommandsPatch
    {
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            return false;
        }
    }
}

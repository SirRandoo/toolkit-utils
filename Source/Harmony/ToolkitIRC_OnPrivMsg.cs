using System.Linq;

using HarmonyLib;

using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;

using Verse;
using SirRandoo.ToolkitUtils;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(ToolkitIRC), "OnPrivMsg")]
    public static class ToolkitIRC_OnPrivMsg
    {
        [HarmonyPostfix]
        public static void OnPrivMsg(IRCMessage message)
        {
            if(!TKSettings.Linker) return;

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if(component.PawnAssignedToUser(message.User) == null)
            {
                var pawn = Find.ColonistBar.GetColonistsInOrder()
                                .Where(p => (p.Name as NameTriple).Nick.EqualsIgnoreCase(message.User));

                if(pawn.Any())
                {
                    Log.Message($"TKUtils :: Viewer link re-established for viewer:{message.User}!");
                    component.AssignUserToPawn(message.User, pawn.First());
                }
            }
        }
    }
}

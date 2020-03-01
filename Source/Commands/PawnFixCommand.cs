using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnFixCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            var pawn = GetPawn(message.User);

            if(pawn == null)
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        "TKUtils.Responses.NoPawn".Translate().Named("MESSAGE")
                    ),
                    false
                );

                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if(component != null)
            {
                component.pawnHistory[message.User] = pawn;

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        "TKUtils.Responses.PawnRelinked".Translate().Named("MESSAGE")
                    ),
                    false
                );
            }
        }
    }
}

using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnWorkCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message)) return;

            var pawn = GetPawn(message.User);

            if(pawn.workSettings != null && pawn.workSettings.EverWork)
            {
                var segments = pawn.workSettings.WorkGiversInOrderNormal?
                    .Select(w => $"{w.def.defName.CapitalizeFirst()}: {pawn.workSettings.GetPriority(w.def.workType)}")
                    .ToList();

                if(segments != null)
                {
                    SendMessage(
                        "TKUtils.Responses.Format".Translate(
                            NamedArgumentUtility.Named(message.User, "VIEWER"),
                            NamedArgumentUtility.Named(string.Join(", ", segments), "MESSAGE")
                        ),
                        message
                    );
                }
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoWork".Translate(), "MESSAGE")
                    ),
                    message
                );
            }
        }
    }
}

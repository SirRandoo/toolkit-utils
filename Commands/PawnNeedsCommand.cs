using System.Linq;

using TwitchToolkit;
using TwitchToolkit.IRC;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnNeedsCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message)) return;

            var pawn = GetPawn(message.User);

            if(pawn.needs != null)
            {
                var segments = pawn.needs.AllNeeds.Select(n => $"{n.LabelCap}: {Mathf.Round(Mathf.Clamp(n.CurLevelPercentage * 100f, 0f, 100f))}%").ToList();

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named(string.Join(", ", segments), "MESSAGE")
                    ),
                    message
                );
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoNeeds".Translate(), "MESSAGE")
                    ),
                    message
                );
            }
        }
    }
}

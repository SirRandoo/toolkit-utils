using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using TwitchToolkit.PawnQueue;
using Verse;
using Verse.AI;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InsultCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var query = CommandParser.Parse(message.Message).Skip(1).FirstOrFallback("");
            Pawn target = null;

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                var viewerPawn = Viewers.All.FirstOrDefault(v => v.username.EqualsIgnoreCase(query));

                if (viewerPawn == null)
                {
                    return;
                }

                var component = Current.Game.GetComponent<GameComponentPawns>();
                target = component.PawnAssignedToUser(viewerPawn.username.ToLowerInvariant());

                if (target == null)
                {
                    message.Reply("TKUtils.Responses.ViewerNotFound".Translate(query));
                    return;
                }
            }

            target ??= Find.ColonistBar.Entries.RandomElement().pawn;
            var job = new Job(JobDefOf.Insult, target);

            if (job.CanBeginNow(pawn))
            {
                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }
    }
}

using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using Verse;
using Verse.AI;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InsultCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");
            Pawn target = null;

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                var viewer = Viewers.All.FirstOrDefault(v => v.username.EqualsIgnoreCase(query));

                if (viewer == null)
                {
                    return;
                }

                target = GetOrFindPawn(viewer.username);

                if (target == null)
                {
                    twitchMessage.Reply("TKUtils.Responses.ViewerNotFound".Translate(query));
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

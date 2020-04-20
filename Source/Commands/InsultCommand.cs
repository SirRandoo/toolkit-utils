using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using TwitchToolkit.PawnQueue;
using Verse;
using Verse.AI;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class InsultCommand : CommandBase
    {
        private Pawn viewer;
        private Pawn target;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            viewer = GetOrFindPawn(twitchCommand.Username);

            if (viewer == null)
            {
                twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var query = CommandParser.Parse(twitchCommand.Message, TkSettings.Prefix).Skip(1).FirstOrFallback("");

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                target = GetOrFindPawn(query);

                if (target == null)
                {
                    twitchCommand.Reply("TKUtils.Responses.ViewerNotFound".Translate(query));
                    return false;
                }
            }

            target ??= Find.ColonistBar.Entries.RandomElement().pawn;
            return true;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            var job = new Job(JobDefOf.Insult, target);

            if (job.CanBeginNow(viewer))
            {
                viewer.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }

        public InsultCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}

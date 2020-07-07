using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class ResearchCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            string query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();
            ResearchProjectDef project;

            if (query.NullOrEmpty())
            {
                project = Current.Game.researchManager.currentProj;
            }
            else
            {
                project = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                    .FirstOrDefault(
                        p => p.defName.EqualsIgnoreCase(query)
                             || p.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                    );


                if (project == null)
                {
                    ThingDef thing = DefDatabase<ThingDef>.AllDefsListForReading
                        .FirstOrDefault(
                            t => t.defName.EqualsIgnoreCase(query)
                                 || t.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                        );

                    project = thing?.recipeMaker?.researchPrerequisite;
                    project ??= thing?.recipeMaker?.researchPrerequisites?.FirstOrDefault(p => !p.IsFinished);
                }
            }

            if (project == null)
            {
                twitchMessage.Reply(
                    (
                        !query.NullOrEmpty()
                            ? "TKUtils.Research.InvalidQuery".Localize(query)
                            : "TKUtils.Research.None".Localize()
                    ).WithHeader("Research".TranslateSimple())
                );
                return;
            }

            var segments = new List<string>
            {
                ResponseHelper.JoinPair(project.LabelCap, project.ProgressPercent.ToStringPercent())
            };

            if (project.prerequisites != null && !project.PrerequisitesCompleted)
            {
                List<ResearchProjectDef> prerequisites = project.prerequisites;

                string[] container = prerequisites.Where(prerequisite => !prerequisite.IsFinished)
                    .Select(
                        prerequisite => ResponseHelper.JoinPair(
                            prerequisite.LabelCap,
                            prerequisite.ProgressPercent.ToStringPercent()
                        )
                    )
                    .ToArray();

                segments.Add(ResponseHelper.JoinPair("ResearchPrerequisites".Localize(), container.SectionJoin()));
            }

            twitchMessage.Reply(segments.GroupedJoin().WithHeader("Research".Localize()));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class ResearchCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();
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
                    var thing = DefDatabase<ThingDef>.AllDefsListForReading
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
                            ? "TKUtils.Responses.Research.QueryInvalid".Translate(query)
                            : "TKUtils.Responses.Research.None".Translate()
                    ).WithHeader("Research".Translate())
                );
                return;
            }

            var segments = new List<string>
            {
                "TKUtils.Formats.KeyValue".Translate(project.LabelCap, project.ProgressPercent.ToStringPercent())
            };

            if (project.prerequisites != null && !project.PrerequisitesCompleted)
            {
                var prerequisites = project.prerequisites;

                var container = prerequisites.Where(prerequisite => !prerequisite.IsFinished)
                    .Select(
                        prerequisite => "TKUtils.Formats.KeyValue".Translate(
                            prerequisite.LabelCap,
                            prerequisite.ProgressPercent.ToStringPercent()
                        )
                    )
                    .Select(dummy => (string) dummy)
                    .ToArray();

                segments.Add($"{"ResearchPrerequisites".Translate().RawText}: {string.Join(", ", container)}");
            }

            twitchMessage.Reply(string.Join("⎮", segments).WithHeader("Research".Translate()));
        }
    }
}

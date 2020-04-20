using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class ResearchCommand : CommandBase
    {
        private ResearchProjectDef project;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            var query = CommandParser.Parse(twitchCommand.Message).Skip(1).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                project = Current.Game.researchManager.currentProj;
            }
            else
            {
                project = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FirstOrDefault(
                    p => p.defName.EqualsIgnoreCase(query) || p.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                );
            }

            if (project != null)
            {
                return true;
            }

            twitchCommand.Reply(
                (
                    !query.NullOrEmpty()
                        ? "TKUtils.Responses.Research.QueryInvalid".Translate(query)
                        : "TKUtils.Responses.Research.None".Translate()
                ).WithHeader("Research".Translate())
            );
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
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

            twitchCommand.Reply(string.Join("⎮", segments).WithHeader("Research".Translate()));
        }

        public ResearchCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}

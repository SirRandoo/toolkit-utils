using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class ResearchCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var query = message.Message.Split(' ').Skip(1).FirstOrDefault();
            ResearchProjectDef target;

            if (query.NullOrEmpty())
            {
                target = Current.Game.researchManager.currentProj;
            }
            else
            {
                target = DefDatabase<ResearchProjectDef>
                    .AllDefsListForReading
                    .FirstOrDefault(p => p.defName.EqualsIgnoreCase(query) || p.label.EqualsIgnoreCase(query));
            }

            if (target == null)
            {
                message.Reply(
                    !query.NullOrEmpty()
                        ? "TKUtils.Responses.Research.QueryInvalid".Translate(query).WithHeader("Research".Translate())
                        : "TKUtils.Responses.Research.None".Translate().WithHeader("Research".Translate())
                );

                return;
            }

            var segments = new List<string>
            {
                "TKUtils.Formats.KeyValue".Translate(target.LabelCap, target.ProgressPercent.ToStringPercent())
            };

            if (target.prerequisites != null && !target.PrerequisitesCompleted)
            {
                var prerequisites = target.prerequisites;

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

            message.Reply(string.Join("⎮", segments).WithHeader("Research".Translate()));
        }
    }
}

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
            if(!CommandsHandler.AllowCommand(message)) return;

            var query = message.Message.Substring(message.Message.IndexOf(' ') + 1);

            if(query.NullOrEmpty())
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        "TKUtils.Responses.NoResearchQuery".Translate().Named("MESSAGE")
                    ),
                    message
                );
            }

            var result = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                .Where(p => p.defName.EqualsIgnoreCase(query) || p.label.EqualsIgnoreCase(query));

            if(result.Any())
            {
                var r = result.First();
                var response = "TKUtils.Responses.Format".Translate(
                    message.User.Named("VIEWER"),
                    "TKUtils.Responses.ResearchFormat".Translate(
                        r.LabelCap.Named("RESEARCH"),
                        string.Format("{0:P2}", r.ProgressPercent).Named("PERCENT")
                    ).Named("MESSAGE")
                );

                if(r.prerequisites != null)
                {
                    var deps = r.prerequisites.Where(p => !p.IsFinished);

                    if(deps.Any())
                    {
                        response += " | ";
                        response += "TKUtils.Responses.ResearchPrerequisitesFormat".Translate(
                            string.Join(", ", deps).Named("PREREQUISITES")
                        );
                    }
                }

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        response.Named("MESSAGE")
                    ),
                    message
                );
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        message.User.Named("VIEWER"),
                        "TKUtils.Responses.NoResearch".Translate(
                            query.Named("RESEARCH")
                        ),
                        "MESSAGE"
                    ),
                    message
                );
            }
        }
    }
}

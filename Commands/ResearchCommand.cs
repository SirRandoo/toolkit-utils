using System.Linq;

using TwitchToolkit;
using TwitchToolkit.IRC;

using UnityEngine;

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
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoResearchQuery".Translate(), "MESSAGE")
                    ),
                    message
                );
            }

            var result = DefDatabase<ResearchProjectDef>.AllDefsListForReading
                .Where(p => p.defName.EqualsIgnoreCase(query) || p.label.EqualsIgnoreCase(query));

            if(result.Any())
            {
                var r = result.First();

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named($"{r.LabelCap}: {Mathf.Clamp(r.ProgressPercent, 0f, 100f) * 100f}%", "MESSAGE")
                    ),
                    message
                );
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named(
                            "TKUtils.Responses.NoResearch".Translate(
                                NamedArgumentUtility.Named(query, "RESEARCH")
                            ),
                            "MESSAGE"
                        )
                    ),
                    message
                );
            }
        }
    }
}

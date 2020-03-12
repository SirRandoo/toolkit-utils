using System.Collections.Generic;
using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class ResearchCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var query = message.Message.Split(' ').Skip(1).FirstOrDefault();
            ResearchProjectDef target;

            if(query.NullOrEmpty())
            {
                target = Current.Game.researchManager.currentProj;
            }
            else
            {
                target = DefDatabase<ResearchProjectDef>
                    .AllDefsListForReading
                    .Where(p => p.defName.EqualsIgnoreCase(query) || p.label.EqualsIgnoreCase(query))
                    .FirstOrDefault();
            }

            if(target == null)
            {
                if(!query.NullOrEmpty())
                {
                    SendCommandMessage(
                        "TKUtils.Formats.Research.Base".Translate(
                            "TKUtils.Responses.Research.QueryInvalid".Translate(
                                query.Named("PROJECT")
                            ).Named("TEXT")
                        ),
                        message
                    );
                }
                else
                {
                    SendCommandMessage(
                        "TKUtils.Responses.Research.None".Translate(),
                        message
                    );
                }
                return;
            }

            var segments = new List<string>(){
                "TKUtils.Formats.Research.Current".Translate(
                    target.LabelCap.Named("PROJECT"),
                    string.Format(
                        "{0:P2}",
                        target.ProgressPercent
                    ).Named("PERCENT")
                )
            };

            if(target.prerequisites != null && !target.PrerequisitesCompleted)
            {
                segments.Add(
                    "TKUtils.Formats.Research.Prerequisites".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            target.prerequisites
                                .Where(p => !p.IsFinished)
                                .Select(p => "TKUtils.Formats.Research.Current".Translate(
                                    p.LabelCap.Named("PROJECT"),
                                    string.Format(
                                        "{0:P2}",
                                        p.ProgressPercent
                                    ).Named("PERCENT")
                                ))
                        ).Named("PREREQUISITES")
                    )
                );
            }

            SendCommandMessage(
                "TKUtils.Formats.Research.Base".Translate(
                    string.Join(
                        "TKUtils.Misc.Separators.Upper".Translate(),
                        segments
                    ).Named("TEXT")
                ),
                message
            );
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnSkillsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var parts = new List<string>();
            var skills = pawn.skills.skills;

            foreach (var skill in skills)
            {
                var container = "";

                container += "TKUtils.Formats.KeyValue".Translate(
                        skill.def.LabelCap,
                        skill.TotallyDisabled ? "-" : skill.levelInt.ToString()
                    )
                    .RawText;

                container += string.Concat(Enumerable.Repeat("🔥".AltText("+"), (int) skill.passion));

                parts.Add(container);
            }
            
            twitchMessage.Reply(string.Join(", ", parts.ToArray()).WithHeader("StatsReport_Skills".Translate()));
        }
    }
}

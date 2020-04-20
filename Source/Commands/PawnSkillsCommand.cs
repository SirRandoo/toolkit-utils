using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnSkillsCommand : CommandBase
    {
        private Pawn pawn;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Username);

            if (pawn != null)
            {
                return true;
            }

            twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate());
            return false;
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
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
            
            twitchCommand.Reply(string.Join(", ", parts.ToArray()).WithHeader("StatsReport_Skills".Translate()));
        }

        public PawnSkillsCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}

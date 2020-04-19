using System;
using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnSkillsCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var parts = new List<string>();
            var skills = pawn.skills.skills;

            foreach (var skill in skills)
            {
                var container = "";

                container += "TKUtils.Formats.KeyValue".Translate(
                        skill.def.LabelCap,
                        skill.TotallyDisabled
                            ? "-"
                            : skill.levelInt.ToString()
                    )
                    .RawText;

                container += new string(Convert.ToChar("🔥".AltText("+")), (int) skill.passion);

                parts.Add(container);
            }

            message.Reply(string.Join(", ", parts.ToArray()).WithHeader("StatsReport_Skills".Translate()));
        }
    }
}

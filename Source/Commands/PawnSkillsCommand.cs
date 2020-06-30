using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnSkillsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate());
                return;
            }

            var parts = new List<string>();
            List<SkillRecord> skills = pawn.skills.skills;

            foreach (SkillRecord skill in skills)
            {
                var container = "";

                container += "TKUtils.Formats.KeyValue".Translate(
                        skill.def.LabelCap,
                        skill.TotallyDisabled ? "🚫".AltText("-") : skill.levelInt.ToString()
                    )
                    .RawText;

                container += !Interests.Active
                    ? string.Concat(Enumerable.Repeat("🔥".AltText("+"), (int) skill.passion))
                    : Interests.GetIconForPassion(skill);

                parts.Add(container);

                TkLogger.Info($"{skill.passion.GetType().FullName}: {skill.passion.ToString()}");
            }

            twitchMessage.Reply(string.Join(", ", parts.ToArray()).WithHeader("StatsReport_Skills".Translate()));
        }
    }
}

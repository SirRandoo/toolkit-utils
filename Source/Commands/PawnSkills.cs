using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnSkills : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var parts = new List<string>();
            List<SkillRecord> skills = pawn.skills.skills;

            foreach (SkillRecord skill in skills)
            {
                var container = "";

                container += ResponseHelper.JoinPair(
                    skill.def.LabelCap,
                    skill.TotallyDisabled ? ResponseHelper.ForbiddenGlyph.AltText("-") : skill.levelInt.ToString()
                );

                container += !Interests.Active
                    ? string.Concat(Enumerable.Repeat(ResponseHelper.FireGlyph.AltText("+"), (int) skill.passion))
                    : Interests.GetIconForPassion(skill);

                parts.Add(container);
            }

            twitchMessage.Reply(parts.SectionJoin().WithHeader("StatsReport_Skills".Localize()));
        }
    }
}

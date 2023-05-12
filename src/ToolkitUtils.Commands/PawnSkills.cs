// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using ToolkitUtils.Utils.ModComp;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnSkills : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            var parts = new List<string>();
            List<SkillRecord> skills = pawn!.skills.skills;

            foreach (SkillRecord skill in skills)
            {
                var container = "";

                container += ResponseHelper.JoinPair(skill.def.LabelCap, skill.TotallyDisabled ? ResponseHelper.ForbiddenGlyph.AltText("-") : skill.levelInt.ToString());

                container += !Interests.Active
                    ? string.Concat(Enumerable.Repeat(ResponseHelper.FireGlyph.AltText("+"), (int)skill.passion))
                    : Interests.GetIconForPassion(skill);

                parts.Add(container);
            }

            twitchMessage.Reply(parts.SectionJoin().WithHeader("StatsReport_Skills".TranslateSimple()));
        }
    }
}

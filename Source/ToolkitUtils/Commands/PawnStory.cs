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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnStory : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize().WithHeader("TabCharacter".Localize()));

                return;
            }

            var parts = new List<string> { $"{"Backstory".Localize()}: {pawn!.story.AllBackstories.Select(b => b.title.CapitalizeFirst()).SectionJoin()}" };

            if (!pawn.story.title.NullOrEmpty())
            {
                parts.Add(pawn.story.TitleCap);
            }

            bool isRoyal = pawn.royalty?.MostSeniorTitle != null;

            switch (pawn.gender)
            {
                case Gender.Female:
                    parts.Add((isRoyal ? ResponseHelper.PrincessGlyph : ResponseHelper.FemaleGlyph).AltText("Female".Localize().CapitalizeFirst()));

                    break;
                case Gender.Male:
                    parts.Add((isRoyal ? ResponseHelper.PrinceGlyph : ResponseHelper.MaleGlyph).AltText("Male".Localize().CapitalizeFirst()));

                    break;
                case Gender.None:
                    parts.Add((isRoyal ? ResponseHelper.CrownGlyph : ResponseHelper.GenderlessGlyph).AltText("NoneLower".Localize().CapitalizeFirst()));

                    break;
                default:
                    parts.Add(isRoyal ? ResponseHelper.CrownGlyph : "");

                    break;
            }

            parts.Add("AgeIndicator".LocalizeKeyed(pawn.ageTracker.AgeNumberString).CapitalizeFirst());

            WorkTags workTags = pawn.story.DisabledWorkTagsBackstoryAndTraits;

            if (workTags != WorkTags.None)
            {
                string[] filteredTags = pawn.story.DisabledWorkTagsBackstoryAndTraits.GetAllSelectedItems<WorkTags>()
                   .Where(t => t != WorkTags.None)
                   .Select(t => t.LabelTranslated().CapitalizeFirst())
                   .ToArray();

                parts.Add($"{"IncapableOf".Localize()}: {filteredTags.SectionJoin()}");
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits.Count > 0)
            {
                parts.Add($"{"Traits".Localize()}: {traits.Select(t => RichTextHelper.StripTags(t.LabelCap)).SectionJoin()}");
            }

            msg.Reply(parts.GroupedJoin().WithHeader("TabCharacter".Localize()));
        }
    }
}

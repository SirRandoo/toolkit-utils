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

#if !RW12
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;
#endif

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class GenderSwap : IncidentVariablesBase
    {
        private Pawn pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            if (pawn.kindDef?.RaceProps?.hasGenders == true || pawn.gender == Gender.None)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.GenderSwap.None".Localize());

            return false;
        }

        public override void Execute()
        {
            pawn.gender = pawn.gender == Gender.Female ? Gender.Male : Gender.Female;
            pawn.story.hairColor = PawnHairColors.RandomHairColor(pawn.story.SkinColor, pawn.ageTracker.AgeBiologicalYears);

            if (pawn.style.HasUnwantedBeard)
            {
                pawn.style.beardDef = BeardDefOf.NoBeard;
                pawn.style.Notify_StyleItemChanged();
            }

            pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);

            pawn.story.bodyType = pawn.story.adulthood == null
                ? Rand.Value >= 0.5
                    ? pawn.gender != Gender.Female
                        ? BodyTypeDefOf.Male
                        : BodyTypeDefOf.Female
                    : BodyTypeDefOf.Thin
                : pawn.story.adulthood.BodyTypeFor(pawn.gender);

            Viewer.Charge(storeIncident);

            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.GenderSwap.Complete".LocalizeKeyed(pawn.gender.ToString()));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.GenderSwapLetter.Title".Localize(),
                "TKUtils.GenderSwapLetter.Description".LocalizeKeyed(Viewer.username, pawn.gender.ToString()),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}

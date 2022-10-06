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
    public class GenderSwap : IncidentVariablesBase
    {
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            if (_pawn.kindDef?.RaceProps?.hasGenders == true || _pawn.gender == Gender.None)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.GenderSwap.None".Localize());

            return false;
        }

        public override void Execute()
        {
            _pawn.gender = _pawn.gender == Gender.Female ? Gender.Male : Gender.Female;
            _pawn.story.HairColor = PawnHairColors.RandomHairColor(_pawn, _pawn.story.SkinColor, _pawn.ageTracker.AgeBiologicalYears);

            if (_pawn.style.HasUnwantedBeard)
            {
                _pawn.style.beardDef = BeardDefOf.NoBeard;
                _pawn.style.Notify_StyleItemChanged();
            }

            _pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(_pawn);

            _pawn.story.bodyType = _pawn.story.Adulthood == null ? RandomBodyType(_pawn) : _pawn.story.Adulthood.BodyTypeFor(_pawn.gender);

            Viewer.Charge(storeIncident);

            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.GenderSwap.Complete".LocalizeKeyed(_pawn.gender.ToString()));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.GenderSwapLetter.Title".Localize(),
                "TKUtils.GenderSwapLetter.Description".LocalizeKeyed(Viewer.username, _pawn.gender.ToString()),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private static BodyTypeDef BodyTypeForGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return BodyTypeDefOf.Female;
                case Gender.Male:
                    return BodyTypeDefOf.Male;
                default:
                    return BodyTypeDefOf.Thin;
            }
        }

        private static BodyTypeDef RandomBodyType(Pawn pawn) => Rand.Value >= 0.5 ? BodyTypeForGender(pawn.gender) : BodyTypeDefOf.Thin;
    }
}

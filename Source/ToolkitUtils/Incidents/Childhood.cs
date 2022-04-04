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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class Childhood : IncidentVariablesBase
    {
        private Backstory _backstory;
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            _backstory = BackstoryDatabase.allBackstories.Values.Where(b => b.slot == BackstorySlot.Childhood)
               .Where(b => !WouldBeViolation(b))
               .InRandomOrder()
               .FirstOrDefault();

            return _backstory != null;
        }

        public override void Execute()
        {
            Backstory previous = _pawn.story.childhood;
            _pawn.story.childhood = _backstory;
            _pawn.Notify_DisabledWorkTypesChanged();
            _pawn.workSettings?.Notify_DisabledWorkTypesChanged();
            _pawn.skills?.Notify_SkillDisablesChanged();

            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Childhood.Complete".LocalizeKeyed(_backstory.TitleCapFor(_pawn.gender)));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.BackstoryLetter.Title".Localize(),
                "TKUtils.BackstoryLetter.ChildhoodDescription".LocalizeKeyed(Viewer.username, previous.TitleCapFor(_pawn.gender), _backstory.TitleCapFor(_pawn.gender)),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private bool WouldBeViolation([NotNull] Backstory story)
        {
            if (story.disallowedTraits.NullOrEmpty())
            {
                return false;
            }

            foreach (TraitEntry entry in story.disallowedTraits)
            {
                Trait trait = _pawn.story.traits.allTraits.Find(t => t.def.Equals(entry.def) && t.Degree == entry.degree);

                if (trait != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

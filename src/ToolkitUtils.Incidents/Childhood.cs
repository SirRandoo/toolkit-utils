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
using TwitchToolkit;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class Childhood : IncidentVariablesBase
    {
        private BackstoryDef _backstory;
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".TranslateSimple());

                return false;
            }

            if (!Data.Backstories.TryGetValue(BackstorySlot.Childhood, out List<BackstoryDef> backstories))
            {
                return false;
            }

            _backstory = backstories.Where(b => !WouldBeViolation(b)).InRandomOrder().FirstOrDefault();

            return _backstory != null;
        }

        public override void Execute()
        {
            BackstoryDef previous = _pawn.story.Childhood;
            _pawn.story.Childhood = _backstory;
            _pawn.Notify_DisabledWorkTypesChanged();
            _pawn.workSettings?.Notify_DisabledWorkTypesChanged();
            _pawn.skills?.Notify_SkillDisablesChanged();

            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Childhood.Complete".Translate(_backstory.TitleCapFor(_pawn.gender)));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.BackstoryLetter.Title".TranslateSimple(),
                "TKUtils.BackstoryLetter.ChildhoodDescription".Translate(Viewer.username, previous.TitleCapFor(_pawn.gender), _backstory.TitleCapFor(_pawn.gender)),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private bool WouldBeViolation([NotNull] BackstoryDef story)
        {
            if (story.disallowedTraits.NullOrEmpty())
            {
                return false;
            }

            foreach (BackstoryTrait entry in story.disallowedTraits)
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

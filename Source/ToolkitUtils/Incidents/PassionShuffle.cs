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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class PassionShuffle : IncidentVariablesBase
    {
        private Pawn _pawn;
        private SkillDef _target;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            int passions = _pawn!.skills.skills.Sum(skill => (int)skill.passion);

            if (passions <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.PassionShuffle.None".Localize());

                return false;
            }

            string query = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return _pawn.skills.skills.Any(s => (int)s.passion > (int)Passion.None);
            }

            _target = _pawn.skills.skills.FirstOrDefault(
                    s => s.def.defName.EqualsIgnoreCase(query!.ToToolkit())
                         || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                         || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                )
              ?.def;

            if (_target != null)
            {
                return _pawn.skills.skills.Any(s => (int)s.passion > (int)Passion.None);
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".LocalizeKeyed(query));

            return false;
        }

        public override void Execute()
        {
            if (Interests.Active)
            {
                ShuffleWithInterests();
            }
            else
            {
                Shuffle();
            }

            Viewer.Charge(storeIncident);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.PassionShuffle.Complete".Localize());

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionShuffleLetter.Title".Localize(),
                "TKUtils.PassionShuffleLetter.Description".LocalizeKeyed(Viewer.username),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private void Shuffle()
        {
            int passionCount = _pawn.skills.skills.Sum(s => (int)s.passion);
            var iterations = 0;

            passionCount = GetPassionCount(passionCount);
            ShufflePassions(passionCount, ref iterations);
        }

        private void ShuffleWithInterests()
        {
            var iterations = 0;
            int passionCount = _pawn.skills.skills.Select(s => (int)s.passion).Where(p => p < 3).Sum();
            List<Passion> interests = _pawn.skills.skills.Where(s => (int)s.passion >= 3).Select(s => s.passion).ToList();

            passionCount = GetPassionCount(passionCount);

            if (!ShufflePassions(passionCount, ref iterations))
            {
                return;
            }

            while (interests.Any())
            {
                SkillRecord skill = _pawn.skills.skills.Where(s => !s.TotallyDisabled).Where(s => s.passion == Passion.None).RandomElementWithFallback();

                if (skill == null)
                {
                    iterations += 1;

                    continue;
                }

                Passion interest = interests.RandomElementWithFallback();

                skill.passion = interest;

                interests.Remove(interest);
                iterations += 1;

                if (iterations < 150)
                {
                    continue;
                }

                LogHelper.Warn("Exceeded 100 iterations while shuffling interests!");

                return;
            }
        }

        private bool ShufflePassions(int passionCount, ref int iterations)
        {
            while (passionCount > 0)
            {
                SkillRecord skill = _pawn.skills.skills.Where(s => !s.TotallyDisabled).Where(s => s.passion != Passion.Major).RandomElementWithFallback();

                if (skill == null)
                {
                    iterations += 1;

                    continue;
                }

                IncreasePassionFor(skill);
                passionCount -= 1;
                iterations += 1;

                if (iterations < 150)
                {
                    continue;
                }

                LogHelper.Warn("Exceeded 100 iterations while shuffling passions!");

                return false;
            }

            return true;
        }

        private static void IncreasePassionFor([NotNull] SkillRecord skill)
        {
            switch (skill.passion)
            {
                case Passion.None:
                    skill.passion = Passion.Minor;

                    break;
                case Passion.Minor:
                    skill.passion = Passion.Major;

                    break;
            }
        }

        private int GetPassionCount(int passionCount)
        {
            foreach (SkillRecord skill in _pawn.skills.skills)
            {
                if (skill.def == _target && passionCount > 0)
                {
                    skill.passion = Passion.Minor;
                    passionCount -= 1;

                    continue;
                }

                skill.passion = Passion.None;
            }

            return passionCount;
        }
    }
}

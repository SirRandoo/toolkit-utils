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
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class RemovePassion : IncidentVariablesBase
    {
        private Pawn _pawn;
        private SkillRecord _target;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".TranslateSimple());

                return false;
            }

            string query = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            _target = _pawn!.skills.skills.Where(s => !s.TotallyDisabled)
               .FirstOrDefault(
                    s => s.def.defName.EqualsIgnoreCase(query!.ToToolkit()) || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                        || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                );

            if (_target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".Translate(query));

                return false;
            }

            if ((int)_target.passion is { } passion && passion > 0 && passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemovePassion.None".Translate(query));

            return false;
        }

        public override void Execute()
        {
            if (!global::ToolkitUtils.IncidentSettings.RemovePassion.Randomness)
            {
                _target.passion = (Passion)Mathf.Clamp((int)_target.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifySuccess(_target);

                return;
            }

            if (global::ToolkitUtils.IncidentSettings.RemovePassion.ChanceToFail.ToChance())
            {
                Viewer.Charge(storeIncident);
                NotifyFailed(_target);

                return;
            }

            if (global::ToolkitUtils.IncidentSettings.RemovePassion.ChanceToHop.ToChance() && TryGetEligibleSkill(out SkillRecord skill, true))
            {
                skill!.passion = (Passion)Mathf.Clamp((int)skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyHopped(_target, skill);

                return;
            }

            if (global::ToolkitUtils.IncidentSettings.RemovePassion.ChanceToIncrease.ToChance() && (int)_target.passion < (int)Passion.Major)
            {
                _target.passion = (Passion)Mathf.Clamp((int)_target.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyIncrease(_target);

                return;
            }

            if (global::ToolkitUtils.IncidentSettings.RemovePassion.ChanceToIncrease.ToChance() && global::ToolkitUtils.IncidentSettings.RemovePassion.ChanceToHop.ToChance() && TryGetEligibleSkill(out skill))
            {
                skill!.passion = (Passion)Mathf.Clamp((int)skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyIncreaseHopped(_target, skill);

                return;
            }

            _target.passion = (Passion)Mathf.Clamp((int)_target.passion - 1, 0, 2);
            Viewer.Charge(storeIncident);
            NotifySuccess(_target);
        }

        private void NotifySuccess([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.Complete".Translate(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".TranslateSimple(),
                "TKUtils.PassionLetter.SuccessDescription".Translate(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private void NotifyIncrease([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.Increase".Translate(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".TranslateSimple(),
                "TKUtils.PassionLetter.IncreaseDescription".Translate(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private void NotifyFailed([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Failed".Translate(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".TranslateSimple(),
                "TKUtils.PassionLetter.FailedDescription".Translate(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private void NotifyHopped([NotNull] SkillRecord viewerSkill, [NotNull] SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.Hopped".Translate(viewerSkill.def.label, randomSkill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".TranslateSimple(),
                "TKUtils.PassionLetter.RemoveHoppedDescription".Translate(Viewer.username, viewerSkill.def.label, randomSkill.def.label),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private void NotifyIncreaseHopped([NotNull] SkillRecord viewerSkill, [NotNull] SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.IncreaseHopped".Translate(viewerSkill.def.label, randomSkill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".TranslateSimple(),
                "TKUtils.PassionLetter.IncreaseHoppedDescription".Translate(Viewer.username, viewerSkill.def.label, randomSkill.def.label),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        [ContractAnnotation("=> true,skill:notnull; => false,skill:null")]
        private bool TryGetEligibleSkill(out SkillRecord skill, bool forDecrease = false)
        {
            IEnumerable<SkillRecord> filters = _pawn.skills.skills.Where(s => s != _target && !s.TotallyDisabled);

            skill = (forDecrease
                ? filters.Where(
                    s =>
                    {
                        var passionInt = (int)s.passion;

                        return passionInt > 0 && passionInt <= (int)Passion.Major;
                    }
                )
                : filters.Where(s => (int)s.passion < (int)Passion.Major)).RandomElement();

            return skill != null;
        }
    }
}

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
using ToolkitCore.Utilities;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RemovePassion : IncidentVariablesBase
    {
        private Pawn pawn;
        private SkillRecord target;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            string query = CommandFilter.Parse(msg).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            target = pawn!.skills.skills.Where(s => !s.TotallyDisabled)
               .FirstOrDefault(
                    s => s.def.defName.EqualsIgnoreCase(query!.ToToolkit())
                         || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                         || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                );

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".LocalizeKeyed(query));
                return false;
            }

            if ((int) target.passion is { } passion && passion > 0 && passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemovePassion.None".Localize());
            return false;
        }

        public override void Execute()
        {
            if (!IncidentSettings.RemovePassion.Randomness)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifySuccess(target);
                return;
            }

            if (IncidentSettings.RemovePassion.ChanceToFail.ToChance())
            {
                Viewer.Charge(storeIncident);
                NotifyFailed(target);
                return;
            }

            if (IncidentSettings.RemovePassion.ChanceToHop.ToChance()
                && TryGetEligibleSkill(out SkillRecord skill, true))
            {
                skill!.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyHopped(target, skill);
                return;
            }

            if (IncidentSettings.RemovePassion.ChanceToIncrease.ToChance()
                && (int) target.passion < (int) Passion.Major)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyIncrease(target);
                return;
            }

            if (IncidentSettings.RemovePassion.ChanceToIncrease.ToChance()
                && IncidentSettings.RemovePassion.ChanceToHop.ToChance()
                && TryGetEligibleSkill(out skill))
            {
                skill!.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyIncreaseHopped(target, skill);
                return;
            }

            target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
            Viewer.Charge(storeIncident);
            NotifySuccess(target);
        }

        private void NotifySuccess([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.Complete".LocalizeKeyed(skill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.SuccessDescription".LocalizeKeyed(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyIncrease([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.Increase".LocalizeKeyed(skill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.IncreaseDescription".LocalizeKeyed(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyFailed([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Failed".LocalizeKeyed(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.FailedDescription".LocalizeKeyed(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyHopped([NotNull] SkillRecord viewerSkill, [NotNull] SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.Hopped".LocalizeKeyed(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.RemoveHoppedDescription".LocalizeKeyed(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyIncreaseHopped([NotNull] SkillRecord viewerSkill, [NotNull] SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.IncreaseHopped".LocalizeKeyed(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.IncreaseHoppedDescription".LocalizeKeyed(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        [ContractAnnotation("=> true,skill:notnull; => false,skill:null")]
        private bool TryGetEligibleSkill(out SkillRecord skill, bool forDecrease = false)
        {
            IEnumerable<SkillRecord> filters = pawn.skills.skills.Where(s => s != target && !s.TotallyDisabled);

            skill = (forDecrease
                ? filters.Where(
                    s =>
                    {
                        var passionInt = (int) s.passion;

                        return passionInt > 0 && passionInt <= (int) Passion.Major;
                    }
                )
                : filters.Where(s => (int) s.passion < (int) Passion.Major)).RandomElement();
            return skill != null;
        }
    }
}

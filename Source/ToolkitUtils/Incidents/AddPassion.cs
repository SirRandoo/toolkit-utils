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
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class AddPassion : IncidentVariablesBase
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

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(message).Skip(2));

            if (!worker.TryGetNextAsSkill(out SkillDef skillDef))
            {
                return false;
            }

            target = pawn!.skills.skills.Where(s => !s.TotallyDisabled).FirstOrDefault(s => s.def.Equals(skillDef));

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if ((int) target.passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Passion.Full".Localize());
            return false;
        }

        public override void Execute()
        {
            if (!IncidentSettings.AddPassion.Randomness)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 2);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToFail.ToChance())
            {
                Viewer.Charge(storeIncident);
                NotifyFailed(target);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToHop.ToChance() && TryGetEligibleSkill(out SkillRecord skill))
            {
                skill!.passion = (Passion) Mathf.Clamp((int) skill.passion + 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyHopped(target, skill);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToDecrease.ToChance()
                && (target.passion == Passion.Minor || target.passion == Passion.Major))
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyDecrease(target);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToDecrease.ToChance()
                && IncidentSettings.AddPassion.ChanceToHop.ToChance()
                && TryGetEligibleSkill(out skill, true))
            {
                skill!.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyDecreaseHopped(target, skill);
                return;
            }

            target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 2);
            Viewer.Charge(storeIncident);
            NotifySuccess(target);
        }

        private bool TryGetEligibleSkill([CanBeNull] out SkillRecord skill, bool forDecrease = false)
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

        private void NotifySuccess([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Complete".Localize(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.SuccessDescription".LocalizeKeyed(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyDecrease([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Decrease".Localize(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.DecreaseDescription".LocalizeKeyed(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyFailed([NotNull] SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Failed".Localize(skill.def.label));

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
                "TKUtils.Passion.Hopped".LocalizeKeyed(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.HoppedDescription".LocalizeKeyed(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyDecreaseHopped([NotNull] SkillRecord viewerSkill, [NotNull] SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.Passion.DecreaseHopped".LocalizeKeyed(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.DecreaseHoppedDescription".LocalizeKeyed(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}

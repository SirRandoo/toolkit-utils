﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class RemovePassion : IncidentHelperVariables
    {
        private Pawn pawn;
        private SkillRecord target;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            target = pawn.skills.skills.Where(s => !s.TotallyDisabled)
               .FirstOrDefault(
                    s => s.def.defName.EqualsIgnoreCase(query.ToToolkit())
                         || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                         || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                );

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".Localize(query));
                return false;
            }

            if ((int) target.passion is { } passion && passion > 0 && passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemovePassion.None".Localize());
            return false;
        }

        public override void TryExecute()
        {
            if (!IncidentSettings.RemovePassion.Randomness)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
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
                skill.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
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
                skill.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
                Viewer.Charge(storeIncident);
                NotifyIncreaseHopped(target, skill);
                return;
            }

            target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 2);
            Viewer.Charge(storeIncident);
            NotifySuccess(target);
        }

        private void NotifySuccess(SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.Complete".Localize(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.SuccessDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyIncrease(SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.RemovePassion.Increase".Localize(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.IncreaseDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyFailed(SkillRecord skill)
        {
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.Passion.Failed".Localize(skill.def.label));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.FailedDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyHopped(SkillRecord viewerSkill, SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.Hopped".Localize(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.RemoveHoppedDescription".Localize(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyIncreaseHopped(SkillRecord viewerSkill, SkillRecord randomSkill)
        {
            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.RemovePassion.IncreaseHopped".Localize(viewerSkill.def.label, randomSkill.def.label)
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.IncreaseHoppedDescription".Localize(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

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
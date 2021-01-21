using System.Collections.Generic;
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
    public class AddPassion : IncidentHelperVariables
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
                    s => s.def.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                         || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                );

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".Localize(query));
                return false;
            }

            if ((int) target.passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Passion.Full".Localize());
            return false;
        }

        public override void TryExecute()
        {
            if (Viewer == null || pawn == null || target == null)
            {
                return;
            }

            if (!IncidentSettings.AddPassion.Randomness)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 2);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToFail.ToChance())
            {
                ChargeViewer();
                NotifyFailed(target);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToHop.ToChance() && TryGetEligibleSkill(out SkillRecord skill))
            {
                skill.passion = (Passion) Mathf.Clamp((int) skill.passion + 1, 0, 2);
                ChargeViewer();
                NotifyHopped(target, skill);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToDecrease.ToChance()
                && (target.passion == Passion.Minor || target.passion == Passion.Major))
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 2);
                ChargeViewer();
                NotifyDecrease(target);
                return;
            }

            if (IncidentSettings.AddPassion.ChanceToDecrease.ToChance()
                && IncidentSettings.AddPassion.ChanceToHop.ToChance()
                && TryGetEligibleSkill(out skill, true))
            {
                skill.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 2);
                ChargeViewer();
                NotifyDecreaseHopped(target, skill);
                return;
            }

            target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 2);
            ChargeViewer();
            NotifySuccess(target);
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

        private void ChargeViewer()
        {
            if (ToolkitSettings.UnlimitedCoins)
            {
                return;
            }

            Viewer.TakeViewerCoins(storeIncident.cost);
        }

        private void NotifySuccess(SkillRecord skill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Passion.Complete".Localize(skill.def.label));
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.SuccessDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyDecrease(SkillRecord skill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Passion.Decrease".Localize(skill.def.label));
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.DecreaseDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyFailed(SkillRecord skill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Passion.Failed".Localize(skill.def.label));
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.FailedDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyHopped(SkillRecord viewerSkill, SkillRecord randomSkill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Passion.Hopped".Localize(viewerSkill.def.label, randomSkill.def.label)
                );
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.HoppedDescription".Localize(
                    Viewer.username,
                    viewerSkill.def.label,
                    randomSkill.def.label
                ),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyDecreaseHopped(SkillRecord viewerSkill, SkillRecord randomSkill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Passion.DecreaseHopped".Localize(viewerSkill.def.label, randomSkill.def.label)
                );
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.DecreaseHoppedDescription".Localize(
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

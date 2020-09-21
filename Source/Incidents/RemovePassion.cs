using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
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
            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
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

            if ((int) target.passion is {} passion && passion > 0 && passion < 3)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemovePassion.None".Localize());
            return false;
        }

        public override void TryExecute()
        {
            if (Viewer == null || pawn == null || target == null)
            {
                return;
            }

            if (!IncidentSettings.RemovePassion.Randomness)
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 3);
                return;
            }

            if (Rand.Chance(IncidentSettings.RemovePassion.ChanceToFail))
            {
                ChargeViewer();
                NotifyFailed(target);
                return;
            }

            if (Rand.Chance(IncidentSettings.RemovePassion.ChanceToHop))
            {
                SkillRecord skill = GetRandomEligibleSkillForDecrease();

                if (skill == null)
                {
                    NotifyFailed(target);
                    return;
                }

                skill.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 3);
                ChargeViewer();
                NotifyHopped(target, skill);
                return;
            }

            if (Rand.Chance(IncidentSettings.RemovePassion.ChangeToIncrease))
            {
                target.passion = (Passion) Mathf.Clamp((int) target.passion - 1, 0, 3);
                ChargeViewer();
                NotifyIncrease(target);
                return;
            }

            if (Rand.Chance(IncidentSettings.RemovePassion.ChangeToIncrease)
                && Rand.Chance(IncidentSettings.RemovePassion.ChanceToHop))
            {
                SkillRecord skill = GetRandomEligibleSkillForDecrease();

                if (skill == null)
                {
                    NotifyFailed(target);
                    return;
                }

                skill.passion = (Passion) Mathf.Clamp((int) skill.passion - 1, 0, 3);
                ChargeViewer();
                NotifyIncreaseHopped(target, skill);
                return;
            }

            target.passion = (Passion) Mathf.Clamp((int) target.passion + 1, 0, 3);
            ChargeViewer();
            NotifySuccess(target);
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
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemovePassion.Complete".Localize(skill.def.label));
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.SuccessDescription".Localize(Viewer.username, skill.def.label),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void NotifyIncrease(SkillRecord skill)
        {
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemovePassion.Increase".Localize(skill.def.label));
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.PassionLetter.Title".Localize(),
                "TKUtils.PassionLetter.IncreaseDescription".Localize(Viewer.username, skill.def.label),
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
                    "TKUtils.RemovePassion.Hopped".Localize(viewerSkill.def.label, randomSkill.def.label)
                );
            }

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
            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.RemovePassion.IncreaseHopped".Localize(viewerSkill.def.label, randomSkill.def.label)
                );
            }

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

        private SkillRecord GetRandomEligibleSkill()
        {
            return pawn.skills.skills.Where(s => !s.TotallyDisabled)
               .Where(s => (int) s.passion < 3)
               .RandomElementWithFallback();
        }

        private SkillRecord GetRandomEligibleSkillForDecrease()
        {
            return pawn.skills.skills.Where(s => !s.TotallyDisabled)
               .Where(s => (int) s.passion is { } passion && passion <= 3 && passion > 0)
               .RandomElementWithFallback();
        }
    }
}

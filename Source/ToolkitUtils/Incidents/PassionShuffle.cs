using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PassionShuffle : IncidentHelperVariables
    {
        private Pawn pawn;
        private SkillDef target;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            int passions = pawn.skills.skills.Sum(skill => (int) skill.passion);

            if (passions <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.PassionShuffle.None".Localize());
                return false;
            }

            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return pawn.skills.skills.Any(s => (int) s.passion > (int) Passion.None);
            }

            target = pawn.skills.skills.FirstOrDefault(
                    s => s.def.defName.EqualsIgnoreCase(query.ToToolkit())
                         || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                         || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                )
              ?.def;

            if (target != null)
            {
                return pawn.skills.skills.Any(s => (int) s.passion > (int) Passion.None);
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidSkillQuery".Localize(query));
            return false;
        }

        public override void TryExecute()
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
                "TKUtils.PassionShuffleLetter.Description".Localize(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void Shuffle()
        {
            int passionCount = pawn.skills.skills.Sum(s => (int) s.passion);
            var iterations = 0;

            passionCount = GetPassionCount(passionCount);
            ShufflePassions(passionCount, ref iterations);
        }

        private void ShuffleWithInterests()
        {
            var iterations = 0;
            int passionCount = pawn.skills.skills.Select(s => (int) s.passion).Where(p => p < 3).Sum();
            List<Passion> interests = pawn.skills.skills
               .Where(s => (int) s.passion >= 3)
               .Select(s => s.passion)
               .ToList();

            passionCount = GetPassionCount(passionCount);

            if (!ShufflePassions(passionCount, ref iterations))
            {
                return;
            }

            while (interests.Any())
            {
                SkillRecord skill = pawn.skills.skills.Where(s => !s.TotallyDisabled)
                   .Where(s => s.passion == Passion.None)
                   .RandomElementWithFallback();

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
                SkillRecord skill = pawn.skills.skills.Where(s => !s.TotallyDisabled)
                   .Where(s => s.passion != Passion.Major)
                   .RandomElementWithFallback();

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

        private static void IncreasePassionFor(SkillRecord skill)
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
            foreach (SkillRecord skill in pawn.skills.skills)
            {
                if (skill.def == target && passionCount > 0)
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

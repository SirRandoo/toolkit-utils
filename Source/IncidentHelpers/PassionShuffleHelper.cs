using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class PassionShuffleHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        private SkillDef target;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, storeIncident.cost))
            {
                return false;
            }

            Pawn viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            int passions = viewerPawn.skills.skills.Sum(skill => (int) skill.passion);

            if (passions <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.PassionShuffle.NoPassions".Translate());
                return false;
            }

            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (!query.NullOrEmpty())
            {
                target = viewerPawn.skills.skills
                    .FirstOrDefault(
                        s => s.def.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                             || (s.def.skillLabel?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                             || (s.def.label?.ToToolkit().EqualsIgnoreCase(query.ToToolkit()) ?? false)
                    )
                    ?.def;

                if (target == null)
                {
                    MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.SkillQueryInvalid".Translate(query));
                    return false;
                }
            }

            pawn = viewerPawn;
            Viewer = viewer;
            return viewerPawn.skills.skills.Any(s => (int) s.passion > (int) Passion.None);
        }

        public override void TryExecute()
        {
            if (pawn == null)
            {
                return;
            }

            if (Interests.Active)
            {
                ShuffleWithInterests();
            }
            else
            {
                Shuffle();
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.PassionShuffle.Shuffled".Translate());
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.PassionShuffle.Title".Translate(),
                "TKUtils.Letters.PassionShuffle.Description".Translate(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }

        private void Shuffle()
        {
            int passionCount = pawn.skills.skills.Sum(s => (int) s.passion);
            var iterations = 0;

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

            while (passionCount > 0)
            {
                SkillRecord skill = pawn.skills.skills
                    .Where(s => !s.TotallyDisabled)
                    .Where(s => s.passion != Passion.Major)
                    .RandomElementWithFallback();

                if (skill == null)
                {
                    iterations += 1;
                    continue;
                }

                switch (skill.passion)
                {
                    case Passion.None:
                        skill.passion = Passion.Minor;
                        break;
                    case Passion.Minor:
                        skill.passion = Passion.Major;
                        break;
                }

                passionCount -= 1;
                iterations += 1;

                if (iterations < 150)
                {
                    continue;
                }

                TkLogger.Warn("Exceeded 100 iterations while shuffling passions!");
                return;
            }
        }

        private void ShuffleWithInterests()
        {
            var iterations = 0;
            int passionCount = pawn.skills.skills
                .Select(s => (int) s.passion)
                .Where(p => p < 3)
                .Sum();
            List<Passion> interests = pawn.skills.skills
                .Where(s => (int) s.passion >= 3)
                .Select(s => s.passion)
                .ToList();

            TkLogger.Info($"Interest count: {interests.Count}");

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

            while (passionCount > 0)
            {
                SkillRecord skill = pawn.skills.skills
                    .Where(s => !s.TotallyDisabled)
                    .Where(s => s.passion != Passion.Major)
                    .RandomElementWithFallback();

                if (skill == null)
                {
                    iterations += 1;
                    continue;
                }

                switch (skill.passion)
                {
                    case Passion.None:
                        skill.passion = Passion.Minor;
                        break;
                    case Passion.Minor:
                        skill.passion = Passion.Major;
                        break;
                }

                passionCount -= 1;
                iterations += 1;

                if (iterations < 150)
                {
                    continue;
                }

                TkLogger.Warn("Exceeded 100 iterations while shuffling passions!");
                return;
            }

            while (interests.Any())
            {
                SkillRecord skill = pawn.skills.skills
                    .Where(s => !s.TotallyDisabled)
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

                TkLogger.Warn("Exceeded 100 iterations while shuffling interests!");
                return;
            }
        }
    }
}

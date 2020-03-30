using System;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class PassionShuffleHelper : IncidentHelperVariables
    {
        private Pawn pawn;
        private SkillDef target;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, storeIncident.cost))
            {
                return false;
            }

            var viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var passions = viewerPawn.skills.skills.Sum(skill => (int) skill.passion);

            if (passions <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.PassionShuffle.NoPassions".Translate());
                return false;
            }

            var query = CommandParser.Parse(message, TkSettings.Prefix).Skip(2).FirstOrDefault();

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
            return true;
        }

        public override void TryExecute()
        {
            if (pawn == null)
            {
                return;
            }

            var passionCount = pawn.skills.skills.Sum(s => (int) s.passion);

            foreach (var skill in pawn.skills.skills)
            {
                if (skill.def == target)
                {
                    skill.passion = Passion.Minor;
                    passionCount -= 1;
                    continue;
                }
                
                skill.passion = Passion.None;
            }

            while (passionCount > 0)
            {
                foreach (var skill in pawn.skills.skills)
                {
                    if (skill.passion == Passion.Major)
                    {
                        continue;
                    }

                    if (Random.Range(0, 1) == 1)
                    {
                        passionCount -= 1;

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
                }
            }

            Viewer.TakeViewerCoins(storeIncident.cost);
            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.SkillShuffle.Shuffled".Translate());
            }
            
            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.PassionShuffle.Title".Translate(),
                "TKUtils.Letters.PassionShuffle.Description".Translate(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}

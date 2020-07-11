using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RemoveTraitHelper : IncidentHelperVariables
    {
        private TraitItem buyable;
        private Pawn pawn;
        private Trait trait;
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            string query = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            List<Trait> traits = pawn.story.traits.allTraits;

            if (traits?.Count <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.None".Localize());
                return false;
            }

            TraitItem traitQuery =
                TkUtils.Traits.FirstOrDefault(t => TraitHelper.CompareToInput(t, query));

            if (traitQuery == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(query));
                return false;
            }

            if (!traitQuery.CanRemove)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Disabled".Localize(query)
                );
                return false;
            }

            if (!Viewer.CanAfford(traitQuery.CostToRemove))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        traitQuery.CostToRemove.ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            Trait target = traits?.FirstOrDefault(t => TraitHelper.CompareToInput(traitQuery, t.Label));

            if (target == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".Localize(query));
                return false;
            }

            trait = target;
            buyable = traitQuery;
            return true;
        }

        public override void TryExecute()
        {
            if (pawn == null || trait == null)
            {
                return;
            }

            pawn.story.traits.allTraits.Remove(trait);
            TraitDegreeData data = trait.def.DataAtDegree(buyable.Degree);

            if (data?.skillGains != null)
            {
                foreach (KeyValuePair<SkillDef, int> gain in data.skillGains)
                {
                    SkillRecord skill = pawn.skills.GetSkill(gain.Key);
                    skill.Level -= gain.Value;
                }
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(buyable.CostToRemove);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, buyable.CostToRemove);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.RemoveTrait.Complete".Localize(trait.LabelCap)
                );
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.Trait.RemoveDescription".Localize(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}

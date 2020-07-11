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
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class AddTraitHelper : IncidentHelperVariables
    {
        private TraitItem buyableTrait;
        private Pawn pawn;
        private Trait trait;
        private TraitDef traitDef;
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            string traitQuery = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (traitQuery.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            buyableTrait = TkUtils.Traits.FirstOrDefault(t => TraitHelper.CompareToInput(t, traitQuery));
            int maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            List<Trait> traits = pawn.story.traits.allTraits;

            if (buyableTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(traitQuery));
                return false;
            }

            if (!buyableTrait.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Disabled".Localize(buyableTrait.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!Viewer.CanAfford(buyableTrait.CostToAdd))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        buyableTrait.CostToAdd.ToString("N0"),
                        Viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (traits != null)
            {
                int tally = traits.Count(t => !TraitHelper.IsSexualityTrait(t));
                bool canBypassLimit = buyableTrait.BypassLimit;

                if (tally >= maxTraits && !canBypassLimit)
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Trait.LimitReached".Localize(maxTraits)
                    );
                    return false;
                }
            }

            traitDef = DefDatabase<TraitDef>.AllDefsListForReading.FirstOrDefault(
                t => t.defName.Equals(buyableTrait.DefName)
            );

            if (traitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".Localize(traitQuery));
                return false;
            }

            foreach (Backstory backstory in pawn.story.AllBackstories)
            {
                if (!backstory.DisallowsTrait(traitDef, buyableTrait.Degree))
                {
                    continue;
                }

                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".Localize(backstory.identifier, traitQuery)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits.Any(t => t.defName.Equals(traitDef.defName)))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.LabelCap, traitQuery)
                );
                return false;
            }

            trait = new Trait(traitDef, buyableTrait.Degree);

            foreach (Trait t in pawn.story.traits.allTraits.Where(
                t => t.def.ConflictsWith(trait) || traitDef.ConflictsWith(t)
            ))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Conflict".Localize(t.LabelCap, trait.LabelCap)
                );
                return false;
            }

            if (traits?.Find(s => s.def.defName == trait.def.defName) == null)
            {
                return traitQuery != null && buyableTrait != null;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Trait.Duplicate".Localize(trait.Label)
            );
            return false;
        }

        public override void TryExecute()
        {
            pawn.story.traits.GainTrait(trait);
            TraitDegreeData val = traitDef.DataAtDegree(buyableTrait.Degree);
            if (val?.skillGains != null)
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in val.skillGains)
                {
                    SkillRecord skill = pawn.skills.GetSkill(skillGain.Key);
                    int level = TraitHelpers.FinalLevelOfSkill(pawn, skillGain.Key);
                    skill.Level = level;
                }
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(buyableTrait.CostToAdd);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, buyableTrait.CostToAdd);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Trait.Complete".Localize(trait.Label));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.AddDescription".Localize(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}

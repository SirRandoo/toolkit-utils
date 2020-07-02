using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Traits;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class AddTraitHelper : IncidentHelperVariables
    {
        private XmlTrait buyableTrait;
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
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".TranslateSimple());
                return false;
            }

            buyableTrait = TkUtils.ShopExpansion.Traits.FirstOrDefault(t => TraitHelper.MultiCompare(t, traitQuery));
            int maxTraits = AddTraitSettings.maxTraits > 0 ? AddTraitSettings.maxTraits : 4;
            List<Trait> traits = pawn.story.traits.allTraits;

            if (buyableTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(traitQuery));
                return false;
            }

            if (!buyableTrait.CanAdd)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuyTrait.AddDisabled".Translate(buyableTrait.Name.CapitalizeFirst())
                );
                return false;
            }

            if (!Viewer.CanAfford(buyableTrait.AddPrice))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.NotEnoughCoins".Translate(
                        buyableTrait.AddPrice.ToString("N0"),
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
                        "TKUtils.Responses.BuyTrait.LimitReached".Translate(maxTraits)
                    );
                    return false;
                }
            }

            traitDef = DefDatabase<TraitDef>.AllDefsListForReading.FirstOrDefault(
                t => t.defName.Equals(buyableTrait.DefName)
            );

            if (traitDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.TraitQueryInvalid".Translate(traitQuery));
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
                    "TKUtils.Responses.BuyTrait.Backstory".Translate(backstory.identifier, traitQuery)
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
                    "TKUtils.Responses.BuyTrait.Conflicts".Translate(t.LabelCap, trait.LabelCap)
                );
                return false;
            }

            if (traits?.Find(s => s.def.defName == trait.def.defName) == null)
            {
                return traitQuery != null && buyableTrait != null;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Responses.BuyTrait.Duplicate".Translate(trait.Label)
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
                Viewer.TakeViewerCoins(buyableTrait.AddPrice);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, buyableTrait.AddPrice);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.BuyTrait.Added".Translate(trait.Label));
            }

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.Letters.Trait.Title".TranslateSimple(),
                "TKUtils.Letters.Trait.Description".Translate(Viewer.username, trait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class AddTrait : IncidentHelperVariables
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

            buyableTrait = Data.Traits.FirstOrDefault(t => TraitHelper.CompareToInput(t, traitQuery));
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
                int tally = traits.Count(t => !t.IsSexualityTrait());
                bool canBypassLimit = buyableTrait.BypassLimit;

                if (tally >= maxTraits && !canBypassLimit)
                {
                    MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.LimitReached".Localize(maxTraits));
                    return false;
                }
            }

            traitDef = DefDatabase<TraitDef>.AllDefs.FirstOrDefault(t => t.defName.Equals(buyableTrait.DefName));

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

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(traitDef.defName)) ?? false)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.LabelCap, traitQuery)
                );
                return false;
            }

            if (AlienRace.Enabled && !AlienRace.IsTraitAllowed(pawn, traitDef, buyableTrait.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".Localize(pawn.kindDef.LabelCap, traitQuery)
                );
            }

            trait = new Trait(traitDef, buyableTrait.Degree);

            if (traits != null)
            {
                foreach (Trait t in traits.Where(t => t.def.ConflictsWith(trait) || traitDef.ConflictsWith(t)))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Trait.Conflict".Localize(t.LabelCap, trait.LabelCap)
                    );
                    return false;
                }
            }

            Trait duplicateTrait =
                traits?.FirstOrDefault(t => t.def.Equals(traitDef) && t.Degree != buyableTrait.Degree);
            if (duplicateTrait != null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Duplicate".Localize(duplicateTrait.Label, trait.Label)
                );
                return false;
            }

            if (!MagicComp.Active)
            {
                return traitQuery != null && buyableTrait != null;
            }

            List<TraitDef> classes = MagicComp.GetAllClasses().ToList();

            if (!classes.Any(c => c.defName.Equals(traitDef.defName)))
            {
                return traitQuery != null && buyableTrait != null;
            }

            foreach (TraitDef clazz in classes.Where(c => !c.defName.Equals(traitDef.defName)))
            {
                Trait traitOf = pawn.story.traits.GetTrait(clazz);

                if (traitOf == null)
                {
                    continue;
                }

                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Class".Localize(traitOf.LabelCap, trait.LabelCap)
                );
                return false;
            }

            return traitQuery != null && buyableTrait != null;
        }

        public override void TryExecute()
        {
            TraitHelper.GivePawnTrait(pawn, trait);

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(buyableTrait.CostToAdd);
            }

            Viewer.CalculateNewKarma(
                buyableTrait.Data?.KarmaTypeForAdding ?? storeIncident.karmaType,
                buyableTrait.CostToAdd
            );

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
